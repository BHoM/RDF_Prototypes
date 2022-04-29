using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
	public static partial class Query
	{
		[Description("Returns the name of a type as it would be written in a C# code file. E.g. for typeof(System.String) returns 'string'.")]
		public static string GetCodeName(this Type type, bool includeNamespace = false)
		{
			m_CodeDomProvider = m_CodeDomProvider ?? CodeDomProvider.CreateProvider("CSharp"); ; // cache provider.

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

		private static CodeDomProvider m_CodeDomProvider = null;
	}
}