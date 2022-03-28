using BH.oM.Analytical.Elements;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    [Description("Describes a property that subclasses of a thing must own. Typically represents properties of a C# interface.")]
    public class TBoxSettings : IObject
    {
        public string RepositoryRootPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "GitHub");
        public string CacheRootPath { get; set; } = Directory.GetParent(Directory.GetParent(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath).FullName).FullName;
        public string Cache_RepositoryAllFilePaths_FileName { get; set; } = "cached_GithubRootCsFilepaths.txt";
        public string SaveDir_RelativeToRoot = "WebVOWLOntology";
        public Uri GithubOrganisation { get; set; } = new Uri(@"https://github.com/BHoM/");
        public bool ResetCache { get; set; } = false;
    }
}
