using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.oM.Base;
using BH.Engine.Geometry;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static ObjectIdentity ObjectIdentity(this IBHoMObject iObj, ComparisonConfig cc = null)
        {
            if (cc == null)
                cc = new ComparisonConfig();

            ObjectIdentity oi = new ObjectIdentity();
            cc.NamespaceExceptions = cc.NamespaceExceptions ?? new List<string>();
            cc.NamespaceExceptions.Add("Geometry");

            oi.Hash = iObj.Hash(cc);
            oi.IGeometryPoint = iObj.IGeometry().ICentroid();

            return oi;
        }
    }
}
