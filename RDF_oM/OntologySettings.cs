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
        [Description("The base address where the ontology definition for Custom Types will be hosted. Custom Types are produced when computing an ontology that includes BHoM CustomObjects.")]
        public string CustomObjectTypesBaseAddress { get; set; } = $"http://customizeMeFrom-OntologySettings.TBoxSettings.{nameof(CustomObjectTypesBaseAddress)}";

        [Description("Types found in this dictionary will use the corresponding string URI when converted to ontology.")]
        public Dictionary<Type, string> TypeUris { get; set; } = new Dictionary<Type, string>();

        [Description("Default base URI used for unknown types. The full URI for the type will be this uri plus #typeName appended at its end.")]
        public string DefaultBaseUriForUnknownTypes { get; set; } = $"http://customizeFrom-OntologySettings.TBoxSettings.{nameof(DefaultBaseUriForUnknownTypes)}";

        [Description("If true, any CustomObject that has a Type key in its CustomData dictionary will be treated as if it was an instance of a custom class," +
            "which will be called like the value stored in the Type key.")]
        public bool TreatCustomObjectsWithTypeKeyAsCustomObjectTypes { get; set; } = true;

        [Description("Key of the CustomData dictionary that will be sought in CustomObjects. If a value is found there, and if the above option is true," +
            "the value will be used as if the CustomObject was a class called with this value.")]
        public string CustomobjectsTypeKey { get; set; } = "Type";
    }



    [Description("Settings for the definition of an Ontology's A-Box.")]
    public class ABoxSettings : IObject
    {
        [Description("The base address where the individuals will be hosted.")]
        public string IndividualsBaseAddress { get; set; } = $"http://customizeFrom-OntologySettings.ABoxSettings.{nameof(IndividualsBaseAddress)}";

        [Description("If this is set to true, the ABox Ontology will be assigned an individual also for any Class Property that is set to a default value.")]
        public bool ConsiderDefaultPropertyValues { get; set; } = true;

        [Description("If this is set to true, if an individual's ObjectProperty or Data property is null or an empty collection, it will still be added.")]
        public bool ConsiderNullOrEmptyPropertyValues { get; set; } = false;
    }
}
