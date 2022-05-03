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
    public class LocalRepositorySettings : IObject
    {
        public string RepositoryRootPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "GitHub");
        public string CacheRootPath { get; set; } = Directory.GetParent(Directory.GetParent(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath).FullName).FullName;
        public string CacheFileName_RepositoryAllFilePaths { get; set; } = "cached_RepositoryAllFilePaths.txt";
        public string CacheFileName_PropertyInfoFileLines { get; set; } = "cached_PropertyInfoFileLines.txt";
        public string SaveDir_RelativeToRoot = "WebVOWLOntology";
        public Uri GithubOrganisation { get; set; } = new Uri(@"https://github.com/BHoM/");
        public string GithubOrganisationURL { get; set; } = "https://github.com/BHoM/";
        public bool ReadCacheFiles { get; set; } = true;
        public bool WriteCacheFiles { get; set; } = false;

    }
}
