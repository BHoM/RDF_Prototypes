/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

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
using FluentAssertions;

namespace BH.Test.RDF
{
    /// <summary>
    /// Class to interface with Unit Tests.
    /// </summary>
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

        public static void IsEqual(object target, object actual)
        {
            actual.Should().BeEquivalentTo(target);
        }

        public static void IsNotEqual(object target, object actual)
        {
            actual.Should().NotBeEquivalentTo(target);
        }


        public static void IsTrue(bool value, string error = null)
        {
            if (!value)
                RecordTestFailure(error);
        }

        public static void Single<T>(this IEnumerable<T> iList, string variableName = null)
        {
            iList.TotalCount(1, variableName);
        }

        public static void Single<T>(this IEnumerable<T> iList, object obj, string variableName = null)
        {
            iList.Where(o => o.Equals(obj)).TotalCount(1, variableName);
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
            NUnit.Framework.Assert.Fail(error);
        }

        private static string GetCallerName(int stackIndex = 3)
        {
            var method = new StackFrame(stackIndex).GetMethod();
            return $"`{method.DeclaringType.Name}.{method.Name}`";
        }
    }
}
