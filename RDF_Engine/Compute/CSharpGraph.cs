/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing;
using BH.Engine.Base;
using BH.oM.Adapters.RDF;
using BH.oM.Base.Attributes;
using BH.Engine.Adapters.RDF.Types;
using System.Collections;
using VDS.RDF.Ontology;

namespace BH.Engine.Adapters.RDF
{
    public static partial class Compute
    {
        [Description("Returns an ontology graph that includes CSharp objects and types." +
            "This methods takes in a list of objects as an input, so the resulting CSharp graph has both the T-Box and A-Box sections populated." +
            "If you only are interested in the T-Box, use the other CSharpGraph() method that takes in a list of Types.")]
        public static CSharpGraph CSharpGraph(this List<object> objects, GraphSettings graphSettings = null)
        {
            // First, check if the input objects include 1 single CSharpGraph, in which case just return it.
            // Do not allow to input CSharpGraph objects together with any other kind of object.
            var inputGraphs = objects.OfType<CSharpGraph>();
            if (inputGraphs.Any())
            {
                if (inputGraphs.Count() != objects.Count())
                {
                    Log.RecordError($"Please input either a set of BHoM Objects or 1 single {nameof(CSharpGraph)} object. Not both.");
                    return null;
                }
                else
                    return inputGraphs.FirstOrDefault();
            }

            graphSettings = graphSettings ?? new GraphSettings();
            string messageToAppend = $"Please check your inputs of the {nameof(graphSettings)} component. ";

            if (!Query.IsValidURI(graphSettings.OntologyBaseAddress, messageToAppend) || 
                !Query.IsValidURI(graphSettings.TBoxSettings.CustomObjectTypesBaseAddress, messageToAppend) ||
                !Query.IsValidURI(graphSettings.TBoxSettings.DefaultBaseUriForUnknownTypes, messageToAppend) ||
                !Query.IsValidURI(graphSettings.ABoxSettings.IndividualsBaseAddress, messageToAppend))
                return null;

            m_cSharpGraph = new CSharpGraph() { GraphSettings = graphSettings };

            foreach (var iObject in objects)
                AddIndividualToOntology(iObject, graphSettings);

            return m_cSharpGraph;
        }

        /***************************************************/

        [Description("Returns an ontology graph that includes CSharp objects and types. " +
            "This methods takes in a list of Types as an input, so the resulting CSharp graph has only the T-Box." +
            "For a complete graph that also includes the A-Box, use the other CSharpGraph() method that takes in a list of objects.")]
        public static CSharpGraph CSharpGraph(this List<Type> types, GraphSettings graphSettings = null)
        {
            graphSettings = graphSettings ?? new GraphSettings();
            m_cSharpGraph = new CSharpGraph() { GraphSettings = graphSettings };

            foreach (var type in types)
                AddToOntology(type, graphSettings);

            return m_cSharpGraph;
        }


        /***************************************************/
        // Private methods
        /***************************************************/

        private static void AddToOntology(this Type type, GraphSettings graphSettings)
        {
            if (type == typeof(CustomObjectType))
                return; // only add sub-types of CustomObjectType.

            if (type is ListPropertyType)
                return; // do not add type for list property.

            CustomObjectType cType = type as CustomObjectType;
            if (cType != null && m_cSharpGraph.Classes.OfType<CustomObjectType>().Where(ct => ct.Name == cType.Name).SelectMany(ct => ct.PropertyNames.Except(cType.PropertyNames)).Any())
                throw new ArgumentException($"The input contained multiple CustomObjects with the same Type key `{cType.Name}` which had different properties. Make sure that all instances of `{cType.Name}` have the same property names.");

            if (m_cSharpGraph.Classes.Contains(type))
                return;

            //if (type.IsCollectionOfOntologyClasses())
            //    type = type.InnermostType();

            if (type.IsOntologyClass(graphSettings.ABoxSettings))
                m_cSharpGraph.Classes.Add(type);

            var props = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            props.AddToOntology(graphSettings);

            // Recurse for parent Types.
            List<Type> parentTypes = type.BaseTypes();
            foreach (var parentType in parentTypes)
            {
                if (!parentType.IsOntologyClass(graphSettings.ABoxSettings))
                    continue;

                AddToOntology(parentType, graphSettings);
            }
        }

