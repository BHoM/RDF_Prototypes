
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
        private static void AddWebOwlRelationNodes(IsSubclassOf isSubclassOf,
                                                    JArray classArray,
                                                    JArray classAttributeArray, HashSet<Type> addedTypes,
                                                    JArray propertyArray = null,
                                                    JArray propertyAttributeArray = null,
                                                    HashSet<string> internalNamespaces = null,
                                                    HashSet<string> exceptions = null)
        {
            Type domainType = isSubclassOf.Subject as Type;
            Type rangeType = isSubclassOf.Object as Type;

            // Filter using input exceptions.
            if (exceptions?.Contains(rangeType.FullName) ?? false)
                return;

            // Filter using default exceptions. These apply only when the domainType namespace is not "BH.oM.Base". Useful to remove uninteresting relations.
            if ((!domainType?.Namespace.StartsWith("BH.oM.Base") ?? false) && (rangeType.Name == "BHoMObject"))
                return;

            if (!rangeType.IsBHoMType())
                return;

            if (domainType == null || rangeType == null)
            {
                log.RecordError($"Cannot add IsA relation `{isSubclassOf.WebVOWLNodeId()}`");
                return;
            }

            string propertyTypeNodeId = rangeType.FullNameValidChars();

            // See if we have yet to add a Node for the Relation.Object type.
            if (!addedTypes.Contains(rangeType))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(propertyTypeNodeId, "owl:Class");

                classAttributeArray.AddToAttributeArray(propertyTypeNodeId, rangeType.GithubURI(), rangeType.DescriptiveName(true), !rangeType.IsInNamespace(internalNamespaces) ?? false);

                addedTypes.Add(rangeType.GetTypeInfo());
            }

            // Add the "IsSubclassOf" relation to link this property to the corresponding type node.
            string propertyTypeRelationId = isSubclassOf.WebVOWLNodeId();
            propertyArray.AddToIdTypeArray(propertyTypeRelationId, "rdfs:subClassOf");
            propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, new Uri(typeof(IsSubclassOf).GithubURI().ToString(), UriKind.Absolute), typeof(IsA).Name, attributes: new List<string>() { "object" }, domain: new List<string>() { domainType.WebVOWLNodeId() }, range: new List<string>() { rangeType.WebVOWLNodeId() }, label_iriBased: null);
        }

        /***************************************************/

        private static void AddWebOwlRelationNodes(IsAListOf isAListOfRelation,
                                                    JArray classArray,
                                                    JArray classAttributeArray, HashSet<Type> addedTypes,
                                                    JArray propertyArray = null,
                                                    JArray propertyAttributeArray = null,
                                                    HashSet<string> internalNamespaces = null,
                                                    HashSet<string> exceptions = null)
        {
            Type subjectType = isAListOfRelation.Object as Type;
            Type objectType = isAListOfRelation.Object as Type;

            if (exceptions?.Contains(objectType.FullName) ?? false)
                return;

            if (subjectType == null || objectType == null)
            {
                log.RecordError($"Cannot add IsAListOf relation `{isAListOfRelation.WebVOWLNodeId()}`");
                return;
            }

            string propertyTypeNodeId = objectType.FullNameValidChars();

            // See if we have yet to add a Node for the Relation.Object type.
            if (!addedTypes.Contains(objectType))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(propertyTypeNodeId, "owl:Class");

                classAttributeArray.AddToAttributeArray(propertyTypeNodeId, objectType.GithubURI(), objectType.DescriptiveName(true), !objectType.IsInNamespace(internalNamespaces) ?? false);

                addedTypes.Add(objectType.GetTypeInfo());
            }

            // Add the "IsAListOf" relation to link this property to the corresponding type node.
            string propertyTypeRelationId = isAListOfRelation.WebVOWLNodeId();
            propertyArray.AddToIdTypeArray(propertyTypeRelationId, "owl:ObjectProperty");
            propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, new Uri(typeof(IsAListOf).GithubURI().ToString(), UriKind.Absolute), typeof(IsAListOf).Name, attributes: new List<string>() { "object" }, domain: new List<string>() { subjectType.WebVOWLNodeId() }, range: new List<string>() { objectType.WebVOWLNodeId() }, label_iriBased: null);
        }

        /***************************************************/

        private static void AddWebOwlRelationNodes(IsA isARelation,
                                                    JArray classArray,
                                                    JArray classAttributeArray, HashSet<Type> addedTypes,
                                                    JArray propertyArray = null,
                                                    JArray propertyAttributeArray = null,
                                                    HashSet<string> internalNamespaces = null,
                                                    HashSet<string> exceptions = null)
        {
            Type domainType = isARelation.Subject as Type;
            Type rangeType = isARelation.Object as Type;

            // Filter specific exceptions
            if (exceptions?.Contains(rangeType.FullNameValidChars()) ?? false)
                return;

            // Filter using default exceptions. These apply only when the domainType namespace is not "BH.oM.Base". Useful to remove uninteresting relations.
            if ((!domainType?.Namespace.StartsWith("BH.oM.Base") ?? false) && (rangeType.Name == "IObject" || rangeType.Name == "IBHoMObject"))
                return;

            if (domainType == null || rangeType == null)
            {
                log.RecordError($"Cannot add IsA relation `{isARelation.WebVOWLNodeId()}`");
                return;
            }

            if (!domainType.IsBHoMType() || !rangeType.IsBHoMType())
            {
                // Do not record this relationship.
                return;
            }

            string domainTypeNodeId = rangeType.WebVOWLNodeId();
            string rangeTypeNodeId = rangeType.WebVOWLNodeId();

            if (rangeTypeNodeId == "System.Collections.ObjectModel.KeyedCollection")
                return;

            // See if we have yet to add a Node for the Relation.Subject (domain) type.
            if (!addedTypes.Contains(domainType))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(domainTypeNodeId, "owl:Class");

                classAttributeArray.AddToAttributeArray(domainTypeNodeId, rangeType.GithubURI(), rangeType.DescriptiveName(true), !rangeType.IsInNamespace(internalNamespaces) ?? false);

                addedTypes.Add(domainType.GetTypeInfo());
            }

            // See if we have yet to add a Node for the Relation.Object (range) type.
            if (!addedTypes.Contains(rangeType))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(rangeTypeNodeId, "owl:Class");

                classAttributeArray.AddToAttributeArray(rangeTypeNodeId, rangeType.GithubURI(), rangeType.DescriptiveName(true), !rangeType.IsInNamespace(internalNamespaces) ?? false);

                addedTypes.Add(rangeType.GetTypeInfo());
            }

            // Add the "IsA" relation to link this property to the corresponding type node.
            string propertyTypeRelationId = isARelation.WebVOWLNodeId();
            propertyArray.AddToIdTypeArray(propertyTypeRelationId, "owl:ObjectProperty");
            propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, typeof(IsA).GithubURI(), typeof(IsA).Name, false, new List<string>() { "object" }, new List<string>() { domainType.WebVOWLNodeId() }, new List<string>() { rangeType.WebVOWLNodeId() }, label_iriBased: null);
        }

        /***************************************************/

        private static void AddWebOwlRelationNodes(HasProperty hasPropertyRelation,
                                                JArray classArray,
                                                JArray classAttributeArray, HashSet<Type> addedTypes,
                                                JArray propertyArray = null,
                                                JArray propertyAttributeArray = null,
                                                HashSet<string> internalNamespaces = null,
                                                HashSet<string> exceptions = null)
        {
            // This is the `IRelation.Object` or "range" (to avoid confusion on names)
            Type domainType = hasPropertyRelation.Subject as Type;
            PropertyInfo rangePropertyInfo = hasPropertyRelation.Object as PropertyInfo;

            if (domainType == null)
            {
                log.RecordError($"The {nameof(HasProperty)} relation `{hasPropertyRelation.WebVOWLNodeId()}` has its {nameof(IRelation.Subject)} of type `{hasPropertyRelation.Object.GetType().FullName}` instead of {nameof(Type)}.");
                return;
            }

            if (rangePropertyInfo == null)
            {
                log.RecordError($"The {nameof(HasProperty)} relation `{hasPropertyRelation.WebVOWLNodeId()}` has its {nameof(IRelation.Object)} of type `{hasPropertyRelation.Object.GetType().FullName}` instead of {nameof(PropertyInfo)}.");
                return;
            }

            // Check if the property type is a BHoM type.
            // If the property type is a BHoM type, we need to add a "IsA" relation to a node that represents its property type.
            // We do this by:
            // - creating a node containing the name of the property ("PropertyNameNode"). This is linked to the parent type by a "HasProperty" relation.
            // - creating a node for the property type, if it doesn't exist already, or retrieve its ID if exists already ("PropertyTypeNode"). This is linked to the "PropertyNameNode" by a "IsA" relation.
            if (rangePropertyInfo.PropertyType.IsBHoMType() && propertyArray != null && propertyAttributeArray != null)
            {
                // Add the PropertyNameNode. This node will contain the name of the property.
                string propertyNameNodeId = rangePropertyInfo.DeclaringType.FullName + "." + rangePropertyInfo.Name;
                classArray.AddToIdTypeArray(propertyNameNodeId, "owl:Class");
                classAttributeArray.AddToAttributeArray(propertyNameNodeId, rangePropertyInfo.GithubURI(), rangePropertyInfo.DescriptiveName());

                // Add the "HasProperty" relation between the parent type and the PropertyNameNode.
                string classHasPropertyNameRelationId = domainType.WebVOWLNodeId() + "-HasProperty-" + propertyNameNodeId;
                propertyArray.AddToIdTypeArray(classHasPropertyNameRelationId, "owl:ObjectProperty");
                propertyAttributeArray.AddToAttributeArray(classHasPropertyNameRelationId, hasPropertyRelation.GetType().GithubURI(), hasPropertyRelation.GetType().Name, false, new List<string>() { "object" }, new List<string>() { domainType.WebVOWLNodeId() }, new List<string>() { propertyNameNodeId });

                // Now deal with the property Type.
                string propertyTypeNodeId = rangePropertyInfo.PropertyType.WebVOWLNodeId();

                // See if we have yet to add a Node for the property type.
                if (!addedTypes.Contains(rangePropertyInfo.PropertyType))
                {
                    // We need to add the type of this property as a node.
                    classArray.AddToIdTypeArray(propertyTypeNodeId, "owl:Class");
                    classAttributeArray.AddToAttributeArray(propertyTypeNodeId, rangePropertyInfo.PropertyType.GithubURI(), rangePropertyInfo.PropertyType.DescriptiveName(true), !rangePropertyInfo.PropertyType.IsInNamespace(internalNamespaces) ?? false);
                    addedTypes.Add(rangePropertyInfo.PropertyType.GetTypeInfo());
                }

                // Add the "IsA" relation between the PropertyNameNode and the PropertyTypeNode.
                string propertyNameIsATypeRelationId = propertyNameNodeId + "-IsA-" + propertyTypeNodeId;
                propertyArray.AddToIdTypeArray(propertyNameIsATypeRelationId, "owl:ObjectProperty");
                propertyAttributeArray.AddToAttributeArray(propertyNameIsATypeRelationId, typeof(IsA).GithubURI(), typeof(IsA).Name, false, new List<string>() { "object" }, new List<string>() { propertyNameNodeId }, new List<string>() { propertyTypeNodeId });

                return;
            }

            // If the property type is a IEnumerable with a BHoM type in it, we need to add a "IsAListOf" relation to a node that represents its property type. 
            List<Type> genericBHoMArgs = rangePropertyInfo.PropertyType.GetGenericArguments().Where(t => t.IsBHoMType()).ToList();
            if (typeof(IEnumerable).IsAssignableFrom(rangePropertyInfo.PropertyType) && genericBHoMArgs.Count == 1)
            {
                // Add the PropertyNameNode. This node will contain the name of the property.
                string propertyNameNodeId = rangePropertyInfo.DeclaringType.FullName + "." + rangePropertyInfo.Name;
                classArray.AddToIdTypeArray(propertyNameNodeId, "owl:Class");
                classAttributeArray.AddToAttributeArray(propertyNameNodeId, rangePropertyInfo.GithubURI(), rangePropertyInfo.DescriptiveName());

                // Add the "HasProperty" relation between the parent type and the PropertyNameNode.
                string classHasPropertyNameRelationId = domainType.WebVOWLNodeId() + "-HasProperty-" + propertyNameNodeId;
                propertyArray.AddToIdTypeArray(classHasPropertyNameRelationId, "owl:ObjectProperty");
                propertyAttributeArray.AddToAttributeArray(classHasPropertyNameRelationId, hasPropertyRelation.GetType().GithubURI(), hasPropertyRelation.GetType().Name, false, new List<string>() { "object" }, new List<string>() { domainType.WebVOWLNodeId() }, new List<string>() { propertyNameNodeId });

                // Now deal with the property Type. In this case, the PropertyTypeNode will contain the generic argument type.
                Type ienumerableType = genericBHoMArgs.First();
                string propertyTypeNodeId = ienumerableType.WebVOWLNodeId();

                // See if we have yet to add a Node for the property type.
                if (!addedTypes.Contains(ienumerableType))
                {
                    // We need to add the type of this property as a node.
                    classArray.AddToIdTypeArray(propertyTypeNodeId, "owl:Class");
                    classAttributeArray.AddToAttributeArray(propertyTypeNodeId, ienumerableType.GithubURI(), ienumerableType.DescriptiveName(true), !ienumerableType.IsInNamespace(internalNamespaces) ?? false);
                    addedTypes.Add(ienumerableType.GetTypeInfo());
                }

                // Add the IsAListOf relation.
                string propertyNameIsATypeRelationId = propertyNameNodeId + "-IsAListOf-" + propertyTypeNodeId;
                propertyArray.AddToIdTypeArray(propertyNameIsATypeRelationId, "owl:ObjectProperty");
                propertyAttributeArray.AddToAttributeArray(propertyNameIsATypeRelationId, typeof(IsAListOf).GithubURI(), typeof(IsAListOf).Name, false, new List<string>() { "object" }, new List<string>() { propertyNameNodeId }, new List<string>() { propertyTypeNodeId });

                return;
            }

            // For all other cases

            // Add the class node for the Range of the HasProperty relation. 
            string rangeClassId = rangePropertyInfo.WebVOWLNodeId();
            classArray.AddToIdTypeArray(rangeClassId, "owl:Class");
            classAttributeArray.AddToAttributeArray(rangeClassId, rangePropertyInfo.GithubURI(), rangePropertyInfo.DescriptiveName(), false, new List<string>() { "datatype" });

            // Add the relation connection.
            string propertyTypeRelationId = hasPropertyRelation.WebVOWLNodeId();
            propertyArray.AddToIdTypeArray(propertyTypeRelationId, "owl:DatatypeProperty");
            propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, hasPropertyRelation.GetType().GithubURI(), hasPropertyRelation.GetType().Name, false, new List<string>() { "datatype" }, new List<string>() { domainType.WebVOWLNodeId() }, new List<string>() { rangeClassId });

        }

        /***************************************************/

        private static void AddWebOwlRelationNodes(RequiresProperty requiresPropertyRelation,
                                                    JArray classArray,
                                                    JArray classAttributeArray, HashSet<Type> addedTypes,
                                                    JArray propertyArray = null,
                                                    JArray propertyAttributeArray = null,
                                                    HashSet<string> internalNamespaces = null,
                                                    HashSet<string> exceptions = null)
        {
            Type domainType = requiresPropertyRelation.Subject as Type;
            PropertyInfo rangePi = requiresPropertyRelation.Object as PropertyInfo;

            if (domainType == null || rangePi == null)
            {
                log.RecordError($"Cannot add requiresPropertyRelation `{requiresPropertyRelation.WebVOWLNodeId()}`");
                return;
            }

            if (!domainType.IsBHoMType())
            {
                // Do not record this relationship.
                return;
            }

            string domainTypeNodeId = rangePi.WebVOWLNodeId();
            string rangeTypeNodeId = rangePi.WebVOWLNodeId();

            // See if we have yet to add a Node for the Relation.Subject (domain) type.
            if (!addedTypes.Contains(domainType))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(domainTypeNodeId, "owl:Class");

                classAttributeArray.AddToAttributeArray(domainTypeNodeId, rangePi.GithubURI(), rangePi.DescriptiveName(true), false);

                addedTypes.Add(domainType.GetTypeInfo());
            }

            // See if we have yet to add a Node for the Relation.Object (range) type.
            if (!addedTypes.Contains(rangePi.PropertyType))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(rangeTypeNodeId, "owl:Class");

                classAttributeArray.AddToAttributeArray(rangeTypeNodeId, rangePi.GithubURI(), rangePi.DescriptiveName(true), false, new List<string> { "datatype" });

                addedTypes.Add(rangePi.PropertyType.GetTypeInfo());
            }

            // Add the "IsA" relation to link this property to the corresponding type node.
            string propertyTypeRelationId = requiresPropertyRelation.WebVOWLNodeId();
            propertyArray.AddToIdTypeArray(propertyTypeRelationId, "owl:DatatypeProperty");
            propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, typeof(RequiresProperty).GithubURI(), typeof(RequiresProperty).Name, false, new List<string>() { "datatype" }, new List<string>() { domainType.WebVOWLNodeId() }, new List<string>() { rangePi.WebVOWLNodeId() });
        }
    }
}