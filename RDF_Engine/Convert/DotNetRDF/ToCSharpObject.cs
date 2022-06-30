using BH.oM.Base;
using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Ontology;

namespace BH.Engine.RDF
{
    public static partial class Convert
    {
        public static object ToCSharpObject(this OntologyResource individual, OntologyGraph dotNetRDFOntology)
        {
            if (individual == null || dotNetRDFOntology == null)
                return null;

            Type individualType = individual.EquivalentBHoMType();
            bool isCustomType = typeof(Types.CustomObjectType).IsAssignableFrom(individualType.GetType());

            object resultObject = null;

            try
            {
                resultObject = Activator.CreateInstance(individualType);
            }
            catch (Exception e)
            {
                Log.RecordWarning($"The conversion does not support the type {individualType.FullName} yet. Error:\n{e.Message}");
                return null;
            }

            if (resultObject == null)
                return null;

            if (isCustomType)
                (resultObject as CustomObject).CustomData[new TBoxSettings().CustomobjectsTypeKey] = individualType.Name;

            List<PropertyInfo> typeProperties = individualType.GetProperties().ToList();

            // Get the equivalent properties
            List<OntologyProperty> propertyNodes = dotNetRDFOntology.OwlProperties.Where(p => p.UsedBy.Any(n => n.Types.Any(uriN => uriN.ToString().Contains(individualType.FullName)))).ToList();

            Dictionary<PropertyInfo, object> propertyValues = new Dictionary<PropertyInfo, object>();
            foreach (OntologyProperty propertyNode in propertyNodes)
            {
                Triple predicateNode = propertyNode.TriplesWithPredicate
                    .Where(t => (t.Subject as UriNode)?.Uri.Segments.LastOrDefault() == individual.Resource.Uri().Segments.LastOrDefault())
                    .FirstOrDefault();

                if (predicateNode == null)
                    continue;

                string propertyFullName = predicateNode.Predicate.BHoMSegment();
                object propertyValue = null;
                Type propertyType = typeof(object);

                LiteralNode literalNode = predicateNode.Object as LiteralNode;
                if (literalNode != null)
                {
                    if (literalNode.DataType.AbsolutePath.EndsWith(typeof(Base64JsonSerialized).FullName))
                        propertyValue = literalNode.Value.FromBase64JsonSerialized();
                    else
                        propertyValue = literalNode.Value;

                    propertyType = BH.oM.RDF.OntologyTypeMap.FromOntologyDataType.Where(kv => kv.Key.Contains(literalNode.DataType.Fragment.OnlyAlphabetic())).FirstOrDefault().Value ?? typeof(object);
                }

                UriNode uriNode = predicateNode.Object as UriNode;
                if (uriNode != null)
                {
                    // Check if it is a List.
                    // Could not find a more reliable way that checking the uri address for mentions of "rdf" and "seq".
                    string typeAddress = uriNode.ToString().ToLower();
                    var typeAddressPortions = typeAddress.Split('#').SelectMany(p => p.Split('-')).SelectMany(p => p.Split('/')).ToList();
                    if (typeAddressPortions.Contains("rdf") && typeAddressPortions.Contains("seq"))
                    {
                        SortedDictionary<int, int> listIdx_tripleIdx = new SortedDictionary<int, int>();

                        bool sequenceStarted = false;

                        for (int i = 0; i < individual.TriplesWithSubject.Count(); i++)
                        {
                            Triple triple = individual.TriplesWithSubject.ElementAtOrDefault(i);

                            if (triple.Predicate.Uri().ToString().Contains(propertyFullName))
                            {
                                sequenceStarted = true;
                                continue;
                            }

                            if (!sequenceStarted)
                                continue;

                            int rdfListIndexFound = -1;
                            string predicateUri = (triple?.Predicate as UriNode)?.Uri.ToString();
                            string listIdxString = predicateUri?.Split('_')?.LastOrDefault();

                            if (int.TryParse(listIdxString, out rdfListIndexFound))
                                listIdx_tripleIdx[rdfListIndexFound] = i;
                            else
                            {
                                // Reached end of the sequence.
                                break;
                            }
                        }

                        List<object> listValues = new List<object>();
                        foreach (var kv in listIdx_tripleIdx)
                        {
                            Triple listItemTriple = individual.TriplesWithSubject.ElementAt(kv.Value);
                            OntologyResource listIndividual = listItemTriple.Object.IndividualOntologyResource(dotNetRDFOntology);
                            object convertedIndividual = listIndividual.ToCSharpObject(dotNetRDFOntology);
                            listValues.Add(convertedIndividual);
                        }

                        propertyValue = listValues;
                    }
                    else
                    {
                        OntologyResource relatedIndividual = uriNode.IndividualOntologyResource(dotNetRDFOntology);
                        propertyValue = relatedIndividual.ToCSharpObject(dotNetRDFOntology);
                    }

                    if (propertyValue == null)
                    {
                        int asdaa = 0;
                    }
                }

                PropertyInfo correspondingPInfo = typeProperties.FirstOrDefault(pi => pi.FullNameValidChars() == propertyFullName);

                if (correspondingPInfo == null)
                {
                    string propertyName = propertyFullName.Split('.').LastOrDefault();
                    correspondingPInfo = new Types.CustomPropertyInfo(individualType as Types.CustomObjectType, new KeyValuePair<string, Type>(propertyName, propertyType));
                }

                object convertedValue = ConvertValue(correspondingPInfo, propertyValue);

                SetValueOnResultObject(ref resultObject, correspondingPInfo, convertedValue);
            }

