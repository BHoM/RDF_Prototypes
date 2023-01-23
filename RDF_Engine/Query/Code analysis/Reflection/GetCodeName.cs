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

using System;
using System.CodeDom;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace BH.Engine.RDF
{
	public static partial class Query
	{
		[Description("Returns the name of a type as it would be written in a C# code file. E.g. for typeof(System.String) returns 'string'.")]
		public static string GetCodeName(this Type type, bool includeNamespace = false)
		{
            string RoslynPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\roslyn\\csc.exe";
            m_CodeDomProvider = new CSharpCodeProvider(new ProviderOptions(RoslynPath, 0));

            m_CodeDomProvider = m_CodeDomProvider ?? new CSharpCodeProvider(); // cache provider.

			var reference = CreateTypeReference(type);

			string result = m_CodeDomProvider.GetTypeOutput(reference);

			if (includeNamespace)
				result = $"{type.Namespace ?? ""}.{result}";

			return result;
		}

		private static CodeTypeReference CreateTypeReference(Type type)
		{
			string typeName = (type.IsPrimitive || type == typeof(string)) ? type.FullName : type.Name;
			CodeTypeReference reference = new CodeTypeReference(typeName);

			if (type.IsArray)
			{
				reference.ArrayElementType = CreateTypeReference(type.GetElementType());
				reference.ArrayRank = type.GetArrayRank();
			}

			if (type.IsGenericType)
			{
				foreach (var argument in type.GetGenericArguments())
				{
					reference.TypeArguments.Add(CreateTypeReference(argument));
				}
			}

			return reference;
		}

		private static CSharpCodeProvider m_CodeDomProvider = null;
	}
}