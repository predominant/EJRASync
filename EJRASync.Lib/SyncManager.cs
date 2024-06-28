﻿using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Win32;
using System.IO;
using System.Security.AccessControl;
using System;
using System.Security.Principal;

namespace EJRASync.Lib
{
    public class SyncManager
    {
        private readonly IAmazonS3 _s3Client;

        private string _carsFolder = "cars";
        private string _tracksFolder = "tracks";

        public SyncManager(IAmazonS3 s3Client)
        {
            this._s3Client = s3Client;
            var steamPath = this.FindSteam();
            Console.WriteLine($"Steam path: {steamPath}");

            var acPath = this.FindAssettoCorsa(steamPath);
            Console.WriteLine($"Assetto Corsa path: {acPath}");

            this._carsFolder = Path.Combine(acPath, "content", this._carsFolder);
            Console.WriteLine($"Cars folder: {this._carsFolder}");

            this._tracksFolder = Path.Combine(acPath, "content", this._tracksFolder);
            Console.WriteLine($"Tracks folder: {this._tracksFolder}");
        }

        /// <summary>
        /// Finds the Steam installation path.
        /// </summary>
        /// <returns>Steam installation path</returns>
        /// <exception cref="Exception"></exception>
        private string FindSteam()
        {
            foreach (var (key, value) in Constants.SteamRegistryKeys)
            {
                var steamPath = (string)Registry.GetValue(key, value, null)!;
                if (steamPath == null)
                    continue;

                var libPath = Path.Combine(steamPath, Constants.SteamLibraryFile);

                if (!File.Exists(libPath))
                    continue;

                return libPath;
            }

            throw new Exception("Steam installation not found.");
        }

        /// <summary>
        /// Finds the Assetto Corsa installation path.
        /// </summary>
        /// <param name="steamLibraryPath">Steam library path</param>
        /// <returns>Assetto Corsa installation path</returns>
        /// <exception cref="Exception"></exception>
        private string FindAssettoCorsa(string steamLibraryPath)
        {
            var steamLibrary = File.ReadAllLines(steamLibraryPath);
            var libraryPath = "";

            foreach (var line in steamLibrary)
            {
                if (line.Contains('"' + "path" + '"'))
                {
                    libraryPath = line.Split('"')[3];
                    continue;
                }

                if (!line.Contains('"' + Constants.AssettoCorsaAppId + '"'))
                    continue;

                var acPath = Path.Combine(libraryPath, Constants.AssettoCorsaSubPath);

                if (!Directory.Exists(acPath))
                    continue;

                return acPath;
            }

            throw new Exception("Assetto Corsa installation not found.");
        }

