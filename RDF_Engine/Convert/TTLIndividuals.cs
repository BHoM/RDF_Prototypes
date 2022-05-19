using BH.oM.Base;
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
    public static partial class Convert
    {
        public static List<string> TTLIndividuals(this CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings)
        {
            List<string> TTLIndividuals = new List<string>();

            foreach (object individual in cSharpGraph.AllIndividuals)
            {
                string TTLIndividual = "";

                string individualId = individual.IndividualId();
                string individualUri = $"{ cSharpGraph.OntologySettings.ABoxSettings.IndividualsBaseAddress }/{individualId}";

                TTLIndividual += $"\n### {individualUri}";
                TTLIndividual += $"\n<{individualUri}> rdf:type owl:NamedIndividual ,";
                TTLIndividual += $"\n\t\t:{individual.GetType().UniqueNodeId()} ;";

                IEnumerable<IndividualRelation> individualRelations = cSharpGraph.IndividualRelations.Where(r => r.Individual == individual);

                foreach (IndividualRelation individualRelation in individualRelations)
                {
                    IndividualObjectProperty iop = individualRelation as IndividualObjectProperty;
                    IndividualDataProperty idp = individualRelation as IndividualDataProperty;

                    if (iop != null)
                    {
                        TTLIndividual += $"\n\t\t:{iop.HasProperty.PropertyInfo.UniqueNodeId()} {iop.RangeIndividual.IndividualId()} ;";
                    }
                    else if (idp != null)
                    {
                        TTLIndividual += "\n\t\t" + $@":{idp.PropertyInfo.UniqueNodeId()} ""{idp.StringValue()}""";

                        string dataType = idp.Value.GetType().ToOntologyDataType();

                        if (dataType == typeof(JsonSerialized).UniqueNodeId())
                            TTLIndividual += $"^^:{ idp.Value.GetType().ToOntologyDataType()};";
                        else
                            TTLIndividual += $"^^{ idp.Value.GetType().ToOntologyDataType()};"; // TODO: insert serialized value here, when the individual's datatype is unknown
                    }
                }

                TTLIndividual = TTLIndividual.ReplaceLastOccurenceOf(';', ".");
                TTLIndividuals.Add(TTLIndividual);
            }

            return TTLIndividuals;
        }
    }
}
