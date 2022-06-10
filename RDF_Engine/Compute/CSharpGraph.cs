
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

            if (type == typeof(CustomType))
                return; // only add sub-types of CustomType.
            
            CustomType cType = type as CustomType;
            if (cType != null && m_cSharpGraph.Classes.Contains(type)) 
                throw new ArgumentException($"The input contained multiple CustomObjects with the same value `{cType.Name}` under the {nameof(TBoxSettings.CustomobjectsTypeKey)} `{tBoxSettings.CustomobjectsTypeKey}`. Only unique values for distinct Custom Objects are allowed.");

            if (m_cSharpGraph.Classes.Contains(type))
                return;

            if (type.IsCollectionOfOntologyClasses())
                type = type.InnermostType();

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

        private static void AddToOntology(this PropertyInfo[] pInfos, object obj = null, OntologySettings ontologySettings = null)
        {
            foreach (var pi in pInfos)
                AddToOntology(pi as dynamic, obj, ontologySettings);
        }

        private static void AddToOntology(this CustomPropertyInfo customPI, object individual = null, OntologySettings ontologySettings = null)
        {
            if (customPI.Name == "Type")
                return; // do not add the `Type` property to the ontology for Custom Types.

            AddToOntology((PropertyInfo)customPI, individual, ontologySettings);
        }

        private static void AddToOntology(this PropertyInfo pi, object individual = null, OntologySettings ontologySettings = null)
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

        private static void AddIndividualToOntology(object individual, OntologySettings ontologySettings = null)
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
            properties.AddToOntology(individual, ontologySettings);
        }


        /***************************************************/
        // Private static fields
        /***************************************************/

        private static CSharpGraph m_cSharpGraph = new CSharpGraph();
    }
}
