
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing;
using BH.Engine.Base;
using BH.oM.RDF;
using BH.oM.Base.Attributes;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        [Description("Reads a TTL ontology and attempts to convert any A-Box individual into its CSharp object equivalent.")]
        [MultiOutputAttribute(0,"KG","Ontology produced whatever")]
        [MultiOutputAttribute(1, "OS", "Ontology Settings used to construct this KG")]
        public static Output<List<object>, OntologySettings> ReadTTL(string TTLtext)
        {

            if (string.IsNullOrWhiteSpace(TTLtext))
            {
                return new Output<List<object>, OntologySettings>();

            }
            

            // ############### Ontology Settings ############### 

            OntologySettings ontologySettings = new OntologySettings();

            string ontologyTitle = Convert.GetStringBetweenCharacters(TTLtext, "dc:title ", "@en;");
            ontologySettings.OntologyTitle = ontologyTitle.Replace("\"", string.Empty);

            string ontologyDescription = Convert.GetStringBetweenCharacters(TTLtext, "dc:description ", "@en.");
            ontologySettings.OntologyDescription = ontologyDescription.Replace("\"", string.Empty);

            string ontologyBaseAdress = Convert.GetStringBetweenCharacters(TTLtext, "@base   <", "> .");
            ontologySettings.OntologyBaseAddress = ontologyBaseAdress;


            // ############### TBOX Settings ############### 

            string customBaseAdress = null;
            string[] tokens = TTLtext.Split('#');
            foreach (string token in tokens)
            {
                if (token.Contains("customtype"))
                {
                    string found = Convert.GetUntilOrEmpty(token, "customtype");
                    customBaseAdress = found.Replace(" ", string.Empty);
                    break;
                }
            };

            if(!(customBaseAdress == null)) ontologySettings.TBoxSettings.CustomObjectTypesBaseAddress = customBaseAdress;


            string tBoxSettingsSubString = Convert.GetStringBetweenCharacters(TTLtext, "# TBoxSettings", "# TBoxSettings");


            //string typeUri = Convert.GetStringBetweenCharacters(TTLtext, "# TypeUris: ", "#");
            //ontologySettings.TBoxSettings.TypeUris = typeUri.Split(';').ToDictionary(s => Type.GetType(s.Split(',').First()), s => s.Split(',').Last()) ;

            string treatAsCustomObjectTypes = Convert.GetStringBetweenCharacters(TTLtext, "#TreatCustomObjectsWithTypeKeyAsCustomObjectTypes: ", " #TCOWTK");
            if (treatAsCustomObjectTypes != String.Empty)
            {
                ontologySettings.TBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes = bool.Parse(treatAsCustomObjectTypes.ToLower());
            }
            

            string customObjectsTypeKey = Convert.GetStringBetweenCharacters(TTLtext, "# CustomobjectsTypeKey: ", "#");
            ontologySettings.TBoxSettings.CustomobjectsTypeKey = customObjectsTypeKey;


            // ############### ABOX Settings ############### 

            string individualBaseAdress = Convert.GetStringBetweenCharacters(TTLtext, "#IndividualsBaseAdress :URL ", " #URL");
            ontologySettings.ABoxSettings.IndividualsBaseAddress = individualBaseAdress;

            string considerDefaultPropVal = Convert.GetStringBetweenCharacters(TTLtext, "#ConsiderDefaultPropertyValues: ", " #CDPV");
            if (considerDefaultPropVal.IsNullOrEmpty())
            {
                ontologySettings.ABoxSettings.ConsiderDefaultPropertyValues = bool.Parse(considerDefaultPropVal.ToLower());
            }
            
            string considerNullPropVal = Convert.GetStringBetweenCharacters(TTLtext, "#ConsiderNullOrEmptyPropertyValues: ", " #CNOEPV");
            if (considerNullPropVal.IsNullOrEmpty())
            {
                ontologySettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues = bool.Parse(considerNullPropVal.ToLower());
            }
            
            

            Output<List<object>, OntologySettings> output = new Output<List<object>, OntologySettings>
            {
                Item1 = Convert.ToCSharpObjects(TTLtext),
                Item2 = ontologySettings
            };
            return output;
        }


        [Description("Reads a TTL ontology and attempts to convert any A-Box individual into its CSharp object equivalent.")]
        public static Output<List<object>, OntologySettings> ReadTTL(string TTLfilePath, bool active = false)
        {
            if (!active)
                return new Output<List<object>, OntologySettings>();

            string TTLtext = File.ReadAllText(TTLfilePath);


            Output<List<object>, OntologySettings> output = new Output<List<object>, OntologySettings>
            {
                Item1 = null /*ReadTTL(TTLtext)*/,
                Item2 = new OntologySettings()
            };
            return output;
        }

    }
}
