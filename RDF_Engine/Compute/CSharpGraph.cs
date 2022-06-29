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
using BH.oM.RDF;
using BH.oM.Base.Attributes;
using BH.Engine.RDF.Types;
using System.Collections;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        [Description("Returns an ontology graph that includes CSharp objects and types." +
            "This methods takes in a list of objects as an input, so the resulting CSharp graph has both the T-Box and A-Box sections populated." +
            "If you only are interested in the T-Box, use the other CSharpGraph() method that takes in a list of Types.")]
        public static CSharpGraph CSharpGraph(this List<IObject> iObjects, OntologySettings ontologySettings)
        {
            m_cSharpGraph = new CSharpGraph() { OntologySettings = ontologySettings };

            foreach (var iObject in iObjects)
                AddIndividualToOntology(iObject, ontologySettings);

            return m_cSharpGraph;
        }

        /***************************************************/

        [Description("Returns an ontology graph that includes CSharp objects and types. " +
            "This methods takes in a list of Types as an input, so the resulting CSharp graph has only the T-Box." +
            "For a complete graph that also includes the A-Box, use the other CSharpGraph() method that takes in a list of objects.")]
        public static CSharpGraph CSharpGraph(this List<Type> types, OntologySettings ontologySettings)
        {
            m_cSharpGraph = new CSharpGraph() { OntologySettings = ontologySettings };

            foreach (var type in types)
                AddToOntology(type, ontologySettings.TBoxSettings);

            return m_cSharpGraph;
        }


        /***************************************************/
        // Private methods
        /***************************************************/

        private static void AddToOntology(this Type type, TBoxSettings tBoxSettings = null)
        {
            tBoxSettings = tBoxSettings ?? new TBoxSettings();

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

            if (type.IsOntologyClass())
                m_cSharpGraph.Classes.Add(type);

            List<Type> parentTypes = type.BaseTypes();
            foreach (var parentType in parentTypes)
            {
                if (!parentType.IsOntologyClass())
                    continue;

                AddToOntology(parentType);
            }
        }

        private static void AddToOntology(this PropertyInfo[] pInfos, OntologySettings ontologySettings, object obj = null)
        {
            foreach (var pi in pInfos)
            {
                if (pi is CustomPropertyInfo && obj != null)
                {
                    object individualPropertyValue = pi.GetValue(obj);
                    AddToOntology((CustomPropertyInfo)pi, ontologySettings, obj, individualPropertyValue);
                }
                else
                    AddToOntology(pi as dynamic, ontologySettings, obj);

            }
        }

        private static void AddToOntology(this CustomPropertyInfo customPI, OntologySettings ontologySettings, object individual, object individualPropertyValue)
        {
            if (customPI.Name == "Type")
                return; // do not add the `Type` property to the ontology for Custom Types.

            AddToOntology((PropertyInfo)customPI, ontologySettings, individual, individualPropertyValue);
        }

        private static void AddToOntology(this PropertyInfo pi, OntologySettings ontologySettings, object individual, object individualPropertyValue = null)
        {
            // In C#'s Reflection, relations are represented with PropertyInfos.
            // In an ontology, PropertyInfos may correspond to either ObjectProperties or DataProperties.

            Type domainType = pi.DeclaringType;
            Type rangeType = pi.PropertyType;

            if (!domainType.IsOntologyClass())
                return; // do not add Properties of classes that are not Ontology classes (e.g. if domainType is a String, we do not want to add its property Chars).
            else
                domainType.AddToOntology();

            if (pi.IsObjectProperty())
            {
                // OBJECT PROPERTY RELATION
                // The relation between Individuals corresponds to an ObjectPropertyRelation (between two Classes of the Ontology).

                if (pi.PropertyType.IsListOfOntologyClasses(individualPropertyValue) && individual != null)
                {
                    rangeType = new ListPropertyType(individual, pi, ontologySettings.TBoxSettings);

                    IList list = pi.GetValue(individual) as IList;

                    if (list != null && list.Count != 0)
                    {
                        foreach (var item in list)
                        {
                            m_cSharpGraph.AllIndividuals.Add(item);

                            //AddIndividualToOntology(item, ontologySettings);

                            // Recurse for this individual's relations.
                            PropertyInfo[] listItemProps = item?.GetType().GetProperties() ?? new PropertyInfo[] { };
                            AddToOntology(listItemProps, ontologySettings, item);
                        }
                    }
                }

                // Make sure the RangeType is added to the ontology.
                rangeType.AddToOntology();

                // Add the ObjectProperty to the Graph for the T-Box.
                ObjectProperty hasPropertyRelation = new ObjectProperty() { PropertyInfo = pi, DomainClass = domainType, RangeClass = rangeType };
                m_cSharpGraph.ObjectProperties.Add(hasPropertyRelation);

                // If the individual is non-null, we will need to add the individuals' relation to the Graph in order to define the A-Box.
                if (individual == null) return;
                object propertyValue = pi.CanRead ? pi.GetValue(individual) : null;
                if (!ontologySettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues && propertyValue.IsNullOrEmpty())
                    return;

                IndividualObjectProperty rel = new IndividualObjectProperty()
                {
                    HasProperty = hasPropertyRelation,
                    Individual = individual,
                    RangeIndividual = propertyValue
                };

                m_cSharpGraph.IndividualRelations.Add(rel);

                // Recurse for the individual's property value, which will be another individual.
                AddIndividualToOntology(propertyValue, ontologySettings);
                return;
            }

            if (pi.IsDataProperty())
            {
                // DATA PROPERTY RELATION
                // We do not have an Ontology class corresponding to the rangeType:
                // this PropertyInfo relation corresponds to a Data property.

                // Add the ObjectProperty to the Graph for the T-Box.
                DataProperty hasPropertyRelation = new DataProperty() { PropertyInfo = pi, DomainClass = domainType, RangeType = rangeType };
                m_cSharpGraph.DataProperties.Add(hasPropertyRelation);

                // If the individual is non-null, we will need to add the individuals' relation to the Graph in order to define the A-Box.
                if (individual == null) return;
                object propertyValue = pi.CanRead ? pi.GetValue(individual) : null;
                if (!ontologySettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues && propertyValue.IsNullOrEmpty())
                    return;

                IndividualDataProperty rel = new IndividualDataProperty()
                {
                    Individual = individual,
                    Value = propertyValue,
                    PropertyInfo = pi
                };

                m_cSharpGraph.IndividualRelations.Add(rel);
            }
        }

        private static void AddIndividualToOntology(object individual, OntologySettings ontologySettings)
        {
            Type individualType = individual.IndividualType(ontologySettings.TBoxSettings);
            ontologySettings = ontologySettings ?? new OntologySettings();

            // Only individuals that are of types mappable to Ontology classes can be added.
            if (individualType.IsOntologyClass())
            {
                if (!ontologySettings.ABoxSettings.ConsiderDefaultPropertyValues)
                    throw new NotImplementedException($"Feature {nameof(ABoxSettings)}.{nameof(ABoxSettings.ConsiderDefaultPropertyValues)} not yet implemented. Please set it to true.");

                // Make sure the individual type is among the ontology classes.
                individualType.AddToOntology();

                // Add the individual.
                m_cSharpGraph.AllIndividuals.Add(individual);
            }

            // Recurse for properties of this individual.
            PropertyInfo[] properties = individualType.GetProperties();
            properties.AddToOntology(ontologySettings, individual);
        }


        /***************************************************/
        // Private static fields
        /***************************************************/

        private static CSharpGraph m_cSharpGraph = new CSharpGraph();
    }
}