        public async Task SyncBucketAsync(string bucketName, string localPath, bool forceInstall)
        {
            Console.WriteLine($"Syncing {bucketName} to {localPath}...");
            var s3Objects = await this.ListS3ObjectsAsync(bucketName);
            //s3Objects.ForEach(o => Console.WriteLine($"Object: {o.Key}"));
            var localFiles = Directory.GetFiles(localPath, "*", SearchOption.AllDirectories);

            var s3Keys = s3Objects.Select(o => o.Key).ToList();
            var localPaths = localFiles.Select(f => f.Replace(@"\", "").Replace(@"\", "/")).ToList();

            var downloadTasks = new List<Task>();


            foreach (var s3Object in s3Objects)
            {
                try
                {
                    var localFilePath = Path.Combine(localPath, s3Object.Key.Replace("/", @"\"));
                    var localDirectory = Path.GetDirectoryName(Path.GetFullPath(localFilePath));
                    Directory.CreateDirectory(localDirectory);

                    // Only DownloadFileAsync if the local file is older than the S3 object
                    if (File.Exists(localFilePath))
                    {
                        var localFileTime = File.GetLastWriteTime(localFilePath);
                        var s3ObjectTime = s3Object.LastModified.ToLocalTime();
                        if (localFileTime >= s3ObjectTime)
                        {
                            Console.WriteLine($"Skipping {s3Object.Key}...");
                            continue;
                        }
                    }

                    downloadTasks.Add(this.DownloadFileAsync(bucketName, s3Object.Key, localFilePath));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR encountered: '{s3Object.Key}': {e.Message}");
                }
            }

            await Task.WhenAll(downloadTasks);

                //// TODO: Implement deletion of local files not present in S3
                //var filesToDelete = localPaths.Except(s3Keys).ToList();
                //foreach (var fileToDelete in filesToDelete)
                //{
                //    var fullPath = fileToDelete.Replace("/", @"\");
                //    Console.WriteLine($"Deleting {fullPath}...");
                //    //File.Delete(Path.Combine(localPath, fullPath));
                //}
        }

        // Get a list of all top level folders in an S3 bucket
        public async Task<List<string>> ListS3FoldersAsync(string bucketName)
        {
            Console.WriteLine($"Listing folders in {bucketName}...");
            var folders = new List<string>();
            string? continuationToken = null;

            do
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Delimiter = "/",
                    ContinuationToken = continuationToken
                };
                var response = await this._s3Client.ListObjectsV2Async(request);
                folders.AddRange(response.CommonPrefixes);
                continuationToken = response.NextContinuationToken;
            } while (continuationToken != null);

            Console.WriteLine($"Found {folders.Count} folders in {bucketName}.");
            return folders;
        }

        public async Task<List<S3Object>> ListS3ObjectsAsync(string bucketName)
        {
            Console.WriteLine($"Listing objects in {bucketName}...");
            var objects = new List<S3Object>();
            string? continuationToken = null;

            do
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    ContinuationToken = continuationToken
                };
                var response = await this._s3Client.ListObjectsV2Async(request);
                objects.AddRange(response.S3Objects);
                continuationToken = response.NextContinuationToken;
            } while (continuationToken != null);

            Console.WriteLine($"Found {objects.Count} objects in {bucketName}.");
            return objects;
        }

        private async Task DownloadFileAsync(string bucketName, string key, string localPath)
        {
            if (key.EndsWith("/"))
            {
                Directory.CreateDirectory(localPath);
                return;
            }

            //if (File.Exists(localPath))
            //    this.DebugAccess(localPath);

            var metadataRequest = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = key
            };
            var metadataResponse = await this._s3Client.GetObjectMetadataAsync(metadataRequest);
            var remoteETag = metadataResponse.ETag.Trim('"');

            var localChecksum = File.Exists(localPath) ? FileChecksum.Calculate(localPath) : null;

            if (localChecksum != null && remoteETag == localChecksum)
            {
                Console.WriteLine($"Skipping {key}...");
                return;
            }

            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            Console.WriteLine($"Downloading {key}...");
            using var response = await this._s3Client.GetObjectAsync(request);
            await using var responseStream = response.ResponseStream;

            if (response.ContentLength == 0)
                await File.Create(localPath).DisposeAsync();

            await using var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write);
            await responseStream.CopyToAsync(fileStream);
        }

        private void DebugAccess(string localPath)
        {
            try
            {
                var fileInfo = new FileInfo(localPath);
                var fileSecurity = fileInfo.GetAccessControl();
                var acl = fileSecurity.GetAccessRules(true, true, typeof(NTAccount));

                Console.WriteLine($"Permissions for {localPath}:");
                Console.WriteLine(new string('-', 50));
                Console.WriteLine("{0,-25} {1,-15} {2}", "Identity", "Access Control Type", "Rights");
                Console.WriteLine(new string('-', 50));

                foreach (FileSystemAccessRule ace in acl)
                {
                    var identity = ace.IdentityReference.Value;
                    var accessControlType = ace.AccessControlType.ToString();
                    var rights = ace.FileSystemRights.ToString();

                    Console.WriteLine("{0,-25} {1,-15} {2}", identity, accessControlType, rights);
                }
                Console.WriteLine(new string('-', 50));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task SyncCarsAsync(bool forceInstall)
        {
            await this.SyncBucketAsync(Constants.CarsBucketName, this._carsFolder, forceInstall);
        }

        public async Task SyncTracksAsync(bool forceInstall)
        {
            await this.SyncBucketAsync(Constants.TracksBucketName, this._tracksFolder, forceInstall);
        }

        public async Task SyncAllAsync(bool forceInstall = false)
        {
            await this.SyncCarsAsync(forceInstall);
            await this.SyncTracksAsync(forceInstall);
        }
    }
}
