using BH.oM.Analytical.Results;
using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.RDF;
using VDS.RDF;
using VDS.RDF.Writing;
using BH.oM.Architecture.Elements;
using BH.oM.Physical.Elements;
using BH.oM.RDF;
using System.ComponentModel;
using System.Diagnostics;
using VDS.RDF.Ontology;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;

namespace BH.Test.RDF
{
    // Class that performs Unit Test checks, necessary because I could not get any Unit Testing library to work,
    // likely due to .NET incompatibilities.
    // If we can get other unit testing libraries to work, calls to this class can simply be replaced.
    public static partial class Assert
    {
        public static void IsTTLParsable(string ttl)
        {
            try
            {
                OntologyGraph g = BH.Engine.RDF.Convert.ToDotNetRDF(ttl);

                if (g == null)
                    RecordTestFailure("TTL was not parsable.");
            }
            catch
            {
                RecordTestFailure("TTL was not parsable.");
            }
        }

        public static void IsNotNull(object value, string error = null)
        {
            if (value == null)
                RecordTestFailure(error);
        }

        public static void IsEqual(object obj1, object obj2, string error = null)
        {
            List<Difference> differences = new List<Difference>();

            if (!AreObjectsEqual(obj1, obj2, out differences))
            {
                var differencesText = string.Join("\n", differences.Select(d =>
                    $"{obj1.GetType().FullName}.{d.PropertyName} should have been {d.Object1Value} but was {d.Object2Value}."));

                RecordTestFailure($"{obj1.GetType().FullName} should have been equal to the other object." + (!differences.Any() ? "" : $"Differences:\n\t{differencesText.SplitInLinesAndTabify()}"));
            }
        }

        public static void IsNotEqual(object obj1, object obj2, string error = null)
        {
            List<Difference> differences = new List<Difference>();

            if (AreObjectsEqual(obj1, obj2, out differences))
                RecordTestFailure($"{obj1.GetType().FullName} should have been different to the other object.");
        }

        private static bool AreObjectsEqual(object obj1, object obj2, out List<Difference> differences)
        {
            differences = new List<Difference>();

            if (obj1 != null && obj2 == null || obj1 == null)
                return false;

            CompareLogic compareLogic = new CompareLogic() { Config = new KellermanSoftware.CompareNetObjects.ComparisonConfig() { MaxDifferences = 99 } };
            OnlyIEnumContents onlyIEnumContentsComparer = new OnlyIEnumContents();
            compareLogic.Config.CustomComparers.Add(onlyIEnumContentsComparer);
            ComparisonResult result = compareLogic.Compare(obj1, obj2);

            if (!result.AreEqual)
            {
                result.Differences.RemoveAll(d => true);
                result.Differences.AddRange(onlyIEnumContentsComparer.ListObjectDifferences);
            }

            differences = result.Differences;

            return result.AreEqual;
        }

        public static void IsTrue(bool value, string error = null)
        {
            if (!value)
                RecordTestFailure(error);
        }

        public static void Single<T>(this IEnumerable<T> iList, string variableName = null)
        {
            TotalCount(iList, 1, variableName);
        }

        public static void Single<T>(this IEnumerable<T> iList, object obj, string variableName = null)
        {
            TotalCount(iList.Where(o => o.Equals(obj)), 1, variableName);
        }

        public static void TotalCount<T>(this IEnumerable<T> iList, int expectedCount, string variableName = null)
        {
            int count = iList.Count();
            variableName = string.IsNullOrWhiteSpace(variableName) ? "" : variableName + " ";
            if (count != expectedCount)
                RecordTestFailure($"Total {variableName}count should have been {expectedCount} but was {count}.");
        }

        public static void ThrowsException(Action method)
        {
            _ThrowsException(method, null);
        }

        public static void ThrowsException<T>(Action method)
        {
            _ThrowsException(method, typeof(T));
        }

        private static void _ThrowsException(Action method, Type exceptionType = null)
        {
            string methodName = method.Method.Name.Replace(">b__0", "").Replace("<", "");
            try
            {
                method();
            }
            catch (Exception e)
            {
                if (exceptionType != null && !exceptionType.IsAssignableFrom(e.GetType()))
                    RecordTestFailure($"Method `{methodName}` should have thrown an exception of type `{exceptionType.Name}` but threw an exception of type `{e.GetType().Name}` instead.", GetCallerName(3));

                return;
            }

            RecordTestFailure($"Method `{methodName}` should have thrown an exception, but it did not.", GetCallerName(3));
        }

