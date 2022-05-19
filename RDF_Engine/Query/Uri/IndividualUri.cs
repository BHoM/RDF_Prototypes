using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Create
    {
        public static Uri IndividualUri(this object individual, OntologySettings ontologySettings)
        {
            string individualId = individual.IndividualId();
            string baseAddress = ontologySettings.ABoxSettings.IndividualsBaseAddress;

            Uri individualUri = Query.CombineUris(baseAddress, individualId);

            if (individualUri == null)
                Log.RecordError($"Could not compute the Uri for individual with Id `{individualId}` and with the base address obtained from the {nameof(ABoxSettings)}: `{baseAddress}`. Check that they are both correct.");

            return individualUri;
        }
    }
}
