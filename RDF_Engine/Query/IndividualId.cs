using BH.oM.Base;
using BH.oM.RDF;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static string IndividualId(this object individual)
        {
            if (individual == null)
                return null;

            // If it is an IObject, simply use BHoM's Hash method.
            IObject iObject = individual as IObject;
            if (iObject != null)
                return BH.Engine.Base.Query.Hash(iObject);

            // Otherwise, obtain a static, repeatable Hash by serializing the object, encoding it in Base64, and then computing a hash of the resulting string.
            // This is slow but should only happen in edge cases.
            string base64Serialized = Convert.ToBase64JsonSerialized(individual);
            string base64SerializedHash = Query.SHA256Hash(base64Serialized);

            return base64SerializedHash;
        }
    }
}
