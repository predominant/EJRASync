using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJRASync.Lib
{
    public static class Constants
    {
        public static readonly string Version = "0.0.12";

        public static readonly string AssettoCorsaAppId = "244210";
        public static readonly string AssettoCorsaSubPath = @"steamapps\common\assettocorsa";
        
        public static readonly string CarsBucketName = "ejra-cars";
        public static readonly string TracksBucketName = "ejra-tracks";
        public static readonly string FontsBucketName = "ejra-fonts";
        public static readonly string AppsBucketName = "ejra-apps";

        public static readonly Dictionary<string, string> SteamRegistryKeys = new()
        {
            { @"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath" },
            { @"HKEY_LOCAL_MACHINE\Software\Valve\Steam", "InstallPath" },
        };
        public static readonly string SteamLibraryFile = @"steamapps\libraryfolders.vdf";
        public static readonly string MinioUrl = "http://usw01.grahamweldon.com:9002";

        public static readonly string CarsYamlFile = "cars.yaml";
        public static readonly string TracksYamlFile = "tracks.yaml";

        public static readonly string SentryDSN =
            "https://9545721f9e247759f9a2902d79123937@o323948.ingest.us.sentry.io/4507506715983872";

        public static readonly string GithubReleaseURL = "https://api.github.com/repos/predominant/ejrasync/releases/latest";

        public static readonly string ExecutableName = "EJRASync.CLI.exe";

        public static readonly string UserAgent = "EJRASync.Lib.AutoUpdater";
    }
}
