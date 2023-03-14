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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing;
using BH.Engine.Base;
using BH.oM.Adapters.RDF;
using BH.oM.Base.Attributes;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Linq;
using BH.Engine.Adapters.RDF;

namespace BH.Engine.Adapters.TTL
{
    public static partial class Convert
    {
        [Description("Reads a TTL ontology and attempts to convert any A-Box individual into its CSharp object equivalent.")]
        [MultiOutputAttribute(0,"KG","Ontology produced whatever")]
        [MultiOutputAttribute(1, "OS", "Ontology Settings used to construct this KG")]
        public static Output<List<object>, OntologySettings> FromTTL(string TTLtext)
        {
            if (string.IsNullOrWhiteSpace(TTLtext))
                return new Output<List<object>, OntologySettings>();

            OntologySettings ontologySettings = ExtractOntologySettings(TTLtext);

            Output<List<object>, OntologySettings> output = new Output<List<object>, OntologySettings>
            {
                Item1 = BH.Engine.Adapters.RDF.Convert.ToCSharpObjects(TTLtext),
                Item2 = ontologySettings
            };
            return output;
        }

        [Description("Reads a TTL ontology and attempts to convert any A-Box individual into its CSharp object equivalent.")]
        public static Output<List<object>, OntologySettings> FromTTL(string TTLfilePath, bool active = false)
        {
            if (!active)
                return new Output<List<object>, OntologySettings>();

            string TTLtext = File.ReadAllText(TTLfilePath);
            Output<List<object>, OntologySettings> readTTLOutput = FromTTL(TTLtext);

            return readTTLOutput;
        }

        private static OntologySettings ExtractOntologySettings(string TTLtext)
        {
            string ontologySettingsDeclaration = $"# {nameof(OntologySettings)}: ";

            foreach (var line in TTLtext.SplitToLines())
            {
                if (line.Contains(ontologySettingsDeclaration))
                    return BH.Engine.Adapters.RDF.Convert.FromBase64JsonSerialized(line.Replace(ontologySettingsDeclaration, "")) as OntologySettings ?? new OntologySettings();
            }

            return new OntologySettings();
        }

        private static IEnumerable<string> SplitToLines(this string input)
        {
            if (input == null)
            {
                yield break;
            }

            using (System.IO.StringReader reader = new System.IO.StringReader(input))
            {
                string line;
                while ((line = reader?.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
