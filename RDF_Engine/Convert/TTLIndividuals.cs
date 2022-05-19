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

                string individualUri = individual.IndividualUri(cSharpGraph.OntologySettings).ToString();

                TTLIndividual += $"\n### {individualUri}";
                TTLIndividual += $"\n<{individualUri}> rdf:type owl:NamedIndividual ,";
                TTLIndividual += $"\n\t\t:{individual.GetType().UniqueNodeId()} ;";

                TTLIndividual += TLLIndividualRelations(individual, cSharpGraph, localRepositorySettings);

                TTLIndividual = TTLIndividual.ReplaceLastOccurenceOf(';', ".");
                TTLIndividuals.Add(TTLIndividual);
            }

            return TTLIndividuals;
        }

        private static string TLLIndividualRelations(object individual, CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings)
        {
            string TLLIndividualRelations = "";
            List<IndividualRelation> individualRelations = cSharpGraph.IndividualRelations.Where(r => r.Individual == individual).ToList();

            foreach (IndividualRelation individualRelation in individualRelations)
            {
                IndividualObjectProperty iop = individualRelation as IndividualObjectProperty;
                if (iop != null)
                {
                    TLLIndividualRelations += $"\n\t\t:{iop.HasProperty.PropertyInfo.UniqueNodeId()} <{iop.RangeIndividual.IndividualUri(cSharpGraph.OntologySettings)}> ;";
                }

                IndividualDataProperty idp = individualRelation as IndividualDataProperty;
                if (idp != null)
                {
                    TLLIndividualRelations += "\n\t\t" + $@":{idp.PropertyInfo.UniqueNodeId()} ""{idp.StringValue()}""";

                    string dataType = idp.Value.GetType().ToOntologyDataType();

                    if (dataType == typeof(JsonSerialized).UniqueNodeId())
                        TLLIndividualRelations += $"^^:{ idp.Value.GetType().ToOntologyDataType()};";
                    else
                        TLLIndividualRelations += $"^^{ idp.Value.GetType().ToOntologyDataType()};"; // TODO: insert serialized value here, when the individual's datatype is unknown
                }
            }

            return TLLIndividualRelations;
        }
    }
}