            return resultObject;
        }

        private static void SetValueOnResultObject(ref object resultObject, PropertyInfo pInfo, object convertedValue)
        {
            // If the property is CustomData, first check whether it wasn't already set (e.g. via a CustomPropertyInfo set).
            if (pInfo.Name == nameof(BHoMObject.CustomData) && resultObject is BHoMObject && convertedValue is Dictionary<string, object>)
            {
                BHoMObject resultBHoMObj = (BHoMObject)resultObject;

                if (resultBHoMObj?.CustomData != null)
                {
                    // Only add CustomData keys that are not already set.
                    foreach (var kv in (Dictionary<string, object>)convertedValue)
                    {
                        if (!resultBHoMObj.CustomData.ContainsKey(kv.Key))
                            resultBHoMObj.CustomData[kv.Key] = kv.Value;
                    }
                    return;
                }
            }

            // Set the convertedValue to the property.
            try
            {
                pInfo.SetValue(resultObject, convertedValue);
                return;
            }
            catch { }

            var field = resultObject.GetType().GetField($"<{pInfo.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

            if (field != null)
            {
                try
                {
                    field.SetValue(resultObject, convertedValue);
                    return;
                }
                catch { }
            }
            else
            {
                // Try set as Custom Data.
                if (resultObject is BHoMObject)
                    try
                    {
                        (resultObject as dynamic).CustomData[pInfo.Name] = convertedValue;
                        return;
                    }
                    catch { }
            }

            Log.RecordWarning($"Could not set property `{pInfo.Name}` on a {resultObject.GetType().FullName}.");
        }

        private static object ConvertValue(PropertyInfo pInfo, object value)
        {
            object convertedValue = null;

            try
            {
                convertedValue = System.Convert.ChangeType(value, pInfo.PropertyType);
            }
            catch { }

            if (convertedValue == null)
            {
                if (pInfo.PropertyType == typeof(Guid))
                    convertedValue = new Guid(value.ToString());
                else if (typeof(IList).IsAssignableFrom(pInfo.PropertyType))
                {
                    List<object> valueList = value as List<object>;

                    if (valueList != null)
                    {
                        // Convert list of objects to list of specific inner type
                        Type listGenericArgument = pInfo.PropertyType.GetGenericArguments()[0];
                        var methodInfo = typeof(Queryable).GetMethod("OfType");
                        var genericMethod = methodInfo?.MakeGenericMethod(listGenericArgument);
                        try
                        {
                            Type listType = typeof(List<>).MakeGenericType(listGenericArgument);
                            IList list = Activator.CreateInstance(listType) as IList;
                            foreach (var item in value as List<object>)
                                list.Add(item);

                            convertedValue = list;
                        }
                        catch { }
                    }
                }
            }

            // Fallback: try keeping the unconverted value.
            if (convertedValue == null)
                convertedValue = value;
            return convertedValue;
        }

        private static Uri Uri(this INode node)
        {
            return (node as UriNode)?.Uri;
        }
    }
}