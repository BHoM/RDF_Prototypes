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

        public static bool IsEqual(object target, object actual)
        {
            List<CustomDifference> differences = new List<CustomDifference>();

            if (!AreObjectsEqual(target, actual, out differences))
            {
                var differencesText = string.Join("\n", differences.Select(d => d.Description));

                RecordTestFailure($"{target.GetType().FullName} should have been equal to the other object." + (!differences.Any() ? "" : $" Differences:\n\t{differencesText.SplitInLinesAndTabify()}"));

                return false;
            }

            return true;
        }

        public static bool IsNotEqual(object target, object actual)
        {
            List<CustomDifference> differences = new List<CustomDifference>();

            if (AreObjectsEqual(target, actual, out differences))
            {
                RecordTestFailure($"{target.GetType().FullName} should have been different to the other object.");
                return false;
            }

            return true;
        }

        private static bool AreObjectsEqual(object obj1, object obj2, out List<CustomDifference> differences)
        {
            differences = new List<CustomDifference>();

            if (obj1 != null && obj2 == null || obj1 == null)
            {
                differences.Add(new CustomDifference() { Object1 = obj1, Object2 = obj2, Description = "One object was null but the other was not." });


                return false;
            }

            if (obj1.GetType() != obj2.GetType())
            {
                differences.Add(new CustomDifference()
                {
                    Object1TypeName = obj1.GetType().FullName,
                    Object2TypeName = obj2.GetType().FullName,
                    Description = $"Obj1 was of type {obj1.GetType().Name}, while Obj2 was of type {obj2.GetType().Name}."
                });

                return false;
            }

            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.CustomComparers.Add(new OnlyIEnumContents());
            ComparisonResult result = compareLogic.Compare(obj1, obj2);

            if (!result.AreEqual)
            {
                result.Differences.RemoveAll(d => true);
                result.Differences.AddRange(m_nestedDifferences);
            }

            differences = result.Differences.ToBHoM();

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
            Log.RecordDebugInfo(header);

            if (_allErrors == null || !_allErrors.Any())
            {
                Log.RecordDebugInfo("No errors in tests.");
                Log.RecordDebugInfo("\n ************************************");
                return;
            }

            Log.RecordDebugInfo("Test failures recap:\n");
            for (int i = 0; i < _allErrors.Count(); i++)
                Log.RecordDebugInfo(_allErrors[i]);

            _allErrors = new List<string>();

            Log.RecordDebugInfo("\n ************************************");
        }

        private static List<string> _allErrors = new List<string>();


        private class OnlyIEnumContents : BaseTypeComparer
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
                m_nestedDifferences = new List<CustomDifference>();

                var ienum1 = parms.Object1 as IEnumerable;
                var ienum2 = parms.Object2 as IEnumerable;

                List<CustomDifference> listDiff = new List<CustomDifference>();

                if (ienum1 != null && ienum2 != null)
                {
                    listDiff = ListDiff(ienum1, ienum2);

                    if (listDiff != null && listDiff.Any())
                    {
                        AddDifference(parms);
                        m_nestedDifferences = listDiff;

                        return;
                    }
                }

                CompareLogic compareLogic = new CompareLogic()
                {
                    Config = new KellermanSoftware.CompareNetObjects.ComparisonConfig()
                    {
                        TypesToIgnore = new List<Type>() { typeof(Guid) },
                        DoublePrecision = 1e-6,
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

                    m_nestedDifferences = result.Differences.ToBHoM();
                }
            }
        }

        private static List<CustomDifference> ListDiff(IEnumerable ienum1, IEnumerable ienum2)
        {
            List<CustomDifference> listDiff = null;

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
                CompareLogic compareLogicList = new CompareLogic();

                ComparisonResult resultLists = compareLogicList.Compare(list1, list2);

                // This is where granularity is lost.
                // The official documentation https://github.com/GregFinzer/Compare-Net-Objects/wiki/Custom-Comparers
                // points that we should return `AddDifference(parms)`,
                // however I don't know how to get `parms` from a given `result` object.
                // Returning a single difference for the entire lists.
                if (!resultLists.AreEqual)
                {
                    return resultLists.Differences.ToBHoM();
                }
            }

            return listDiff;
        }

        private static List<CustomDifference> m_nestedDifferences = new List<CustomDifference>();

        /*
         * TEST OBJECTS
         * 
         */

        public class CustomDifference : Difference, IObject
        {
            public string Description { get; set; }
        }

        /*
         * TEST_ENGINE METHODS 
         * 
         */

        public static CustomDifference ToBHoM(this Difference difference)
        {
            var customDiff = new CustomDifference();
            CopyPropertiesFromParent(customDiff, difference);

            var differencesText = $"{difference.Object1.GetType().FullName}.{difference.PropertyName} should have been {difference.Object1} but was {difference.Object2}.";
            customDiff.Description = differencesText;

            return customDiff;
        }

        public static List<CustomDifference> ToBHoM(this List<Difference> differences)
        {
            return differences.Select(d => d.ToBHoM()).ToList();
        }

        private static void CopyPropertiesFromParent<P, C>(this C childObject, P parentObject) where C : IObject, P
        {
            if (childObject == null || parentObject == null)
                return;

            Type p = typeof(P);
            Type c = typeof(C);

            var parentProps = p.GetProperties();
            var childProps = c.GetProperties().Where(pp => pp.CanWrite);

            foreach (var childProp in childProps)
            {
                var correspondingParentProp = parentProps.FirstOrDefault(pp => pp.Name == childProp.Name);
                if (correspondingParentProp == null)
                    continue;

                var correspondingParentPropValue = correspondingParentProp.GetValue(parentObject);
                childProp.SetValue(childObject, correspondingParentPropValue);
            }
        }
    }
}