        private static void AddToOntology(this IEnumerable<PropertyInfo> pInfos, GraphSettings graphSettings, object obj = null, PropertyInfo fromProperty = null)
        {
            foreach (var pi in pInfos)
            {
                if (pi is CustomPropertyInfo && obj != null)
                {
                    object individualPropertyValue = pi.GetValue(obj);
                    AddToOntology((CustomPropertyInfo)pi, graphSettings, obj, fromProperty, individualPropertyValue);
                }
                else
                    AddToOntology(pi as dynamic, graphSettings, obj, fromProperty);
            }
        }

        private static void AddToOntology(this CustomPropertyInfo customPI, GraphSettings graphSettings, object individual, PropertyInfo fromProperty, object individualPropertyValue)
        {
            if (customPI.Name == "Type")
                return; // do not add the `Type` property to the ontology for Custom Types.

            AddToOntology((PropertyInfo)customPI, graphSettings, individual, fromProperty, individualPropertyValue);
        }

        private static void AddToOntology(this PropertyInfo pi, GraphSettings graphSettings, object individual, PropertyInfo individualFromProperty, object individualPropertyValue = null)
        {
            // In C#'s Reflection, relations are represented with PropertyInfos.
            // In an ontology, PropertyInfos may correspond to either ObjectProperties or DataProperties.

            Type domainType = pi.DeclaringType;
            Type rangeType = pi.PropertyType;

            // Get all parent classes and parent interfaces of the Declaring type
            List<Type> parentTypes = pi.DeclaringType.BaseTypes();

            // Get all properties of them and
            // if the input pi is among the parentProperties,
            // then the domain type is the parent type.
            foreach (Type parentType in parentTypes)
            {
                var parentProperties = parentType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);

                if (parentProperties.Any(pp => pp.Name == pi.Name))
                {
                    domainType = parentType;
                    break;
                }
            }

            if (!domainType.IsOntologyClass(graphSettings.ABoxSettings))
                return; // do not add Properties of classes that are not Ontology classes (e.g. if domainType is a String, we do not want to add its property Chars).
            else
                domainType.AddToOntology(graphSettings);


            if (pi.IsDataProperty(graphSettings.ABoxSettings))
            {
                DataProperty hasPropertyRelation = new DataProperty() { PropertyInfo = pi, DomainClass = domainType, RangeType = rangeType };
                AddIndividualDataPropertyRelation(individual, pi, graphSettings, hasPropertyRelation, individualPropertyValue);
                return;
            }

            if (pi.IsObjectProperty(graphSettings.ABoxSettings))
            {
                // OBJECT PROPERTY RELATION
                // The relation between Individuals corresponds to an ObjectPropertyRelation (between two Classes of the Ontology).

                if (pi.PropertyType.IsListOfOntologyClasses(individualPropertyValue, graphSettings.ABoxSettings) && individual != null)
                {
                    rangeType = new ListPropertyType(individual, pi, graphSettings.TBoxSettings);

                    IList list = pi.GetValue(individual) as IList;

                    if (list != null && list.Count != 0)
                    {
                        foreach (var item in list)
                        {
                            //m_cSharpGraph.AllIndividuals.Add(item);

                            AddIndividualToOntology(item, graphSettings, pi);

                            // Recurse for this individual's relations.
                            PropertyInfo[] listItemProps = item?.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) ?? new PropertyInfo[] { };
                            AddToOntology(listItemProps, graphSettings, item, individualFromProperty);
                        }
                    }
                }

                // Make sure the RangeType is added to the ontology.
                rangeType.AddToOntology(graphSettings);

                // Add the ObjectProperty to the Graph for the T-Box.
                ObjectProperty hasPropertyRelation = new ObjectProperty() { PropertyInfo = pi, DomainClass = domainType, RangeClass = rangeType };
                m_cSharpGraph.ObjectProperties.Add(hasPropertyRelation);

