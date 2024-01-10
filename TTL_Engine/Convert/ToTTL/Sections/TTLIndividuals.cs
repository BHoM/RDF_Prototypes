/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Adapters.RDF;
using BH.oM.Base;
using BH.oM.Adapters.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Adapters.TTL
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

                string individualUri = individual.IndividualUri(cSharpGraph.GraphSettings).ToString();

                TTLIndividual += $"\n### {individualUri}";
                TTLIndividual += $"\n<{individualUri}> rdf:type owl:NamedIndividual ,";
                TTLIndividual += $"\n\t\t:{individual.IndividualType(cSharpGraph.GraphSettings.TBoxSettings).UniqueNodeId()} ;";

                TTLIndividual += TLLIndividualRelations(individual, cSharpGraph, localRepositorySettings); 

                TTLIndividual = TTLIndividual.EnsureEndingDot();

                stringBuilder.Append("\n\n" + TTLIndividual);
            }
        }

        private static string TLLIndividualRelations(object individual, CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings)
        {
            StringBuilder TLLIndividualRelations = new StringBuilder();
            IEnumerable<IIndividualRelation> individualRelations = cSharpGraph.IndividualRelations.Where(r => r.Individual == individual);

            foreach (IIndividualRelation individualRelation in individualRelations)
            {
                IndividualObjectProperty iop = individualRelation as IndividualObjectProperty;
                if (iop != null)
                {
                    // First check if the Object Property is a List.
                    // This check is done here rather than at the CSharpGraph stage because not all output formats support lists.
                    // TTL supports lists.
                    if (iop.IsListOfOntologyClasses(cSharpGraph.GraphSettings.TBoxSettings) ?? false)
                    {
                        var individualList = iop.RangeIndividual as IEnumerable<object>;
                        if (individualList.IsNullOrEmpty())
                            continue;

                        string individualParentUri = individual.IndividualUri(cSharpGraph.GraphSettings).ToString(); // variable name not optimal
                        TLLIndividualRelations.Append($"\n\t\t:{iop.HasProperty.PropertyInfo.UniqueNodeId()} <{individualParentUri}seq>. \n\n"); // Remove rdf:Seq and replace with indURI + seq

                        TLLIndividualRelations.Append($"\n### {individualParentUri}seq");
                        TLLIndividualRelations.Append($"\n<{individualParentUri}seq>. rdf:type owl:NamedIndividual, \t:rdf:Seq;\n");

                        List<string> listIndividualsUris = individualList.Where(o => o != null).Select(o => o.IndividualUri(cSharpGraph.GraphSettings).ToString()).ToList();
                        for (int i = 0; i < listIndividualsUris.Count; i++)
                        {
                            string individualUri = listIndividualsUris[i];

                            TLLIndividualRelations.Append($"\t\t\trdf:_{i} <{individualUri}> ;\n"); // subindividuals are added here
                        }

                        for (int i = 0; i < listIndividualsUris.Count; i++) // individuals again here with geometry
                        {
                            string individualUri = listIndividualsUris[i];

                            TLLIndividualRelations.Append($"\n### {individualUri}"); 
                            TLLIndividualRelations.Append($"\n<{individualUri}> rdf:type owl:NamedIndividual, \t{individualList.ToList()[i]};\n");
                        }
                        

                    }
                    else if (iop.RangeIndividual?.GetType().IsListOfDatatypes(cSharpGraph.GraphSettings.TBoxSettings) ?? false)
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

                        TLLIndividualRelations.Append($"\n\t\t:{iop.HasProperty.PropertyInfo.UniqueNodeId()} <{iop.RangeIndividual.IndividualUri(cSharpGraph.GraphSettings)}> ;");

                    continue;
                }

                IndividualDataProperty idp = individualRelation as IndividualDataProperty;
                if (idp != null && idp.Value != null)
                {

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

