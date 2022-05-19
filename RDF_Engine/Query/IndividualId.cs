using BH.oM.Base;
using BH.oM.RDF;
using Newtonsoft.Json;
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
    public static partial class Query
    {
        public static string IndividualId(this object individual)
        {
            string individualId;

            IBHoMObject bHoMObject = individual as IBHoMObject;
            if (bHoMObject != null)
                return bHoMObject.BHoM_Guid.ToString();

            // TODO: What do we do when an individual does not have a Guid assigned?
            // We could take its Hash, but that is not unique/repeatable. 
            individualId = individual.GetHashCode().ToString();

            return individualId;
        }
    }
}
