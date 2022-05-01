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
    [Description("Settings for the definition of an Ontology.")]
    public class OntologySettings : IObject
    {
        public string OntologyTitle { get; set; } = $"{DateTime.Now.ToString("yyMMdd-HHmmss")}_newBHoMOntology";
        public string OntologyDescription { get; set; } = $"New BHoM ontology";
        public string OntologyBaseAddress { get; set; } = "http://visualdataweb.org/";

        public TBoxSettings TBoxSettings { get; set; } = new TBoxSettings();
        public ABoxSettings ABoxSettings { get; set; } = new ABoxSettings();
    }

    [Description("Settings for the definition of an Ontology's T-Box.")]
    public class TBoxSettings : IObject
    {
        public Type DefaultTypeForUnknowns { get; set; } = typeof(JsonSerialized);
    }

    [Description("Settings for the definition of an Ontology's A-Box.")]
    public class ABoxSettings : IObject
    {
        public string IndividualsBaseAddress { get; set; } = "https://www.intcdc.uni-stuttgart.de/internal/";

        [Description("If this is set to true, the ABox Ontology will be assigned an individual also for any Class Property that is set to a default value.")]
        public bool ConsiderDefaultPropertyValues { get; set; } = true;

        [Description("If this is set to true, if an individual's ObjectProperty or Data property is null or an empty collection, it will still be added.")]
        public bool ConsiderNullOrEmptyPropertyValues { get; set; } = false;
    }
}
