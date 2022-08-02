using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        private static SHA256 m_SHA256Algorithm = null;

        [Description("Computes a SHA 256 hash from the given string. This Hash method is the same used by BH.Engine.Base.Hash() to return the final hash.")]
        public static string SHA256Hash(this string str)
        {
            byte[] strBytes = ASCIIEncoding.Default.GetBytes(str);

            if (m_SHA256Algorithm == null)
                m_SHA256Algorithm = SHA256.Create();

            byte[] byteHash = m_SHA256Algorithm.ComputeHash(strBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteHash)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
