
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
        private static string AddWebOwlClassNodes(PropertyInfo pInfo, JArray classArray, JArray classAttributeArray, HashSet<Type> addedTypes)
        {
            string propertyNodeId = pInfo.PropertyType.WebVOWLNodeId();

            // Check if we need to add a class node for the property type.
            if (!addedTypes.Contains(pInfo.PropertyType))
            {

                // 1) CLASS
                classArray.AddToIdTypeArray(propertyNodeId, "owl:Class");

                // 2) CLASS ATTRIBUTE
                classAttributeArray.AddToAttributeArray(propertyNodeId, pInfo.GithubURI(), pInfo.DescriptiveName());

                addedTypes.Add(pInfo.PropertyType);
            }

            return propertyNodeId;
        }

        private static string AddWebOwlClassNodes(Type type, JArray classArray, JArray classAttributeArray, HashSet<Type> addedTypes)
        {
            string typeId = type.WebVOWLNodeId();
            Uri typeUri = type.GithubURI();

            if (typeUri == null)
                return typeId;

            if (!addedTypes.Contains(type))
            {
                // 1) CLASS
                classArray.AddToIdTypeArray(typeId, "owl:Class");

                // 2) CLASS ATTRIBUTE
                classAttributeArray.AddToAttributeArray(typeId, typeUri, type.DescriptiveName(true));

                addedTypes.Add(type);
            }

            return typeId;
        }
    }
}