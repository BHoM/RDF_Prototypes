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
    public interface IOntologySettings : IObject
    {
        string OntologyTitle { get; set; }
        string OntologyDescription { get; set; }
        string OntologyBaseAddress { get; set; }
    }

    public class TBoxSettings : IOntologySettings
    {
        public string OntologyTitle { get; set; } = $"{DateTime.Now.ToString("yyMMdd-HHmmss")}_newBHoMOntology";
        public string OntologyDescription { get; set; } = $"New BHoM ontology";
        public string OntologyBaseAddress { get; set; } = "http://visualdataweb.org/";
    }

    public class ABoxSettings : IOntologySettings
    {
        public string OntologyTitle { get; set; } = $"{DateTime.Now.ToString("yyMMdd-HHmmss")}_newBHoMOntology";
        public string OntologyDescription { get; set; } = $"New BHoM ontology";
        public string OntologyBaseAddress { get; set; } = "https://www.intcdc.uni-stuttgart.de/internal/";

        [Description("If this is set to true, the ABox Ontology will be assigned an individual also for any Class Property that is set to a default value.")]
        public bool AddDefaultPropertyValues { get; set; } = true;
    }
}
