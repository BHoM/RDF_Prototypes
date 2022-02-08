
using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.RDF;
using Newtonsoft.Json.Linq;
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
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Convert a Graph ontological representation of (BHoM) types and their relations into a Json format readable by WebVOWL (http://vowl.visualdataweb.org/webvowl.html).")]
        public static string ToWebVOWLJson(Dictionary<Type, List<IRelation>> dictionaryGraph)
        {
            // A WebVOWL Json is so structured:
            // 0) Header and general info/settings
            // 1) "Class" section. A JArray with JObjects. Each JObject is a node in the graph. Each JObject must contain 2 JProperties: an "id" (any UID) and a "type" (the OWL/RDF type).
            // 2) "Class attribute" section. A JArray with JObjects. Each JObject is a node in the graph. Each JObject contains additional attributes to be assigned to each node in the graph, e.g. the URL and the node label.
            // 3) "Property" section. A JArray with JObjects. Each JObject is a link in the graph. Each JObject must contain 2 JProperties: an "id" (any UID) and a "type" (the OWL/RDF type).
            // 4) "Property attribute" section. A JArray with JObjects. Each JObject is a link in the graph. Each JObject contains additional attributes to be assigned to each link in the graph, e.g. the URL and the link label.

            // 0) HEADER AND GENERAL INFO/SETTINGS 
            JObject rdfJsonObject = new JObject();
            rdfJsonObject.Add(new JProperty("comment", "Generated from BHoM RDF_Prototypes"));
            rdfJsonObject.Add(new JProperty("namespace", new JArray()));

            JArray classArray = new JArray();
            JArray classAttributeArray = new JArray();

            // Iterate the Types in the dictionary. Each type will be a node in the graph.
            foreach (Type type in dictionaryGraph.Keys)
            {
                string typeId = type.FullName.OnlyAlphabeticAndDots();

                // 1) CLASS
                classArray.AddToIdTypeArray(typeId, "owl:Class");

                // 2) CLASS ATTRIBUTE
                classAttributeArray.AddToAttributeArray(typeId, type.UriFromType(), type.Name.OnlyAlphabeticAndDots() + $" ({type.FullName.OnlyAlphabeticAndDots()})");
            }

            // There could be Types that were not gathered yet and that could emerge from the targets of the Relations.
            HashSet<Type> addedTypes = new HashSet<Type>(dictionaryGraph.Keys);

            JArray propertyArray = new JArray();
            JArray propertyAttributeArray = new JArray();

            // Iterate the IRelations in the dictionary. Each IRelation will be a link in the graph.
            List<IRelation> allRelations = dictionaryGraph.Values.SelectMany(v => v).ToList();
            foreach (IRelation relation in allRelations)
            {
                string relationId = Guid.NewGuid().ToString();
                Type relationType = relation.GetType();

                // // - Domain and range class and class attributes
                // We check these first because it is useful to set the 3) PROPERTY section, see later.
                List<string> subjectPropertyAttribute = new List<string>(); // unused
                List<string> objectPropertyAttribute = new List<string>(); // this is the determinant in the relationship's attributes

                string subjectNodeId = PopulateClassNodes(classArray, classAttributeArray, addedTypes, relation, relationId, relation.Subject, out subjectPropertyAttribute);
                string objectNodeId = PopulateClassNodes(classArray, classAttributeArray, addedTypes, relation, relationId, relation.Object, out objectPropertyAttribute);

                // 3) PROPERTY
                // To determine what kind of relation we want to set, we check the type of the RDF "Object" (or "range").
                if (((relation.Object as PropertyInfo)?.PropertyType ?? relation.Object as Type)?.IsPrimitive ?? false)
                    propertyArray.AddToIdTypeArray(relationId, "owl:DatatypeProperty");
                else
                    propertyArray.AddToIdTypeArray(relationId, "owl:ObjectProperty");

                // 4) PROPERTY ATTRIBUTE
                propertyAttributeArray.AddToAttributeArray(relationId, $"http://visualdataweb.org/newOntology/{relationId}", relationType.Name, null, objectPropertyAttribute, new List<string>() { subjectNodeId }, new List<string>() { objectNodeId });
            }

            rdfJsonObject.Add(new JProperty("class", classArray));
            rdfJsonObject.Add(new JProperty("classAttribute", classAttributeArray));
            rdfJsonObject.Add(new JProperty("property", propertyArray));
            rdfJsonObject.Add(new JProperty("propertyAttribute", propertyAttributeArray));

            return rdfJsonObject.ToString();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string PopulateClassNodes(JArray classArray, JArray classAttributeArray, HashSet<Type> addedTypes, IRelation relation, string relationId, object subjectOrObject, out List<string> propertyAttributes)
        {
            propertyAttributes = null;

            string subjectOrObjectNodeId;

            if (subjectOrObject == null)
                throw new ArgumentException("Subject type cannot be null.");

            string propertyName = (subjectOrObject as PropertyInfo)?.Name ?? "";
            Type subjectOrObjectType = (subjectOrObject as PropertyInfo)?.PropertyType ?? subjectOrObject as Type;

            string label = $"{propertyName} ({subjectOrObjectType.Name})";

            if (subjectOrObjectType.IsBHoMType())
            {
                subjectOrObjectNodeId = subjectOrObjectType.FullName.OnlyAlphabeticAndDots();

                if (!addedTypes.Contains(subjectOrObjectType))
                {
                    classArray.AddToIdTypeArray(subjectOrObjectNodeId, "owl:Class");

                    classAttributeArray.AddToAttributeArray(subjectOrObjectNodeId, subjectOrObjectType.UriFromType(), label);
                }

                propertyAttributes = new List<string>() { "object" };
            }
            else //if (subjectOrObject is string || subjectOrObjectType.IsNumeric()) // Un-comment if we want to create `Literal` nodes for non-primitive, non-BHoM types.
            {
                subjectOrObjectNodeId = relation.WebVOWLNodeId();

                classArray.AddToIdTypeArray(subjectOrObjectNodeId, "rdfs:Datatype");

                classAttributeArray.AddToAttributeArray(subjectOrObjectNodeId, @"http://www.w3.org/2001/XMLSchema#", label, null, new List<string>() { "datatype" });

                propertyAttributes = new List<string>() { "datatype" };
            }
            // Un-comment if we want to create `Literal` nodes for non-primitive, non-BHoM types.
            //else
            //{
            //    subjectOrObjectNodeId = relation.WebVOWLNodeId();

            //    classArray.AddToIdTypeArray(subjectOrObjectNodeId, "rdfs:Literal");

            //    classAttributeArray.AddToAttributeArray(subjectOrObjectNodeId, @"http://www.w3.org/2001/XMLSchema#", label);

            //    // literal node has no property attribute
            //}

            return subjectOrObjectNodeId;
        }

        /***************************************************/

        private static string WebVOWLNodeId(this IRelation relation)
        {
            return $"{(relation.Object as dynamic)?.Name ?? ""}-{Guid.NewGuid().ToString()}-{((Type)relation.Subject).Name}-{relation.GetType().Name}";
        }

        /***************************************************/

        public static void AddToIdTypeArray(this JArray idTypeArray, string id, string type)
        {
            // CLASS
            JObject classObj = new JObject();
            classObj.Add(new JProperty("id", id));
            classObj.Add(new JProperty("type", type));

            idTypeArray.Add(classObj);
        }

        /***************************************************/

        private static void AddToAttributeArray(this JArray attributeArray, string id, string iri, string label_en, string label_iriBased = null,
            List<string> attributes = null,
            List<string> domain = null, List<string> range = null)
        {
            JObject attributeArrayObj = new JObject();
            attributeArrayObj.Add(new JProperty("id", id));
            attributeArrayObj.Add(new JProperty("iri", iri));

            // // - Label
            if (label_iriBased != null)
            {
                JObject labelObj = new JObject();

                labelObj.Add(new JProperty("IRI-based", label_iriBased)); // not sure what this is for
                labelObj.Add(new JProperty("en", label_en)); //  type.Name + $" ({type.Namespace})"

                attributeArrayObj.Add(new JProperty("label", labelObj));
            }
            else
                attributeArrayObj.Add(new JProperty("label", label_en));

            // // - Attributes
            attributeArrayObj.Add(attributes.ToJProperty("attributes"));

            // // - Domain
            attributeArrayObj.Add(domain.ToJProperty("domain"));

            // // - Range
            attributeArrayObj.Add(range.ToJProperty("range"));

            attributeArray.Add(attributeArrayObj);
        }

        /***************************************************/

        public static JProperty ToJProperty(this List<string> list, string propertyName)
        {
            if (list == null)
                return new JProperty(propertyName, new JArray());

            if (list.Count() == 1)
                return new JProperty(propertyName, list.FirstOrDefault());

            return new JProperty(propertyName, list.ToJArray());
        }

        /***************************************************/

        public static JArray ToJArray(this List<string> list)
        {
            if (list == null)
                return new JArray();

            JArray arr = new JArray();
            list.ForEach(d => arr.Add(d));

            return arr;
        }
    }
}
