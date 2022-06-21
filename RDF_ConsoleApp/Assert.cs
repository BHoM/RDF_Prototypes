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

namespace BH.Test.RDF
{
    // Class that performs Unit Test checks, necessary because I could not get any Unit Testing library to work,
    // likely due to .NET incompatibilities.
    // If we can get other unit testing libraries to work, calls to this class can simply be replaced.
    public static partial class Assert
    {
        public static void IsNotNull(object value, string error = null)
        {
            if (value == null)
                RecordTestFailure(error);
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

        private static string GetCallerName(int stackIndex = 1)
        {
            string caller = new StackFrame(stackIndex).GetMethod().Name;
            return caller;
        }

        internal static void TestRecap()
        {
            string header = "\n\n\n\n\n ************ TEST RECAP ************\n";
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
    }
}
