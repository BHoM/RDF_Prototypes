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

using BH.oM.Base;
using BH.oM.RDF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Convert
    {
        public static string ToBase64JsonSerialized(this object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            string serializedValue = JsonConvert.SerializeObject(obj, obj.GetType(), settings);

            serializedValue = JsonExtensions.Convert(obj);

            // Encode to base64 to avoid escaping quote problems
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(serializedValue);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static object FromBase64JsonSerialized(this string plainTextBytes)
        {
            // Encode to base64 to avoid escaping quote problems
            byte[] bytes = System.Convert.FromBase64String(plainTextBytes);
            string serializedValue = System.Text.Encoding.UTF8.GetString(bytes);


            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            object decryptedObj = JsonConvert.DeserializeObject(serializedValue, typeof(object), settings);

            decryptedObj = JsonExtensions.UnConvert<object>(serializedValue);

            return decryptedObj;
        }

        // ------------------------------------------- //

        private abstract class TypeWrapper
        {
            // Taken from this answer https://stackoverflow.com/a/38340375/3744182
            // To https://stackoverflow.com/questions/38336390/deserialize-dictionarystring-object-with-enum-values-in-c-sharp
            // By https://stackoverflow.com/users/3744182/dbc
            protected TypeWrapper() { }

            [JsonIgnore]
            public abstract object ObjectValue { get; }

            public static TypeWrapper CreateWrapper<T>(T value)
            {
                if (value == null)
                    return new TypeWrapper<T>();
                var type = value.GetType();
                if (type == typeof(T))
                    return new TypeWrapper<T>(value);
                // Return actual type of subclass
                return (TypeWrapper)Activator.CreateInstance(typeof(TypeWrapper<>).MakeGenericType(type), value);
            }
        }

        private sealed class TypeWrapper<T> : TypeWrapper
        {
            public TypeWrapper() : base() { }

            public TypeWrapper(T value)
                : base()
            {
                this.Value = value;
            }

            public override object ObjectValue { get { return Value; } }

            public T Value { get; set; }
        }

        // ------------------------------------------- //

        internal static class JsonExtensions
        {
            static readonly IContractResolver globalResolver = new JsonSerializer().ContractResolver;

            public static string Convert<T>(T cacheObject)
            {
                return JsonConvert.SerializeObject(ToTypeWrapperIfRequired(cacheObject), Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
            }

            public static T UnConvert<T>(string json)
            {
                var obj = UnConvert(json);
                if ((obj is TypeWrapper wrapper))
                    if (wrapper.ObjectValue is JObject)
                        return UnConvert<T>(wrapper.ObjectValue.ToString());
                    else
                        return (T)wrapper.ObjectValue;
                return (T)obj;
            }

            public static object UnConvert(string json)
            {
                return JsonConvert.DeserializeObject<object>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
            }

            static object ToTypeWrapperIfRequired<T>(T obj, IContractResolver resolver = null)
            {
                // Type information is redundant for string or bool
                if (obj is bool || obj is string)
                    return obj;

                return TypeWrapper.CreateWrapper(obj);
            }
        }
    }
}
