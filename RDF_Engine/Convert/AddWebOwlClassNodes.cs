
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
        private static string AddWebOwlClassNodes(Type type, JArray classArray, JArray classAttributeArray, HashSet<string> addedWebVOWLNodeIds, LocalRepositorySettings settings, HashSet<string> internalNamespaces = null)
        {
            string typeId = type.UniqueNodeId();

            if (addedWebVOWLNodeIds.Contains(typeId))
                return typeId;

            Uri typeUri = type.GithubURI(settings);
            string comment = type.DescriptionInAttribute();
            bool isExternal = !type.IsInNamespace(internalNamespaces) ?? false;

            // 1) CLASS
            classArray.AddToIdTypeArray(typeId, "owl:Class");

            // 2) CLASS ATTRIBUTE
            classAttributeArray.AddToAttributeArray(typeId, typeUri, type.DescriptiveName(true), isExternal, new List<string>() { "object" }, comment);

            addedWebVOWLNodeIds.Add(typeId);

            return typeId;
        }

        private static string AddWebOwlClassNodes(PropertyInfo pInfo, JArray classArray, JArray classAttributeArray, HashSet<string> addedWebVOWLNodeIds, LocalRepositorySettings settings)
        {
            string propertyNodeId = pInfo.UniqueNodeId();

            // Check if we need to add a class node for the property type.
            if (!addedWebVOWLNodeIds.Contains(propertyNodeId))
            {

                // 1) CLASS
                classArray.AddToIdTypeArray(propertyNodeId, "owl:Class");

                // 2) CLASS ATTRIBUTE
                classAttributeArray.AddToAttributeArray(propertyNodeId, pInfo.GithubURI(settings), pInfo.DescriptiveName());

                addedWebVOWLNodeIds.Add(propertyNodeId);
            }

            return propertyNodeId;
        }
    }
}