
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
                                                    JArray classAttributeArray,
                                                    HashSet<string> addedTypes,
                                                    TBoxSettings settings,
                                                    JArray propertyArray = null,
                                                    JArray propertyAttributeArray = null,
                                                    HashSet<string> internalNamespaces = null,
                                                    HashSet<string> exceptions = null,
                                                    int recursionLevel = 0)
        {
            string propertyTypeRelationId = isSubclassOf.WebVOWLNodeId();
            if (addedTypes.Contains(propertyTypeRelationId))
                return;

            Type domainType = isSubclassOf.Subject as Type;
            Type rangeType = isSubclassOf.Object as Type;

            // Filter using input exceptions.
            if (exceptions?.Contains(rangeType.FullName) ?? false)
                return;

            // Filter using default exceptions. These apply only when the domainType namespace is not "BH.oM.Base". Useful to remove uninteresting relations.
            //if ((!domainType?.Namespace.StartsWith("BH.oM.Base") ?? false) && (rangeType.Name == "BHoMObject"))
            //    return;

            if (!rangeType.IsBHoMType())
                return;

            if (domainType == null || rangeType == null)
            {
                log.RecordError($"Cannot add IsA relation `{isSubclassOf.WebVOWLNodeId()}`");
                return;
            }

            string propertyTypeNodeId = rangeType.WebVOWLNodeId();

            // See if we have yet to add a Node for the Relation.Object type.
            if (!addedTypes.Contains(propertyTypeNodeId))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(propertyTypeNodeId, "owl:Class");

                classAttributeArray.AddToAttributeArray(propertyTypeNodeId, rangeType.GithubURI(settings), rangeType.DescriptiveName(true), !rangeType.IsInNamespace(internalNamespaces) ?? false);

                addedTypes.Add(propertyTypeNodeId);
            }

            // Add the "IsSubclassOf" relation to link this property to the corresponding type node.
            propertyArray.AddToIdTypeArray(propertyTypeRelationId, "rdfs:subClassOf");
            propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, new Uri(typeof(IsSubclassOf).GithubURI(settings).ToString(), UriKind.Absolute), typeof(IsA).Name, attributes: new List<string>() { "object" }, domain: new List<string>() { domainType.WebVOWLNodeId() }, range: new List<string>() { rangeType.WebVOWLNodeId() }, label_iriBased: null);
            addedTypes.Add(propertyTypeRelationId);

            // Add other relations for the range PropertyTypeNode
            if (recursionLevel > 0)
            {
                var relations = rangeType.RelationsFromType();
                foreach (IRelation relation in relations)
                    AddWebOwlRelationNodes(relation as dynamic, classArray, classAttributeArray, addedTypes, settings: settings, propertyArray: propertyArray, propertyAttributeArray: propertyAttributeArray, internalNamespaces: internalNamespaces, exceptions: exceptions, recursionLevel: recursionLevel - 1);
            }
        }

        /***************************************************/

        private static void AddWebOwlRelationNodes(IsAListOf isAListOfRelation,
                                                    JArray classArray,
                                                    JArray classAttributeArray,
                                                    HashSet<string> addedTypes,
                                                    TBoxSettings settings,
                                                    JArray propertyArray = null,
                                                    JArray propertyAttributeArray = null,
                                                    HashSet<string> internalNamespaces = null,
                                                    HashSet<string> exceptions = null,
                                                    int recursionLevel = 0)
        {
            string propertyTypeRelationId = isAListOfRelation.WebVOWLNodeId();
            if (addedTypes.Contains(propertyTypeRelationId))
                return;

            Type domainType = isAListOfRelation.Object as Type;
            Type rangeType = isAListOfRelation.Object as Type;

            if (exceptions?.Contains(rangeType.FullName) ?? false)
                return;

            if (domainType == null || rangeType == null)
            {
                log.RecordError($"Cannot add IsAListOf relation `{isAListOfRelation.WebVOWLNodeId()}`");
                return;
            }

            string propertyTypeNodeId = rangeType.WebVOWLNodeId();

            // See if we have yet to add a Node for the Relation.Object type.
            if (!addedTypes.Contains(propertyTypeNodeId))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(propertyTypeNodeId, "owl:Class");

                classAttributeArray.AddToAttributeArray(propertyTypeNodeId, rangeType.GithubURI(settings), rangeType.DescriptiveName(true), !rangeType.IsInNamespace(internalNamespaces) ?? false);

                addedTypes.Add(propertyTypeNodeId);
            }

            // Add the "IsAListOf" relation to link this property to the corresponding type node.
            propertyArray.AddToIdTypeArray(propertyTypeRelationId, "owl:ObjectProperty");
            propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, new Uri(typeof(IsAListOf).GithubURI(settings).ToString(), UriKind.Absolute), typeof(IsAListOf).Name, attributes: new List<string>() { "object" }, domain: new List<string>() { domainType.WebVOWLNodeId() }, range: new List<string>() { rangeType.WebVOWLNodeId() }, label_iriBased: null);

            // Add other relations for the range PropertyTypeNode
            if (recursionLevel > 0)
            {
                var relations = rangeType.RelationsFromType();
                foreach (IRelation relation in relations)
                    AddWebOwlRelationNodes(relation as dynamic, classArray, classAttributeArray, addedTypes, settings: settings, propertyArray: propertyArray, propertyAttributeArray: propertyAttributeArray, internalNamespaces: internalNamespaces, exceptions: exceptions, recursionLevel: recursionLevel - 1);
            }
        }

        /***************************************************/

        private static void AddWebOwlRelationNodes(IsA isARelation,
                                                    JArray classArray,
                                                    JArray classAttributeArray,
                                                    HashSet<string> addedTypes,
                                                    TBoxSettings settings,
                                                    JArray propertyArray = null,
                                                    JArray propertyAttributeArray = null,
                                                    HashSet<string> internalNamespaces = null,
                                                    HashSet<string> exceptions = null,
                                                    int recursionLevel = 0)
        {
            string propertyTypeRelationId = isARelation.WebVOWLNodeId();
            if (addedTypes.Contains(propertyTypeRelationId))
                return;

            Type domainType = isARelation.Subject as Type;
            Type rangeType = isARelation.Object as Type;

            // Filter specific exceptions
            if (exceptions?.Contains(rangeType.FullNameValidChars()) ?? false)
                return;

            // Filter using default exceptions. These apply only when the domainType namespace is not "BH.oM.Base". Useful to remove uninteresting relations.
            //if ((!domainType?.Namespace.StartsWith("BH.oM.Base") ?? false) && (rangeType.Name == "IObject" || rangeType.Name == "IBHoMObject"))
            //    return;

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

            // See if we have yet to add a Node for the Relation.Subject (domain) type.
            string domainTypeNodeId = AddWebOwlClassNodes(domainType, classArray, classAttributeArray, addedTypes, settings, internalNamespaces);

            // See if we have yet to add a Node for the Relation.Object (range) type.
            string rangeTypeNodeId = AddWebOwlClassNodes(rangeType, classArray, classAttributeArray, addedTypes, settings, internalNamespaces);

            // Add the "IsA" relation to link this property to the corresponding type node.
            propertyArray.AddToIdTypeArray(propertyTypeRelationId, "owl:ObjectProperty");
            propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, typeof(IsA).GithubURI(settings), typeof(IsA).Name, false, new List<string>() { "object" }, typeof(IsA).DescriptionInAttribute(), new List<string>() { domainTypeNodeId }, new List<string>() { rangeTypeNodeId });
            addedTypes.Add(propertyTypeRelationId);

            // Add other relations for the range PropertyTypeNode
            if (recursionLevel > 0)
            {
                var relations = rangeType.RelationsFromType();
                foreach (IRelation relation in relations)
                    AddWebOwlRelationNodes(relation as dynamic, classArray, classAttributeArray, addedTypes, settings: settings, propertyArray: propertyArray, propertyAttributeArray: propertyAttributeArray, internalNamespaces: internalNamespaces, exceptions: exceptions, recursionLevel: recursionLevel - 1);
            }
        }

        /***************************************************/

        private static void AddWebOwlRelationNodes(HasProperty hasPropertyRelation,
                                                JArray classArray,
                                                JArray classAttributeArray,
                                                HashSet<string> addedTypes,
                                                TBoxSettings settings,
                                                JArray propertyArray = null,
                                                JArray propertyAttributeArray = null,
                                                HashSet<string> internalNamespaces = null,
                                                HashSet<string> exceptions = null,
                                                int recursionLevel = 0)
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
                if (!addedTypes.Contains(propertyNameNodeId))
                {
                    classArray.AddToIdTypeArray(propertyNameNodeId, "owl:Class");
                    classAttributeArray.AddToAttributeArray(propertyNameNodeId, rangePropertyInfo.GithubURI(settings), rangePropertyInfo.DescriptiveName(), false, null, rangePropertyInfo.DescriptionInAttribute());
                    addedTypes.Add(propertyNameNodeId);
                }

                // Add the "HasProperty" relation between the parent type and the PropertyNameNode.
                string classHasPropertyNameRelationId = domainType.WebVOWLNodeId() + "-HasProperty-" + propertyNameNodeId;
                if (!addedTypes.Contains(classHasPropertyNameRelationId))
                {
                    propertyArray.AddToIdTypeArray(classHasPropertyNameRelationId, "owl:ObjectProperty");
                    propertyAttributeArray.AddToAttributeArray(classHasPropertyNameRelationId, hasPropertyRelation.GetType().GithubURI(settings), hasPropertyRelation.DescriptiveName(), false, new List<string>() { "object" }, domain: new List<string>() { domainType.WebVOWLNodeId() }, range: new List<string>() { propertyNameNodeId });
                    addedTypes.Add(classHasPropertyNameRelationId);
                }

                // Now deal with the property Type.

                // See if we have yet to add a Node for the property type.
                string propertyTypeNodeId = AddWebOwlClassNodes(rangePropertyInfo.PropertyType, classArray, classAttributeArray, addedTypes, settings, internalNamespaces);

                // Add the "IsA" relation between the PropertyNameNode and the PropertyTypeNode.
                string propertyNameIsATypeRelationId = propertyNameNodeId + "-IsA-" + propertyTypeNodeId;
                if (!addedTypes.Contains(propertyNameIsATypeRelationId))
                {
                    propertyArray.AddToIdTypeArray(propertyNameIsATypeRelationId, "owl:ObjectProperty");
                    propertyAttributeArray.AddToAttributeArray(propertyNameIsATypeRelationId, typeof(IsA).GithubURI(settings), typeof(IsA).Name, false, new List<string>() { "object" }, domain: new List<string>() { propertyNameNodeId }, range: new List<string>() { propertyTypeNodeId });
                    addedTypes.Add(propertyNameIsATypeRelationId);
                }

                // Add other relations for this PropertyTypeNode
                if (recursionLevel > 0)
                {
                    var relations = rangePropertyInfo.PropertyType.RelationsFromType();
                    foreach (IRelation relation in relations)
                        AddWebOwlRelationNodes(relation as dynamic, classArray, classAttributeArray, addedTypes, settings: settings, propertyArray: propertyArray, propertyAttributeArray: propertyAttributeArray, internalNamespaces: internalNamespaces, exceptions: exceptions, recursionLevel: recursionLevel - 1);
                }

                return;
            }

            // If the property type is a IEnumerable with a BHoM type in it, we need to add a "IsAListOf" relation to a node that represents its property type. 
            List<Type> genericBHoMArgs = rangePropertyInfo.PropertyType.GetGenericArguments().Where(t => t.IsBHoMType()).ToList();
            if (typeof(IEnumerable).IsAssignableFrom(rangePropertyInfo.PropertyType) && genericBHoMArgs.Count == 1)
            {
                // Add the PropertyNameNode. This node will contain the name of the property.
                string propertyNameNodeId = rangePropertyInfo.DeclaringType.FullName + "." + rangePropertyInfo.Name;
                if (!addedTypes.Contains(propertyNameNodeId))
                {
                    classArray.AddToIdTypeArray(propertyNameNodeId, "owl:Class");
                    classAttributeArray.AddToAttributeArray(propertyNameNodeId, rangePropertyInfo.GithubURI(settings), rangePropertyInfo.DescriptiveName());
                    addedTypes.Add(propertyNameNodeId);
                }

                // Add the "HasProperty" relation between the parent type and the PropertyNameNode.
                string classHasPropertyNameRelationId = domainType.WebVOWLNodeId() + "-HasProperty-" + propertyNameNodeId;
                if (!addedTypes.Contains(classHasPropertyNameRelationId))
                {
                    propertyArray.AddToIdTypeArray(classHasPropertyNameRelationId, "owl:ObjectProperty");
                    propertyAttributeArray.AddToAttributeArray(classHasPropertyNameRelationId, hasPropertyRelation.GetType().GithubURI(settings), hasPropertyRelation.GetType().Name, false, new List<string>() { "object" }, domain: new List<string>() { domainType.WebVOWLNodeId() }, range: new List<string>() { propertyNameNodeId });
                    addedTypes.Add(classHasPropertyNameRelationId);
                }

                // Now deal with the property Type. In this case, the PropertyTypeNode will contain the generic argument type.
                Type ienumerableType = genericBHoMArgs.First();

                // See if we have yet to add a Node for the property type.
                string propertyTypeNodeId = AddWebOwlClassNodes(ienumerableType, classArray, classAttributeArray, addedTypes, settings, internalNamespaces);

                // Add the IsAListOf relation.
                string propertyNameIsATypeRelationId = propertyNameNodeId + "-IsAListOf-" + propertyTypeNodeId;
                if (!addedTypes.Contains(propertyNameIsATypeRelationId))
                {
                    propertyArray.AddToIdTypeArray(propertyNameIsATypeRelationId, "owl:ObjectProperty");
                    propertyAttributeArray.AddToAttributeArray(propertyNameIsATypeRelationId, typeof(IsAListOf).GithubURI(settings), typeof(IsAListOf).Name, false, new List<string>() { "object" }, domain: new List<string>() { propertyNameNodeId }, range: new List<string>() { propertyTypeNodeId });
                    addedTypes.Add(propertyNameIsATypeRelationId);
                }

                // Add other relations for this PropertyTypeNode
                if (recursionLevel > 0)
                {
                    var relations = ienumerableType.RelationsFromType();
                    foreach (IRelation relation in relations)
                        AddWebOwlRelationNodes(relation as dynamic, classArray, classAttributeArray, addedTypes, settings, propertyArray: propertyArray, propertyAttributeArray: propertyAttributeArray, internalNamespaces: internalNamespaces, exceptions: exceptions, recursionLevel: recursionLevel - 1);
                }
                return;
            }

            // For all other cases - add DataType

            // Add the class node for the Range of the HasProperty relation. 
            string rangeClassId = rangePropertyInfo.WebVOWLNodeId();
            if (!addedTypes.Contains(rangeClassId))
            {
                classArray.AddToIdTypeArray(rangeClassId, "owl:Class"); // Can be changed to `owl:Class` to allow URI link
                classAttributeArray.AddToAttributeArray(rangeClassId, rangePropertyInfo.GithubURI(settings), rangePropertyInfo.DescriptiveName(), false, new List<string>() { "datatype" });
                addedTypes.Add(rangeClassId);
            }

            // Add the relation connection.
            string propertyTypeRelationId = hasPropertyRelation.WebVOWLNodeId();
            if (!addedTypes.Contains(propertyTypeRelationId))
            {
                propertyArray.AddToIdTypeArray(propertyTypeRelationId, "owl:DatatypeProperty");
                propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, hasPropertyRelation.GetType().GithubURI(settings), hasPropertyRelation.GetType().Name, false, new List<string>() { "datatype" }, domain: new List<string>() { domainType.WebVOWLNodeId() }, range: new List<string>() { rangeClassId });
                addedTypes.Add(propertyTypeRelationId);
            }
        }

        /***************************************************/

        private static void AddWebOwlRelationNodes(RequiresProperty requiresPropertyRelation,
                                                    JArray classArray,
                                                    JArray classAttributeArray,
                                                    HashSet<string> addedTypes,
                                                    TBoxSettings settings,
                                                    JArray propertyArray = null,
                                                    JArray propertyAttributeArray = null,
                                                    HashSet<string> internalNamespaces = null,
                                                    HashSet<string> exceptions = null,
                                                    int recursionLevel = 0)
        {
            string propertyTypeRelationId = requiresPropertyRelation.WebVOWLNodeId();
            if (addedTypes.Contains(propertyTypeRelationId))
                return;

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

            string domainTypeNodeId = domainType.WebVOWLNodeId();
            string rangeTypeNodeId = rangePi.WebVOWLNodeId();

            // See if we have yet to add a Node for the Relation.Subject (domain) type.
            if (!addedTypes.Contains(domainTypeNodeId))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(domainTypeNodeId, "owl:Class");

                classAttributeArray.AddToAttributeArray(domainTypeNodeId, rangePi.GithubURI(settings), rangePi.DescriptiveName(true), false);

                addedTypes.Add(domainTypeNodeId);
            }

            // See if we have yet to add a Node for the Relation.Object (range) type.
            if (!addedTypes.Contains(rangeTypeNodeId))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(rangeTypeNodeId, "rdfs:Datatype"); // Can be changed to `owl:Class` to allow URI link

                classAttributeArray.AddToAttributeArray(rangeTypeNodeId, rangePi.GithubURI(settings), rangePi.DescriptiveName(true), false, new List<string> { "datatype" });

                addedTypes.Add(rangeTypeNodeId);
            }

            // Add the "RequiresProperty" relation to link this property to the corresponding type node.
            propertyArray.AddToIdTypeArray(propertyTypeRelationId, "owl:DatatypeProperty");
            propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, typeof(RequiresProperty).GithubURI(settings), typeof(RequiresProperty).Name, false, new List<string>() { "datatype" }, domain: new List<string>() { domainType.WebVOWLNodeId() }, range: new List<string>() { rangePi.WebVOWLNodeId() });
            addedTypes.Add(propertyTypeRelationId);
        }
    }
}