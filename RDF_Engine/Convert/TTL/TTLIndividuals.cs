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
        private static void TTLIndividuals(this CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings, StringBuilder stringBuilder)
        {
            foreach (object individual in cSharpGraph.AllIndividuals)
            {
                if (individual == null)
                    continue;

                string TTLIndividual = "";

                string individualUri = individual.IndividualUri(cSharpGraph.OntologySettings).ToString();

                TTLIndividual += $"\n### {individualUri}";
                TTLIndividual += $"\n<{individualUri}> rdf:type owl:NamedIndividual ,";
                TTLIndividual += $"\n\t\t:{individual.IndividualType(cSharpGraph.OntologySettings.TBoxSettings).UniqueNodeId()} ;";

                TTLIndividual += TLLIndividualRelations(individual, cSharpGraph, localRepositorySettings);

                TTLIndividual = TTLIndividual.EnsureEndingDot();

                stringBuilder.Append("\n\n" + TTLIndividual);
            }
        }

        private static string TLLIndividualRelations(object individual, CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings)
        {
            StringBuilder TLLIndividualRelations = new StringBuilder();
            IEnumerable<IndividualRelation> individualRelations = cSharpGraph.IndividualRelations.Where(r => r.Individual == individual);

            foreach (IndividualRelation individualRelation in individualRelations)
            {
                IndividualObjectProperty iop = individualRelation as IndividualObjectProperty;
                if (iop != null)
                {
                    // First check if the Object Property is a List.
                    // This check is done here rather than at the CSharpGraph stage because not all output formats support lists.
                    // TTL supports lists.
                    if (iop.IsListOfOntologyClasses())
                    {
                        var individualList = iop.RangeIndividual as IEnumerable<object>;
                        if (individualList.IsNullOrEmpty())
                            continue;

                        List<string> listIndividualsUris = individualList.Where(o => o != null).Select(o => o.IndividualUri(cSharpGraph.OntologySettings).ToString()).ToList();
                        TLLIndividualRelations.Append($"\n\t\t:{iop.HasProperty.PropertyInfo.UniqueNodeId()} rdf:Seq ;\n");

                        for (int i = 0; i < listIndividualsUris.Count; i++)
                        {
                            string individualUri = listIndividualsUris[i];

                            TLLIndividualRelations.Append($"\t\trdf:_{i} <{individualUri}> ;\n");
                        }
                    }
                    else if (iop.RangeIndividual.GetType().IsListOfDatatypes())
                    {
                        var individualList = iop.RangeIndividual as IEnumerable;
                        if (individualList.IsNullOrEmpty())
                            continue;

                        TLLIndividualRelations.Append($"\n\t\t:{iop.HasProperty.PropertyInfo.UniqueNodeId()} ");

                        List<string> stringValues = new List<string>();
                        foreach (var value in individualList)
                            stringValues.Add($"\"{Query.DataPropertyStringValue(value)}\"^^{value.GetType().ToOntologyDataType()}");

                        TLLIndividualRelations.Append($"({string.Join(" ", stringValues)});");
                    }
                    else

                        TLLIndividualRelations.Append($"\n\t\t:{iop.HasProperty.PropertyInfo.UniqueNodeId()} <{iop.RangeIndividual.IndividualUri(cSharpGraph.OntologySettings)}> ;");

                    continue;
                }

                IndividualDataProperty idp = individualRelation as IndividualDataProperty;
                if (idp != null)
                {
                    if (idp.Value is IEnumerable)
                    {

                    }

                    TLLIndividualRelations.Append("\n\t\t" + $@":{idp.PropertyInfo.UniqueNodeId()} ""{idp.DataPropertyStringValue()}""");

                    string dataType = idp.Value.GetType().ToOntologyDataType();

                    if (dataType == typeof(Base64JsonSerialized).UniqueNodeId())
                        TLLIndividualRelations.Append($"^^:{idp.Value.GetType().ToOntologyDataType()};");
                    else
                        TLLIndividualRelations.Append($"^^{ idp.Value.GetType().ToOntologyDataType()};"); // TODO: insert serialized value here, when the individual's datatype is unknown

                    continue;
                }
            }

            return TLLIndividualRelations.ToString();
        }
    }
}
