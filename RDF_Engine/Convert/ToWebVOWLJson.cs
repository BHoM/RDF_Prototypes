
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
using log = BH.oM.RDF.Log;

namespace BH.Engine.RDF
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Convert a Graph ontological representation of (BHoM) types and their relations into a Json format readable by WebVOWL (http://vowl.visualdataweb.org/webvowl.html).")]
        public static string ToWebVOWLJson(Dictionary<TypeInfo, List<IRelation>> dictionaryGraph, LocalRepositorySettings settings, HashSet<string> internalNamespaces = null, HashSet<string> exceptions = null, int relationRecursion = 0)
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
            HashSet<string> addedWebVOWLNodeIds = new HashSet<string>(); // There could be Types that were not gathered yet and that could emerge from the targets of the Relations.

            foreach (TypeInfo type in dictionaryGraph.Keys)
                AddWebOwlClassNodes(type, classArray, classAttributeArray, addedWebVOWLNodeIds, settings);

            JArray propertyArray = new JArray();
            JArray propertyAttributeArray = new JArray();

            // Iterate the IRelations in the dictionary. Each IRelation will be a link in the graph.
            List<IRelation> allRelations = dictionaryGraph.Values.SelectMany(v => v).ToList();
            foreach (IRelation relation in allRelations)
            {
                AddWebOwlRelationNodes(relation as dynamic, classArray, classAttributeArray, addedWebVOWLNodeIds, settings: settings, propertyArray: propertyArray, propertyAttributeArray: propertyAttributeArray, internalNamespaces: internalNamespaces, exceptions: exceptions, recursionLevel: relationRecursion);
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

        //private static void AddRelationObjectNodes(PropertyInfo relationObject, IRelation relation,
        //                                             JArray classArray,
        //                                             JArray classAttributeArray, HashSet<TypeInfo> addedWebVOWLNodeIds,
        //                                             JArray propertyArray = null,
        //                                             JArray propertyAttributeArray = null,
        //                                             HashSet<string> internalNamespaces = null)
        //{
        //    TypeInfo relationObjectType = (relation.Object as PropertyInfo)?.PropertyType.GetTypeInfo() ?? relation.Object as TypeInfo;

        //    if (relationObjectType.Name.StartsWith("<>c__"))
        //        return null;

        //    string subjectOrObjectNodeId = relationObjectType.FullNameValidChars();

        //    //Type subjectOrObjectType = (subjectOrObject as PropertyInfo)?.PropertyType ?? subjectOrObject as Type;
        //    //string label = (subjectOrObject as PropertyInfo)?.DescriptiveName() ?? $"{subjectOrObjectType.NameValidChars()} ({subjectOrObjectType.FullNameValidChars()})";

        //    if (relationObjectType.Name == "Ceiling")
        //        subjectOrObjectNodeId = subjectOrObjectNodeId;

        //    if (relationObjectType.IsBHoMType() || relationObjectType.IsGenericTypeWithBHoMArgs())
        //    {
        //        // If the Type is a BHoMType, the node id is the type full name.

        //        if (relation is HasProperty && subjectOrObject is PropertyInfo)
        //        {
        //            PropertyInfo pInfo = (PropertyInfo)subjectOrObject;

        //            if (pInfo.Name == "Tiles")
        //                subjectOrObjectNodeId = subjectOrObjectNodeId;

        //            return AddPropertyNodes(pInfo, classArray, classAttributeArray, addedWebVOWLNodeIds, propertyArray, propertyAttributeArray, internalNamespaces);
        //        }
        //        else if (!addedWebVOWLNodeIds.Contains(relationObjectType))
        //        {
        //            string label = subjectOrObject.DescriptiveName();

        //            classArray.AddToIdTypeArray(subjectOrObjectNodeId, "owl:Class");

        //            classAttributeArray.AddToAttributeArray(subjectOrObjectNodeId, relationObjectType.GithubURI(settings), label);

        //            addedWebVOWLNodeIds.Add(relationObjectType);
        //        }

        //        propertyAttributes = new List<string>() { "object" };

        //        return subjectOrObjectNodeId;
        //    }
        //    else if (!(relation is HasProperty))
        //    {
        //        // If the subject or Object type is not a BHoM type, and the Relation is not a HasProperty relation,
        //        // we do not want to be adding another class node.
        //        // This is because we do not want to make ontological class nodes for C# classes, like Dictionary, IEnumerable etc.
        //        return "";
        //    }

        //    // if (subjectOrObject is string || subjectOrObjectType.IsNumeric()) { // Un-comment if we want to create `Literal` nodes for non-primitive, non-BHoM types.

        //    subjectOrObjectNodeId = relation.WebVOWLNodeId();

        //    classArray.AddToIdTypeArray(subjectOrObjectNodeId, "rdfs:Datatype");

        //    classAttributeArray.AddToAttributeArray(subjectOrObjectNodeId, relation.GetType().GithubURI(settings), subjectOrObject.DescriptiveName(), false, new List<string>() { "datatype" });

        //    propertyAttributes = new List<string>() { "datatype" };
        //    // Un-comment if we want to create `Literal` nodes for non-primitive, non-BHoM types.

        //    // }
        //    //else
        //    //{
        //    //    subjectOrObjectNodeId = relation.WebVOWLNodeId();

        //    //    classArray.AddToIdTypeArray(subjectOrObjectNodeId, "rdfs:Literal");

        //    //    classAttributeArray.AddToAttributeArray(subjectOrObjectNodeId, @"http://www.w3.org/2001/XMLSchema#", label);

        //    //    // literal node has no property attribute
        //    //}

        //    // 3) PROPERTY
        //    // To determine what kind of relation we want to set, we check the type of the RDF "Object" (or "range").
        //    // If the "Object" is a BHoM type, we can consider it an "owl:ObjectProperty", otherwise we consider it a "owl:DatatypeProperty".
        //    bool isDatatypeProperty = !((relation.Object as PropertyInfo)?.PropertyType ?? relation.Object as Type)?.InnermostType().IsBHoMType() ?? true;
        //    if (isDatatypeProperty)
        //        propertyArray.AddToIdTypeArray(relationId, "owl:DatatypeProperty");
        //    else
        //        propertyArray.AddToIdTypeArray(relationId, "owl:ObjectProperty");

        //    // 4) PROPERTY ATTRIBUTE
        //    propertyAttributeArray.AddToAttributeArray(relationId, relation.GetType().GithubURI(settings), relation.GetType().Name, false, objectPropertyAttribute, new List<string>() { relation.Subject }, new List<string>() { objectNodeId });
        //}

       

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
