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

                LiteralNode literalNode = predicateNode.Object as LiteralNode;
                if (literalNode != null)
                {
                    if (literalNode.DataType.AbsolutePath.EndsWith(typeof(Base64JsonSerialized).FullName))
                        propertyValue = literalNode.Value.FromBase64JsonSerialized();
                    else
                        propertyValue = literalNode.Value;
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
                }

                PropertyInfo correspondingPInfo = typeProperties.FirstOrDefault(pi => pi.FullNameValidChars() == propertyFullName);

                if (propertyFullName.Contains("aterial"))
                {
                    int asd = 0;
                }

                if (correspondingPInfo == null && isCustomType)
                {
                    string propertyName = propertyFullName.Split('.').LastOrDefault();
                    correspondingPInfo = new Types.CustomPropertyInfo(individualType as Types.CustomObjectType, new KeyValuePair<string, Type>(propertyName, typeof(object)));
                }

                propertyValues[correspondingPInfo] = propertyValue;
            }

            foreach (KeyValuePair<PropertyInfo, object> pValue in propertyValues)
            {
                object convertedValue = null;

                try
                {
                    convertedValue = System.Convert.ChangeType(pValue.Value, pValue.Key.PropertyType);
                }
                catch { }

                if (convertedValue == null)
                {
                    if (pValue.Key.PropertyType == typeof(Guid))
                        convertedValue = new Guid(pValue.Value.ToString());
                    else if (typeof(IList).IsAssignableFrom(pValue.Key.PropertyType))
                    {
                        List<object> valueList = pValue.Value as List<object>;

                        if (valueList != null)
                        {
                            // Convert list of objects to list of specific inner type
                            Type listGenericArgument = pValue.Key.PropertyType.GetGenericArguments()[0];
                            var methodInfo = typeof(Queryable).GetMethod("OfType");
                            var genericMethod = methodInfo?.MakeGenericMethod(listGenericArgument);
                            try
                            {
                                Type listType = typeof(List<>).MakeGenericType(listGenericArgument);
                                IList list = Activator.CreateInstance(listType) as IList;
                                foreach (var item in pValue.Value as List<object>)
                                    list.Add(item);

                                convertedValue = list;
                            }
                            catch { }
                        }
                    }
                }

                // Fallback: try keeping the unconverted value.
                if (convertedValue == null)
                    convertedValue = pValue.Value;

                // Set the convertedValue to the property.
                bool setSuccessfully = false;
                try
                {
                    pValue.Key.SetValue(resultObject, convertedValue);
                    setSuccessfully = true;
                }
                catch
                {
                }

                if (!setSuccessfully)
                {
                    Type propType = pValue.Value.GetType();
                    var field = resultObject.GetType().GetField($"<{pValue.Key.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                    field.SetValue(resultObject, convertedValue);
                }
            }

            return resultObject;
        }

        private static Uri Uri(this INode node)
        {
            return (node as UriNode)?.Uri;
        }
    }
}