using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Ontology;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        [Description("")]
        public static Type EquivalentBHoMType(this OntologyResource individual)
        {
            Uri bhomUri = null;
            string bhomTypeFullName = null;

            // Get the equivalent type
            foreach (var type in individual.Types)
            {
                try
                {
                    bhomUri = type.AbsoluteUri();
                    bhomTypeFullName = bhomUri?.Segments.LastOrDefault();

                    if (!bhomTypeFullName.StartsWith("BH."))
                        continue;
                }
                catch { }
            }

            if (string.IsNullOrEmpty(bhomTypeFullName))
                return default(Type);

            string customTypeName = bhomTypeFullName.Split('.').Last();

            // Check if it is a custom type.
            if (bhomTypeFullName.Contains(typeof(BH.Engine.RDF.Types.CustomObjectType).FullName))
                return new Types.CustomObjectType(customTypeName);

            // See if a match can be found in the complete Type list of BHoM.
            Type bhomType = BH.Engine.Base.Query.AllTypeList().Where(t => t.FullName == bhomTypeFullName).FirstOrDefault();
            if (bhomType != null)
                return bhomType;

            // See if the type can be found among the currently loaded types.
            IEnumerable<Type> allLoadedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()).Where(t => t.Name == customTypeName);

            //if (allLoadedTypes.Count() == 1)
            //    bhomType = allLoadedTypes.FirstOrDefault();

            if (bhomType == null)
            {
                // If no match was found, this is likely a Custom Type that was not correctly detected in the previous steps,
                // or a non-bhom type that was not found in the current Assembly Domain.
                // Regardless, we want to return this as a CustomObject. To be consistent with the rest of the workflows,
                // we return a Custom Type, which later will be translated as a CustomObject.
                Log.RecordNote($"Could not find equivalent BHoM type for the class `{bhomTypeFullName}`. Returning it as a {typeof(BH.oM.Base.CustomObject).Name}.");
                return new Types.CustomObjectType(customTypeName);
            }

            return bhomType;
        }
    }
}