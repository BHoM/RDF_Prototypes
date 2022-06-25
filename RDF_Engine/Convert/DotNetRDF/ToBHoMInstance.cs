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
        public static object ToBHoMInstance(this OntologyResource individual, OntologyGraph dotNetRDFOntology)
        {
            if (individual == null || dotNetRDFOntology == null)
                return null;

            Type individualType = individual.EquivalentBHoMType();

            object resultObject = Activator.CreateInstance(individualType);

            if (resultObject == null)
                return null;

            PropertyInfo[] typeProperties = individualType.GetProperties();

            // Get the equivalent properties
            var propertyNodes = dotNetRDFOntology.OwlProperties.Where(p => p.UsedBy.Any(n => n.Types.Any(uriN => uriN.ToString().Contains(individualType.FullName)))).ToList();

            Dictionary<PropertyInfo, object> propertyValues = new Dictionary<PropertyInfo, object>();
            foreach (OntologyProperty propertyNode in propertyNodes)
            {
                Triple predicateNode = propertyNode.TriplesWithPredicate
                    .Where(t => (t.Subject as UriNode)?.Uri.Segments.LastOrDefault() == (individual.Resource as UriNode)?.Uri.Segments.LastOrDefault())
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
                    if (uriNode.ToString() == "http://www.w3.org/1999/02/22-rdf-syntax-ns#seq")
                    {
                        SortedDictionary<int, int> listIdx_tripleIdx = new SortedDictionary<int, int>();
                        for (int i = 0; i < individual.TriplesWithSubject.Count(); i++)
                        {
                            Triple triple = individual.TriplesWithSubject?.ElementAtOrDefault(i);

                            int rdfListIndexFound = -1;
                            string predicateUri = (triple?.Predicate as UriNode)?.Uri.ToString();
                            string listIdxString = predicateUri?.Split('_')?.LastOrDefault();

                            if (int.TryParse(listIdxString, out rdfListIndexFound))
                                listIdx_tripleIdx[rdfListIndexFound] = i;
                        }

                        List<object> listValues = new List<object>();
                        foreach (var kv in listIdx_tripleIdx)
                        {
                            Triple listItemTriple = individual.TriplesWithSubject.ElementAt(kv.Value);
                            OntologyResource listIndividual = listItemTriple.Object.IndividualOntologyResource(dotNetRDFOntology);
                            object convertedIndividual = listIndividual.ToBHoMInstance(dotNetRDFOntology);
                            listValues.Add(convertedIndividual);
                        }

                        propertyValue = listValues;
                    }
                    else
                    {
                        OntologyResource relatedIndividual = uriNode.IndividualOntologyResource(dotNetRDFOntology);
                        propertyValue = relatedIndividual.ToBHoMInstance(dotNetRDFOntology);
                    }
                }

                PropertyInfo correspondingPInfo = typeProperties.FirstOrDefault(pi => pi.FullNameValidChars() == propertyFullName);
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
                try
                {
                    pValue.Key.SetValue(resultObject, convertedValue);
                }
                catch { }
            }

            return resultObject;
        }
    }
}