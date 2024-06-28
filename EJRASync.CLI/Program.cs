using Amazon.S3;
using EJRASync.Lib;
using System.Security.Principal;

SentrySdk.Init(options =>
{
    options.Dsn = "https://9545721f9e247759f9a2902d79123937@o323948.ingest.us.sentry.io/4507506715983872";
    options.Debug = false;
    options.AutoSessionTracking = true;
    options.TracesSampleRate = 1.0;
    options.ProfilesSampleRate = 1.0;
});

var currentUser = WindowsIdentity.GetCurrent().Name;
Console.WriteLine($"Current user: {currentUser}");

var s3Client = new AmazonS3Client("", "", new AmazonS3Config
{
    ServiceURL = Constants.MinioUrl,
    ForcePathStyle = true,
});

var syncManager = new SyncManager(s3Client);

//var carsObjects = await syncManager.ListS3ObjectsAsync(Constants.CarsBucketName);
//Console.WriteLine($"Cars in S3 bucket: {Constants.CarsBucketName}");
//carsObjects.ForEach(x => Console.WriteLine($"Car: {x.Key}"));

syncManager.SyncAllAsync().Wait();

Console.WriteLine("Sync complete.");
Console.WriteLine("");
Console.WriteLine("Press any key to exit.");
Console.ReadKey();
