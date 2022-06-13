using BH.Engine.RDF;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    public partial class CustomType : Type, IEquatable<Type>
    {
        private Dictionary<string, Type> _propertyTypes = new Dictionary<string, Type>();

        public override Guid GUID { get; }

        public override Module Module { get; }

        public override Assembly Assembly { get; }

        public override string FullName { get; }

        public override string Namespace { get; }

        public override string AssemblyQualifiedName { get; }

        public override Type BaseType { get; }

        public override Type UnderlyingSystemType { get; }

        public override string Name { get; }

        public TBoxSettings TBoxSettings { get; }

        public Uri OntologicalUri { get; }

        public List<string> PropertyNames { get; } = new List<string>();

        public static Type GetCustomTypeIfPossible(object obj, TBoxSettings tBoxSettings)
        {
            if (obj == null)
                return default(Type);

            CustomObject customObj = obj as CustomObject;
            if (customObj == null)
                return obj.GetType();

            if (!customObj.CustomData.ContainsKey(tBoxSettings.CustomobjectsTypeKey))
                return typeof(CustomObject);

            return new CustomType(customObj, tBoxSettings);
        }

        public CustomType(CustomObject customObj, TBoxSettings tBoxSettings)
        {
            object typeNameObj = null;
            if (!customObj.CustomData.TryGetValue(tBoxSettings.CustomobjectsTypeKey, out typeNameObj))
                throw new ArgumentException($"Could not extract the type name for this CustomObject; no value defined for `{typeNameObj}`.");

            Type thisClassType = this.GetType();

            Name = typeNameObj.ToString();

            GUID = ToGuid(Name);
            Module = thisClassType.Module;
            Assembly = thisClassType.Assembly;
            FullName = thisClassType.FullName + $".{Name}";
            Namespace = thisClassType.Namespace;
            AssemblyQualifiedName = thisClassType.AssemblyQualifiedName;
            BaseType = typeof(CustomObject);
            UnderlyingSystemType = typeof(CustomObject);

            if (tBoxSettings == null)
                throw new ArgumentException($"{nameof(TBoxSettings)} was null upon creation of CustomType `{Name}`.");

            TBoxSettings = tBoxSettings;
            OntologicalUri = Query.CombineUris(tBoxSettings.CustomTypesBaseAddress, Name);

            PropertyNames = customObj.CustomData.Keys.Where(k => k != tBoxSettings.CustomobjectsTypeKey).ToList();
            _propertyTypes = customObj.CustomData.Select(cd => new KeyValuePair<string, Type>(cd.Key, GetCustomTypeIfPossible(cd.Value, tBoxSettings))).ToDictionary(cv => cv.Key, cv => cv.Value);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr = BindingFlags.Public)
        {
            List<CustomPropertyInfo> customProps = new List<CustomPropertyInfo>();
            foreach (var item in _propertyTypes)
                customProps.Add(new CustomPropertyInfo(this, item, TBoxSettings));

            return customProps.ToArray();
        }

        private static Guid ToGuid(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return Guid.NewGuid();

            byte[] stringbytes = Encoding.UTF8.GetBytes(source);
            byte[] hashedBytes = new System.Security.Cryptography
                .SHA1CryptoServiceProvider()
                .ComputeHash(stringbytes);
            Array.Resize(ref hashedBytes, 16);
            return new Guid(hashedBytes);
        }

        public bool Equals(CustomType other)
        {
            return null != other && FullName == other.FullName && !PropertyNames.Except(other.PropertyNames).Any();
        }

        public override bool Equals(Type other)
        {
            return Equals(other as CustomType);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CustomType);
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
            throw new NotImplementedException();
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
