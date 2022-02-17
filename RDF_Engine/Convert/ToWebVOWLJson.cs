
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
        public static string ToWebVOWLJson(Dictionary<TypeInfo, List<IRelation>> dictionaryGraph, HashSet<string> internalNamespaces = null)
        {
            if (dictionaryGraph == null)
                return null;

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
            HashSet<TypeInfo> addedTypes = new HashSet<TypeInfo>(); // There could be Types that were not gathered yet and that could emerge from the targets of the Relations.
            foreach (TypeInfo type in dictionaryGraph.Keys)
            {
                string typeId = type.FullNameValidChars();
                Uri typeUri = type.GithubURI();

                if (typeUri == null)
                    continue;

                // 1) CLASS
                classArray.AddToIdTypeArray(typeId, "owl:Class");

                // 2) CLASS ATTRIBUTE
                classAttributeArray.AddToAttributeArray(typeId, typeUri, type.DescriptiveName(true));

                addedTypes.Add(type);
            }

            JArray propertyArray = new JArray();
            JArray propertyAttributeArray = new JArray();

            // Iterate the IRelations in the dictionary. Each IRelation will be a link in the graph.
            List<IRelation> allRelations = dictionaryGraph.Values.SelectMany(v => v).ToList();
            foreach (IRelation relation in allRelations)
            {
                string relationId = $"{relation.Subject}-{relation.GetType().FullName}-{relation.Object}";
                Type relationType = relation.GetType();

                // // - Domain and range class and class attributes
                // We check these first because it is useful to set the 3) PROPERTY section, see later.
                List<string> subjectPropertyAttribute = new List<string>(); // unused
                List<string> objectPropertyAttribute = new List<string>(); // this is the determinant in the relationship's attributes

                Type subjectType = (relation.Subject as PropertyInfo)?.PropertyType ?? relation.Subject as Type;

                string subjectNodeId = PopulateClassNodesFromRelation(relation.Subject, relation, classArray, classAttributeArray, addedTypes, out subjectPropertyAttribute, null, null, internalNamespaces);
                string objectNodeId = PopulateClassNodesFromRelation(relation.Object, relation, classArray, classAttributeArray, addedTypes, out objectPropertyAttribute, propertyArray, propertyAttributeArray, internalNamespaces);

                // 3) PROPERTY
                // To determine what kind of relation we want to set, we check the type of the RDF "Object" (or "range").
                // If the "Object" is a BHoM type, we can consider it an "owl:ObjectProperty", otherwise we consider it a "owl:DatatypeProperty".
                bool isDatatypeProperty = !((relation.Object as PropertyInfo)?.PropertyType ?? relation.Object as Type)?.InnermostType().IsBHoMType() ?? true;
                if (isDatatypeProperty)
                    propertyArray.AddToIdTypeArray(relationId, "owl:DatatypeProperty");
                else
                    propertyArray.AddToIdTypeArray(relationId, "owl:ObjectProperty");

                // 4) PROPERTY ATTRIBUTE
                propertyAttributeArray.AddToAttributeArray(relationId, relation.GetType().GithubURI(), relationType.Name, false, objectPropertyAttribute, new List<string>() { subjectNodeId }, new List<string>() { objectNodeId });
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

        private static string PopulateClassNodesFromRelation(object subjectOrObject, IRelation relation,
                                                             JArray classArray,
                                                             JArray classAttributeArray, HashSet<TypeInfo> addedTypes,
                                                             out List<string> propertyAttributes,
                                                             JArray propertyArray = null,
                                                             JArray propertyAttributeArray = null,
                                                             HashSet<string> internalNamespaces = null)
        {
            propertyAttributes = null;

            if (subjectOrObject == null)
                throw new ArgumentException("Subject type cannot be null.");

            string propertyName = (subjectOrObject as PropertyInfo)?.Name ?? "";
            TypeInfo subjectOrObjectType = (subjectOrObject as PropertyInfo)?.PropertyType.GetTypeInfo() ?? subjectOrObject as TypeInfo;

            if (subjectOrObjectType.Name.StartsWith("<>c__"))
                return null;

            string subjectOrObjectNodeId = subjectOrObjectType.FullNameValidChars();

            //Type subjectOrObjectType = (subjectOrObject as PropertyInfo)?.PropertyType ?? subjectOrObject as Type;
            //string label = (subjectOrObject as PropertyInfo)?.DescriptiveName() ?? $"{subjectOrObjectType.NameValidChars()} ({subjectOrObjectType.FullNameValidChars()})";

            if (subjectOrObjectType.IsBHoMType())
            {
                // If the Type is a BHoMType, the node id is the type full name.

                if (relation is HasProperty && subjectOrObject is PropertyInfo)
                {
                    PropertyInfo pInfo = (PropertyInfo)subjectOrObject;

                    // We need to add a node for the property.
                    string propertyNodeId = $"{pInfo.DeclaringType.FullName}.{pInfo.Name}";

                    classArray.AddToIdTypeArray(propertyNodeId, "owl:Class");

                    classAttributeArray.AddToAttributeArray(propertyNodeId, pInfo.GithubURI(), pInfo.DescriptiveName());

                    AddPropertyNodes(classArray, classAttributeArray, addedTypes, propertyArray, propertyAttributeArray, pInfo, propertyNodeId, internalNamespaces);

                    return propertyNodeId;
                }
                else if (!addedTypes.Contains(subjectOrObjectType))
                {
                    string label = subjectOrObject.DescriptiveName();

                    classArray.AddToIdTypeArray(subjectOrObjectNodeId, "owl:Class");

                    classAttributeArray.AddToAttributeArray(subjectOrObjectNodeId, subjectOrObjectType.GithubURI(), label);

                    addedTypes.Add(subjectOrObjectType);
                }

                propertyAttributes = new List<string>() { "object" };

                return subjectOrObjectNodeId;
            }
            else if (!(relation is HasProperty))
            {
                // If the subject or Object type is not a BHoM type, and the Relation is not a HasProperty relation,
                // we do not want to be adding another class node.
                // This is because we do not want to make ontological class nodes for C# classes, like Dictionary, IEnumerable etc.
                return "";
            }

            // if (subjectOrObject is string || subjectOrObjectType.IsNumeric()) { // Un-comment if we want to create `Literal` nodes for non-primitive, non-BHoM types.

            subjectOrObjectNodeId = relation.WebVOWLNodeId();

            classArray.AddToIdTypeArray(subjectOrObjectNodeId, "rdfs:Datatype");

            classAttributeArray.AddToAttributeArray(subjectOrObjectNodeId, relation.GetType().GithubURI(), subjectOrObject.DescriptiveName(), attributes: new List<string>() { "datatype" }, label_iriBased: null);

            propertyAttributes = new List<string>() { "datatype" };
            // Un-comment if we want to create `Literal` nodes for non-primitive, non-BHoM types.

            // }
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

        private static void AddPropertyNodes(JArray classArray, JArray classAttributeArray, HashSet<TypeInfo> addedTypes, JArray propertyArray, JArray propertyAttributeArray, PropertyInfo pInfo, string propertyNodeId, HashSet<string> internalNamespaces = null)
        {
            // If the property is a BHoM type, We need to add a "IsA" relation to a node that represents its property type.
            if (pInfo.PropertyType.IsBHoMType() && propertyArray != null && propertyAttributeArray != null)
            {
                string propertyTypeNodeId = pInfo.PropertyType.FullNameValidChars();
                // See if we have yet to add a Node for the type of this property.
                if (!addedTypes.Contains(pInfo.PropertyType))
                {
                    // We need to add the type of this property as a node.
                    classArray.AddToIdTypeArray(propertyTypeNodeId, "owl:Class");

                    classAttributeArray.AddToAttributeArray(propertyTypeNodeId, pInfo.PropertyType.GithubURI(), pInfo.PropertyType.DescriptiveName(true), !pInfo.PropertyType.IsInNamespace(internalNamespaces) ?? false);

                    addedTypes.Add(pInfo.PropertyType.GetTypeInfo());
                }

                // Add the "IsA" relation to link this property to the corresponding type node.
                string propertyTypeRelationId = $"{propertyNodeId}-IsA-{propertyTypeNodeId}";
                propertyArray.AddToIdTypeArray(propertyTypeRelationId, "owl:ObjectProperty");
                propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, new Uri(typeof(IsA).GithubURI().ToString(), UriKind.Absolute), typeof(IsA).Name, attributes: new List<string>() { "object" }, domain: new List<string>() { propertyNodeId }, range: new List<string>() { propertyTypeNodeId }, label_iriBased: null);
            }
        }

        /***************************************************/

        private static string WebVOWLNodeId(this IRelation relation)
        {
            return $"{(relation.Object as dynamic)?.Name ?? ""}-{relation.Hash()}-{((Type)relation.Subject).Name}-{relation.GetType().Name}";
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

        private static void AddToAttributeArray(this JArray attributeArray, string id, Uri uri, string label_en, bool isExternal = false,
            List<string> attributes = null,
            List<string> domain = null, List<string> range = null,
            string label_iriBased = null)
        {
            JObject attributeArrayObj = new JObject();
            attributeArrayObj.Add(new JProperty("id", id));
            attributeArrayObj.Add(new JProperty("iri", uri));

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
            if (isExternal)
                attributes = attributes == null ? new List<string> { "external" } : attributes.Concat(new List<string> { "external" }).ToList();

            attributeArrayObj.Add(new JProperty("attributes", attributes.ToJArray()));


            // // - Domain and range
            domain = domain?.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            range = range?.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            if ((domain?.Any() ?? false) && (range?.Any() ?? false))
            {
                attributeArrayObj.Add(domain.ToJProperty("domain"));
                attributeArrayObj.Add(range.ToJProperty("range"));
            }

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
