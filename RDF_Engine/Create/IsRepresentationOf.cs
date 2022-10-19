using BH.oM.Base;
using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IsRepresentationOf IsRepresentationOf(List<IBHoMObject> set1, List<IBHoMObject> set2, ComparisonConfig cc = null)
        {
            IsRepresentationOf isRepresentationOf = new IsRepresentationOf();
            isRepresentationOf.Set1 = set1.Select(o => o.ObjectIdentity(cc)).ToList();
            isRepresentationOf.Set2 = set2.Select(o => o.ObjectIdentity(cc)).ToList();

            return isRepresentationOf;
        }
    }
}