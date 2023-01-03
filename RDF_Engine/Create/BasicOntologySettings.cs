using BH.oM.RDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Create
    {
        public static OntologySettings BasicOntologySettings(string ontologyTitle, string ontologyDescription, string tBoxURI, string aBoxURI)
        {
            return new OntologySettings()
            {
                TBoxSettings = new TBoxSettings()
                {
                    CustomObjectTypesBaseAddress = tBoxURI

                },
                ABoxSettings = new ABoxSettings()
                {
                    IndividualsBaseAddress = aBoxURI
                },
                OntologyBaseAddress = tBoxURI,
                OntologyTitle = ontologyTitle,
                OntologyDescription = ontologyDescription
            };
        }
    }
}
