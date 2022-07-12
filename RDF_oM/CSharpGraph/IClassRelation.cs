using BH.oM.Base;
using System.ComponentModel;
using System.Reflection;

namespace BH.oM.RDF
{
    [Description("Base abstract class for other classes representing either Object Properties or Data Properties in a CSharpGraph.")]
    public abstract class IClassRelation // We do not want to implement the IObject interface on this type: no need to expose this to the UI, other than as an output from an `Explode`d CSharpGraph.
    {
        // CSharp PropertyInfos can be seen as the correspondant to Ontology Object Properties.
        public PropertyInfo PropertyInfo { get; set; }

        public override bool Equals(object obj)
        {
            IClassRelation clRel = obj as IClassRelation;
            if (clRel == null || this.GetType() != obj.GetType())
                return false;

            return FullNameValidChars(PropertyInfo) == FullNameValidChars(clRel.PropertyInfo);
        }

        public override int GetHashCode()
        {
            return FullNameValidChars(PropertyInfo).GetHashCode();
        }

        private static string FullNameValidChars(PropertyInfo pi)
        {
            return $"{pi.DeclaringType.FullName}.{pi.Name}";
        }
    }
}
