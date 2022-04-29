
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
using log = BH.oM.RDF.Log;
using BH.Engine.Base;
using BH.oM.RDF;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        public static void TTLOntology(this IObject iObject, TBoxSettings tBoxSettings, ABoxSettings aBoxSettings, LocalRepositorySettings localRepoSettings)
        {
            // Iterate objects and build ontology.

            iObject.AddIndividualToOntology(iObject.GetType(), aBoxSettings);

            //Dictionary<PropertyInfo, object> declaredPropValues = iObjType.CollectDeclaredPropertyValues(iObject);

            //foreach (KeyValuePair<PropertyInfo, object> individual in declaredPropValues)
            //    individual.Value.AddIndividualToOntology(aBoxSettings);

            // COMPOSE ONTOLOGY
            // TBox
            string tBoxOntology = Query.TTLHeader(tBoxSettings.OntologyTitle, tBoxSettings.OntologyDescription, tBoxSettings.OntologyBaseAddress);

            tBoxOntology = tBoxOntology.AddTTLSectionTitle("Annotation Properties");
            tBoxOntology += string.Join("\n", GetAnnotationProperties());

            tBoxOntology = tBoxOntology.AddTTLSectionTitle("Datatypes");
            tBoxOntology += string.Join("\n", GetDataTypes(tBoxSettings));

            m_output_TBoxTTLOntology = tBoxOntology;

            // ABox
            string aBoxOntology = Query.TTLHeader(aBoxSettings.OntologyTitle, aBoxSettings.OntologyDescription, aBoxSettings.OntologyBaseAddress);

            aBoxOntology = tBoxOntology.AddTTLSectionTitle("Annotation Properties");
            aBoxOntology += string.Join("\n", GetAnnotationProperties());

            aBoxOntology = tBoxOntology.AddTTLSectionTitle("Datatypes");
            aBoxOntology += string.Join("\n", GetDataTypes(aBoxSettings));

            m_output_ABoxTTLOntology = aBoxOntology;
        }

        /***************************************************/
        // Private methods
        /***************************************************/

        private static void AddToOntology(this Type type)
        {
            if (m_cSharpOntology.Classes.Contains(type))
                return;

            if (type.IsOntologyClass())
                m_cSharpOntology.Classes.Add(type);

            List<Type> parentTypes = type.ParentTypes();
            foreach (var parentType in parentTypes)
            {
                AddToOntology(parentType);
            }
        }

        private static void AddToOntology(this PropertyInfo[] pInfos, object obj, ABoxSettings aBoxSettings = null)
        {
            foreach (var pi in pInfos)
                AddToOntology(pi, obj, aBoxSettings);
        }

        private static void AddToOntology(this PropertyInfo pi, object obj, ABoxSettings aBoxSettings = null)
        {
            // In C#'s Reflection, relations are represented with PropertyInfos.
            // PropertyInfos may correspond to either ObjectProperties or DataProperties.

            Type domainType = pi.DeclaringType;
            Type rangeType = pi.PropertyType;

            // Make sure the Property Type and Declaring Type are present in the ontology.
            domainType.AddToOntology();
            rangeType.AddToOntology();

            // Add the propertyInfo to Ontology.
            // Check if this PropertyInfo is to be represented as an ObjectProperty or a DataProperty.

            object propertyValue = pi.CanRead ? pi.GetValue(obj) : null;

            if (propertyValue != null)
                AddIndividualToOntology(propertyValue, pi.PropertyType, aBoxSettings);

            IndividualRelation cSharpRelation = new IndividualRelation() {
                PropertyInfo = pi,
                DomainIndividual = obj,
                RangeIndividual = propertyValue
            };

            m_cSharpOntology.IndividualRelations.Add(cSharpRelation);
        }

        private static void AddIndividualToOntology(this object individual, Type individualType = null, ABoxSettings aBoxSettings = null)
        {
            individualType = individualType ?? individual.GetType();
            aBoxSettings = aBoxSettings ?? new ABoxSettings();

            // Only individuals that are of types mappable to Ontology classes can be added.
            if (individualType.IsOntologyClass())
            {
                if (!aBoxSettings.AddDefaultPropertyValues)
                    throw new NotImplementedException($"Feature {nameof(ABoxSettings)}.{nameof(aBoxSettings.AddDefaultPropertyValues)} not yet implemented. Please set it to true.");

                // Make sure the individual type is among the ontology classes.
                individualType.AddToOntology();

                // Add the individual.
                m_cSharpOntology.AllIndividuals.Add(individual);
            }

            // Recurse for properties of this individual.
            PropertyInfo[] properties = individualType.GetProperties();
            properties.AddToOntology(individual, aBoxSettings);
        }

        private static Dictionary<Type, PropertyInfo[]> m_TypeProperties = new Dictionary<Type, PropertyInfo[]>();

        private static PropertyInfo[] CollectDeclaredProperties(this Type type)
        {
            PropertyInfo[] typeProperties;

            if (!m_TypeProperties.TryGetValue(type, out typeProperties))
            {
                typeProperties = type.DeclaredProperties();
                m_TypeProperties[type] = typeProperties;
            }

            return typeProperties;
        }

        private static Dictionary<PropertyInfo, object> CollectDeclaredPropertyValues(this Type type, object obj, ABoxSettings aBoxSettings = null)
        {
            if (type == null || obj == null)
                return new Dictionary<PropertyInfo, object>();

            PropertyInfo[] typeProperties = type.CollectDeclaredProperties();

            Dictionary<PropertyInfo, object> propertyValues = new Dictionary<PropertyInfo, object>();

            foreach (PropertyInfo prop in typeProperties)
            {
                if (obj != null)
                {
                    if (!prop.CanRead || prop.GetMethod.GetParameters().Count() > 0) continue;

                    // TODO: add mechanism for checking whether property has default value or not.
                    object propValue = prop.GetValue(obj);
                    propertyValues[prop] = propValue;
                }
            }

            return propertyValues;
        }

        private static List<string> GetAnnotationProperties()
        {
            List<string> annotationProperties = new List<string>();

            annotationProperties.Add("###  http://purl.org/dc/elements/1.1/#description\n<http://purl.org/dc/elements/1.1/#description> rdf:type owl:AnnotationProperty .");
            annotationProperties.Add("###  http://purl.org/dc/elements/1.1/#title\n<http://purl.org/dc/elements/1.1/#title> rdf:type owl:AnnotationProperty .");

            return annotationProperties;
        }

        private static List<string> GetDataTypes(IOntologySettings ontologySettings)
        {
            List<string> dataTypes = new List<string>();
            dataTypes.Add($"###  {ontologySettings.OntologyBaseAddress}/serializedJson\n<{ontologySettings.OntologyBaseAddress}/serializedJson> rdf:type rdfs:Datatype ;\nowl:equivalentClass xsd:string .");

            return dataTypes;
        }

        public static string GetDataProperties(this Type type, IOntologySettings ontologySettings)
        {
            string dataProperties = "";

            PropertyInfo[] pInfos = type.DeclaredProperties();
            string ownerTypeName = type.Name;

            // Collect all properties that can be translated to a data property, e.g. string types.
            foreach (var pInfo in pInfos)
            {
                string propertyName = pInfo.Name;
                string rangeType = "";

                // Dispatch to the appropriate type.
                // TODO: add more types.
                if (pInfo.PropertyType == typeof(string))
                    rangeType = "xsd:string";
                else
                    throw new Exception("TODO: add support for more DataType property conversions.");

                dataProperties += $@"\n
                                ###  {ontologySettings.OntologyBaseAddress}/{propertyName}
                                :{propertyName} rdf:type owl:DatatypeProperty ;
                                rdfs:domain :{ownerTypeName} ;
                                rdfs:range {rangeType} ;
                                rdfs:label ""{propertyName}""@en ,
                                           ""{propertyName}""@iri - based.";
            }

            return dataProperties;
        }

        /***************************************************/
        // Private static fields
        /***************************************************/

        private static CSharpOntology m_cSharpOntology = new CSharpOntology();
        private static TBoxSettings m_TBoxSettings = new TBoxSettings();
        private static ABoxSettings m_ABoxSettings = new ABoxSettings();

        private static string m_output_TBoxTTLOntology;
        private static string m_output_ABoxTTLOntology;
    }
}
