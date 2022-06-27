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
            if (!AreObjectsEqual(obj1, obj2))
                RecordTestFailure($"{obj1.GetType().FullName} should have been equal to the other object.");
        }

        public static void IsNotEqual(object obj1, object obj2, string error = null)
        {
            if (AreObjectsEqual(obj1, obj2))
                RecordTestFailure($"{obj1.GetType().FullName} should have been different to the other object.");
        }

        private static bool AreObjectsEqual(object obj1, object obj2)
        {
            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.CustomComparers.Add(new OnlyIEnumContents());
            ComparisonResult result = compareLogic.Compare(obj1, obj2);

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
                var ienum1 = parms.Object1 as IEnumerable;
                var ienum2 = parms.Object2 as IEnumerable;

                if (ienum1 != null && ienum2 != null)
                {
                    List<object> list1 = new List<object>();
                    List<object> list2 = new List<object>();

                    foreach (var item in ienum1)
                        list1.Add(item);

                    foreach (var item in ienum2)
                        list2.Add(item);

                    // Compare the lists. 
                    // Because all the list elements were boxed in a System.Object,
                    // this only returns the differences between the single elements.
                    CompareLogic compareLogic = new CompareLogic();
                    ComparisonResult result = compareLogic.Compare(list1, list2);

                    // This is where granularity is lost.
                    // The official documentation https://github.com/GregFinzer/Compare-Net-Objects/wiki/Custom-Comparers
                    // points that we should return `AddDifference(parms)`,
                    // however I don't know how to get `parms` from a given `result` object.
                    // Returning a single difference for the entire lists.
                    if (!result.AreEqual)
                        AddDifference(parms);
                }
            }
        }
    }
}
