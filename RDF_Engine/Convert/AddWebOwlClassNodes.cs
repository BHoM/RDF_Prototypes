﻿
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
        private static string AddWebOwlClassNodes(Type type, JArray classArray, JArray classAttributeArray, HashSet<string> addedTypes, HashSet<string> internalNamespaces = null)
        {
            string typeId = type.WebVOWLNodeId();

            if (addedTypes.Contains(typeId))
                return typeId;

            Uri typeUri = type.GithubURI();
            string comment = type.DescriptionInAttribute();
            bool isExternal = !type.IsInNamespace(internalNamespaces) ?? false;

            // 1) CLASS
            classArray.AddToIdTypeArray(typeId, "owl:Class");

            // 2) CLASS ATTRIBUTE
            classAttributeArray.AddToAttributeArray(typeId, typeUri, type.DescriptiveName(true), isExternal, new List<string>() { "object" }, comment);

            addedTypes.Add(typeId);

            return typeId;
        }

        private static string AddWebOwlClassNodes(PropertyInfo pInfo, JArray classArray, JArray classAttributeArray, HashSet<string> addedTypes)
        {
            string propertyNodeId = pInfo.WebVOWLNodeId();

            // Check if we need to add a class node for the property type.
            if (!addedTypes.Contains(propertyNodeId))
            {

                // 1) CLASS
                classArray.AddToIdTypeArray(propertyNodeId, "owl:Class");

                // 2) CLASS ATTRIBUTE
                classAttributeArray.AddToAttributeArray(propertyNodeId, pInfo.GithubURI(), pInfo.DescriptiveName());

                addedTypes.Add(propertyNodeId);
            }

            return propertyNodeId;
        }
    }
}