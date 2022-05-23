using BH.Engine.RDF;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace BH.oM.RDF
{
    public class CustomPropertyInfo : PropertyInfo
    {
        public override Type PropertyType { get; }

        public override PropertyAttributes Attributes { get; }

        public override bool CanRead { get; } = true;

        public override bool CanWrite { get; } = true;

        public override string Name { get; }

        public override Type DeclaringType { get; }

        public override Type ReflectedType => throw new NotImplementedException();

        public TBoxSettings TBoxSettings { get; }

        public Uri OntologicalUri { get; }

        public CustomPropertyInfo(CustomType declaringCustomType, KeyValuePair<string, Type> propertyTypes, TBoxSettings tBoxSettings)
        {
            Name = propertyTypes.Key;
            PropertyType = propertyTypes.Value;
            TBoxSettings = tBoxSettings;
            DeclaringType = declaringCustomType;

            // The ontological uri of this property is the Ontological Uri of the parent type
            // followed by an hashtag and the name of this property.
            OntologicalUri = Query.CombineUris(declaringCustomType.OntologicalUri + $"#{Name}");
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
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

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            CustomObject co = obj as CustomObject;
            if (co == null)
                throw new ArgumentException($"The input object must be a {nameof(CustomObject)}.");

            object value = null;
            if (!co.CustomData.TryGetValue(this.Name, out value))
                throw new ArgumentException($"The input {nameof(CustomObject)} does not have a {this.Name} key.");

            return value;
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            CustomObject co = obj as CustomObject;
            if (co == null)
                throw new ArgumentException($"The input object must be a {nameof(CustomObject)}.");

            co.CustomData[this.Name] = value;
        }
    }
}
