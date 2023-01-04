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

using BH.Engine.RDF;
using BH.oM.Base;
using BH.oM.RDF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF.Types
{
    /// <summary>
    /// Type used to represent the type of BH.oM.Base.CustomObject in a CSharpGraph.
    /// CustomObjects are required to be interpreted as their own Type, called like the value stored in a specific Key of their CustomData dictionary.
    /// This is so that users can create their own "Types" when creating CustomObjects.
    /// These CustomObjectType can then be part of a CSharpGraph and be translated to an ontology format
    /// using the same algorithm that deals with any other CSharp Type extracted from BHoM (or other sources).
    /// </summary>
    public partial class CustomObjectType : ICustomRDFType
    {
        // ************************************ //
        // Public properties                    //
        // ************************************ //

        private Dictionary<string, Type> _propertyTypes = new Dictionary<string, Type>();

        public override Guid GUID { get; }

        public override Module Module { get; }

        public override Assembly Assembly { get; }

        public override string FullName { 
            get; 
        }

        public override string Namespace { get; }

        public override string AssemblyQualifiedName { get; }

        public override Type BaseType { get; }

        public override Type UnderlyingSystemType { get; }

        public override string Name { 
            get; 
        }

        public new TBoxSettings TBoxSettings { get; }

        public List<string> PropertyNames { get; } = new List<string>();


        // ************************************ //
        // Ctor                                 //
        // ************************************ //

        public CustomObjectType(string objectTypeName) : this(new CustomObject() { CustomData = new Dictionary<string, object>() { { new TBoxSettings().CustomobjectsTypeKey, objectTypeName } } }, new TBoxSettings())
        {

        }

        public CustomObjectType(CustomObject customObj, TBoxSettings tBoxSettings)
        {
            object typeNameObj = null;
            if (!customObj.CustomData.TryGetValue(tBoxSettings.CustomobjectsTypeKey, out typeNameObj))
                throw new ArgumentException($"Could not extract the type name for this CustomObject; no value defined for `{typeNameObj}`.");

            Type thisClassType = this.GetType();
            
            Name = typeNameObj.ToString();
            RDFTypeName = Name;

            GUID = Query.GuidFromString(Name);
            Module = thisClassType.Module;
            Assembly = thisClassType.Assembly;
            FullName = Name; // do not prepend this class' namespace or name. Not useful.
            Namespace = thisClassType.Namespace;
            AssemblyQualifiedName = Name;
            BaseType = typeof(CustomObject);
            UnderlyingSystemType = typeof(CustomObject);

            if (tBoxSettings == null)
                throw new ArgumentException($"{nameof(TBoxSettings)} was null upon creation of CustomObjectType `{Name}`.");

            TBoxSettings = tBoxSettings;

            PropertyNames = customObj.CustomData.Keys.Where(k => k != tBoxSettings.CustomobjectsTypeKey).ToList();
            _propertyTypes = customObj.CustomData.Select(cd => new KeyValuePair<string, Type>(cd.Key, TryGetCustomObjectType(cd.Value, tBoxSettings))).ToDictionary(cv => cv.Key, cv => cv.Value);
        }

        // ************************************ //
        // Public methods                       //
        // ************************************ //

        /// <summary>
        /// If the input object is a BH.oM.Base.CustomObject with a Type key, return a CustomObjectType for it.
        /// Otherwise, return the result of the GetType() on the input object.
        /// </summary>
        public static Type TryGetCustomObjectType(object obj, TBoxSettings tBoxSettings)
        {
            if (obj == null)
                return default(Type);

            CustomObject customObj = obj as CustomObject;
            if (customObj == null)
                return obj.GetType();

            if (!customObj.CustomData.ContainsKey(tBoxSettings.CustomobjectsTypeKey))
                return typeof(CustomObject);

            return new CustomObjectType(customObj, tBoxSettings);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr = BindingFlags.Public)
        {
            List<CustomPropertyInfo> customProps = new List<CustomPropertyInfo>();
            foreach (var item in _propertyTypes)
                customProps.Add(new CustomPropertyInfo(this, item));

            return customProps.ToArray();
        }

        public bool Equals(CustomObjectType other)
        {
            return null != other && FullName == other.FullName && !PropertyNames.Except(other.PropertyNames).Any();
        }

        public override bool Equals(Type other)
        {
            return Equals(other as CustomObjectType);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CustomObjectType);
        }

        public override int GetHashCode()
        {
            string stringKey = FullName + "_" + string.Join("-", PropertyNames);
            byte[] strBytes = ASCIIEncoding.Default.GetBytes(stringKey);
            int intKey = BitConverter.ToInt32(strBytes, 0);

            return intKey;
        }

        // ------------------------------------------------------------------------ //
        // Not implemented stuff, required for inheritance but otherwise not useful //
        // ------------------------------------------------------------------------ //

        #region NotImplementedStuff

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            throw new NotImplementedException();
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            throw new NotImplementedException();
        }

        public override Type[] GetInterfaces()
        {
            return typeof(CustomObject).GetInterfaces();
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return TypeAttributes.Public;
        }

        protected override bool IsArrayImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsByRefImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsPointerImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsPrimitiveImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsCOMObjectImpl()
        {
            throw new NotImplementedException();
        }

        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }

        protected override bool HasElementTypeImpl()
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
