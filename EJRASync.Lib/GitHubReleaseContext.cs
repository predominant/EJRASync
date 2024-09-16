using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EJRASync.Lib
{
    [JsonSerializable(typeof(GitHubRelease))]
    [JsonSerializable(typeof(Asset))]
    public partial class GitHubReleaseContext : JsonSerializerContext
    {
    }
}