                AddIndividualObjectPropertyRelation(individual, pi, graphSettings, hasPropertyRelation, individualPropertyValue);
                return;
            }
        }

        private static void AddIndividualObjectPropertyRelation(object individual, PropertyInfo pi, GraphSettings graphSettings, ObjectProperty hasPropertyRelation, object propertyValue = null)
        {
            // If the individual is non-null, we will need to add the individuals' relation to the Graph in order to define the A-Box.
            if (!pi.IsObjectProperty(graphSettings.ABoxSettings))
            {
                Log.RecordError("Wrong PropertyInfo specified.");
                return;
            }

            if (individual == null) return;
            if (propertyValue == null)
                try
                {
                    propertyValue = pi.CanRead ? pi.GetValue(individual) : null;
                }
                catch { }

            if (!graphSettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues && propertyValue.IsNullOrEmpty())
                return;

            IndividualObjectProperty rel = new IndividualObjectProperty()
            {
                HasProperty = hasPropertyRelation,
                Individual = individual,
                RangeIndividual = propertyValue
            };

            if (m_cSharpGraph.IndividualRelations.Contains(rel))
                return;

            m_cSharpGraph.IndividualRelations.Add(rel);

            // Recurse for the individual's property value, which will be another individual.
            AddIndividualToOntology(propertyValue, graphSettings);
        }

        private static void AddIndividualDataPropertyRelation(object individual, PropertyInfo pi, GraphSettings graphSettings, DataProperty hasPropertyRelation, object propertyValue = null)
        {
            if (!pi.IsDataProperty(graphSettings.ABoxSettings))
            {
                Log.RecordError("Wrong PropertyInfo specified.");
                return;
            }
            // DATA PROPERTY RELATION
            // We do not have an Ontology class corresponding to the rangeType:
            // this PropertyInfo relation corresponds to a Data property.

            // Add the ObjectProperty to the Graph for the T-Box.
            m_cSharpGraph.DataProperties.Add(hasPropertyRelation);

            // If the individual is non-null, we will need to add the individuals' relation to the Graph in order to define the A-Box.
            if (individual == null) return;

            if (propertyValue == null)
                try
                {
                    propertyValue = pi.CanRead ? pi.GetValue(individual) : null;
                }
                catch { }

            if (!graphSettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues && propertyValue.IsNullOrEmpty())
                return;

            IndividualDataProperty rel = new IndividualDataProperty()
            {
                Individual = individual,
                Value = propertyValue,
                PropertyInfo = pi
            };

            m_cSharpGraph.IndividualRelations.Add(rel);
        }


        private static void AddIndividualToOntology(object individual, GraphSettings graphSettings, PropertyInfo fromProperty = null)
        {
            if (individual == null)
                return;

            Type individualType = individual.IndividualType(graphSettings.TBoxSettings);
            graphSettings = graphSettings ?? new GraphSettings();

            // Only individuals that are of types mappable to Ontology classes can be added.
            if (individualType.IsOntologyClass(graphSettings.ABoxSettings))
            {
                // Make sure the individual type is among the ontology classes.
                individualType.AddToOntology(graphSettings);

                // Add the individual.
                m_cSharpGraph.AllIndividuals.Add(individual);
            }

            // Get this individual's properties.
            List<PropertyInfo> properties = individualType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).ToList();
            BHoMObject individualAsBHoMObj = individual as BHoMObject;
            if (individualAsBHoMObj != null && !(individualType is CustomObjectType))
            {
                // It's not a Custom Type. Let's consider any entry in its CustomData dictionary as an extra property.
                foreach (var entry in individualAsBHoMObj.CustomData)
                {
                    CustomPropertyInfo customProp = new CustomPropertyInfo(individualType, entry.Key, entry.Value?.GetType() ?? typeof(object));

                    int idx = properties.IndexOf(properties.Where(p => p.Name == nameof(BHoMObject.BHoM_Guid)).FirstOrDefault());
                    if (idx != -1)
                        properties.Insert(idx, customProp);
                    else
                        properties.Add(customProp);
                }
            }

            // Recurse for properties of this individual.
            properties.AddToOntology(graphSettings, individual, fromProperty);

            if (individualType is CustomObjectType)
                return;

            // Because the T-Box should only include 'DeclaredOnly' properties,
            // add the Individual properties separately.
            var nonDeclaredProps = individualType.GetProperties().Except(properties);
            foreach (var p in nonDeclaredProps)
                if (p.IsObjectProperty(graphSettings.ABoxSettings))
                {
                    ObjectProperty objectProperty = new ObjectProperty() { DomainClass = p.DeclaringType, RangeClass = p.PropertyType, PropertyInfo = p };
                    AddIndividualObjectPropertyRelation(individual, p, graphSettings, objectProperty);
                }
                else if (p.IsDataProperty(graphSettings.ABoxSettings))
                {
                    DataProperty dataProperty = new DataProperty() { DomainClass = p.DeclaringType, RangeType = p.PropertyType, PropertyInfo = p };
                    AddIndividualDataPropertyRelation(individual, p, graphSettings, dataProperty);
                }
        }


        /***************************************************/
        // Private static fields
        /***************************************************/

        private static CSharpGraph m_cSharpGraph = new CSharpGraph();
    }
}