        private static void RecordTestFailure(string error = null, string testName = null)
        {
            string separator = "\n********************************************************\n";

            string errorMessageToLog = string.Join(" ", $"Test {testName ?? GetCallerName()} failed.\n", error);
            _allErrors.Add(errorMessageToLog);
            Log.RecordError(separator + errorMessageToLog + separator);
        }

        private static string GetCallerName(int stackIndex = 3)
        {
            var method = new StackFrame(stackIndex).GetMethod();
            return $"`{method.DeclaringType.Name}.{method.Name}`";
        }

        internal static void TestRecap()
        {
            string callerClassName = new StackFrame(1).GetMethod().DeclaringType.Name;
            string header = $"\n\n\n\n\n ************ TEST RECAP FOR `{callerClassName}` ************\n";
            Log.RecordNote(header);

            if (_allErrors == null || !_allErrors.Any())
            {
                Log.RecordNote("No errors in tests.");
                Log.RecordNote("\n ************************************");
                return;
            }

            Log.RecordNote("Test failures recap:\n");
            for (int i = 0; i < _allErrors.Count(); i++)
                Log.RecordNote(_allErrors[i]);

            _allErrors = new List<string>();

            Log.RecordNote("\n ************************************");
        }

        private static List<string> _allErrors = new List<string>();


        public class OnlyIEnumContents : BaseTypeComparer
        {
            public OnlyIEnumContents(RootComparer rootComparer) : base(rootComparer)
            {
            }

            public OnlyIEnumContents() : this(RootComparerFactory.GetRootComparer())
            {
            }

            public override bool IsTypeMatch(Type type1, Type type2)
            {
                if (typeof(IEnumerable).IsAssignableFrom(type1) && typeof(IEnumerable).IsAssignableFrom(type2))
                    return true;

                return type1 == type2;
            }

            public override void CompareType(CompareParms parms)
            {
                ListObjectDifferences = new List<Difference>();

                var ienum1 = parms.Object1 as IEnumerable;
                var ienum2 = parms.Object2 as IEnumerable;

                List<Difference> listDiff = new List<Difference>();

                if (ienum1 != null && ienum2 != null)
                {
                    listDiff = ListDiff(ienum1, ienum2);

                    if (listDiff != null && listDiff.Any())
                    {
                        AddDifference(parms);
                        ListObjectDifferences = listDiff;

                        return;
                    }
                }

                CompareLogic compareLogic = new CompareLogic()
                {
                    Config = new KellermanSoftware.CompareNetObjects.ComparisonConfig()
                    {
                        TypesToIgnore = new List<Type>() { typeof(Guid) },
                        DoublePrecision = 1e-6,
                        MaxDifferences = 99
                    }
                };

                ComparisonResult result = compareLogic.Compare(parms.Object1, parms.Object2);

                if (!result.AreEqual)
                {
                    if (result.Differences.Count() == 1)
                    {
                        var firstDiff = result.Differences.FirstOrDefault();

                        if (firstDiff.Object1 is IEnumerable)
                        {
                            ienum1 = firstDiff.Object1 as IEnumerable;
                            ienum2 = firstDiff.Object2 as IEnumerable;

                            listDiff = ListDiff(ienum1, ienum2);

                            if (listDiff == null || !listDiff.Any())
                                return;
                        }
                    }


                    AddDifference(parms);

                    ListObjectDifferences = result.Differences;
                }
            }

            private static List<Difference> ListDiff(IEnumerable ienum1, IEnumerable ienum2)
            {
                List<Difference> listDiff = null;

                if (ienum1 != null && ienum2 != null)
                {
                    List<object> list1 = new List<object>();
                    List<object> list2 = new List<object>();

                    foreach (var item in ienum1)
                        list1.Add(item);

                    foreach (var item in ienum2)
                        list2.Add(item);

                    list1 = list1.Where(li => li != null).ToList();
                    list2 = list2.Where(li => li != null).ToList();

                    // Compare the lists items.
                    // Because all the list elements were boxed in a System.Object,
                    // this only returns the differences between the single elements.
                    CompareLogic compareLogicList = new CompareLogic() { Config = new KellermanSoftware.CompareNetObjects.ComparisonConfig() { MaxDifferences = 99 } };

                    ComparisonResult resultLists = compareLogicList.Compare(list1, list2);

                    // This is where granularity is lost.
                    // The official documentation https://github.com/GregFinzer/Compare-Net-Objects/wiki/Custom-Comparers
                    // points that we should return `AddDifference(parms)`,
                    // however I don't know how to get `parms` from a given `result` object.
                    // Returning a single difference for the entire lists.
                    if (!resultLists.AreEqual)
                    {
                        return resultLists.Differences;
                    }
                }

                return listDiff;
            }

            public List<Difference> ListObjectDifferences { get; set; } = new List<Difference>();
        }
    }
}
