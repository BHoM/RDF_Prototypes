using BH.oM.Base;
using BH.oM.RDF;
using Newtonsoft.Json;
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
    public static partial class Query
    {
        [Description("Returns a Guid based on the SHA1 hash of the input string.")]
        public static Guid GuidFromString(string source)
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
    }
}
