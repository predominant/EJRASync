﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJRASync.Lib
{
    public static class Constants
    {
        public static readonly string AssettoCorsaAppId = "244210";
        public static readonly string AssettoCorsaSubPath = @"steamapps\common\assettocorsa";
        
        public static readonly string CarsBucketName = "ejra-cars";
        public static readonly string TracksBucketName = "ejra-tracks";

        public static readonly Dictionary<string, string> SteamRegistryKeys = new()
        {
            { @"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath" },
            { @"HKEY_LOCAL_MACHINE\Software\Valve\Steam", "InstallPath" },
        };
        public static readonly string SteamLibraryFile = @"steamapps\libraryfolders.vdf";
        public static readonly string MinioUrl = "http://usw01.grahamweldon.com:9002";
    }
}
