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

            if (bhomTypeFullName.Contains(typeof(BH.Engine.RDF.Types.CustomObjectType).FullName))
                return new Types.CustomObjectType(bhomTypeFullName.Split('.').Last());

            Type bhomType = BH.Engine.Base.Query.AllTypeList().Where(t => t.FullName == bhomTypeFullName).FirstOrDefault();

            if (bhomType == null)
            {
                Log.RecordWarning($"Could not find equivalent BHoM type for {bhomTypeFullName}. Using {typeof(BH.oM.Base.CustomObject).Name}.");
                return typeof(BH.oM.Base.CustomObject);
            }

            return bhomType;
        }
    }
}