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
    public abstract class ICustomRDFType : Type, IEquatable<Type>
    {
        public string RDFTypeName { get; protected set; }

        public Uri OntologicalUri { get; protected set; }

        public TBoxSettings TBoxSettings { get; protected set; }
    }
}
