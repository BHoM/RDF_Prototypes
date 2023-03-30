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

using BH.Adapter;
using BH.Engine.Adapters.RDF;
using BH.oM.Adapter;
using BH.oM.Base.Attributes;
using BH.oM.Adapters.RDF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Ontology;
using BH.Adapters;
using BH.oM.Data.Requests;
using System.IO;

namespace BH.Adapters.Markdown
{
    public partial class MarkdownAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("Adapter for Markdown.")]
        [Input("filepath","The filepath where the Markdown will be saved.")]
        [Output("The created TTL adapter.")]
        public MarkdownAdapter(string filepath, GraphSettings graphSettings = null, LocalRepositorySettings localRepositorySettings = null)
        {
            m_AdapterSettings.DefaultPushType = oM.Adapter.PushType.CreateOnly; // Adapter `Push` Action simply calls "Create" method.

            if (string.IsNullOrWhiteSpace(filepath) || !IsValidPath(filepath))
            {
                Log.RecordError("Invalid filepath specified.");
            }

            m_filepath = filepath;
            m_graphSettings = graphSettings ?? new GraphSettings();
            m_localRepositorySettings = localRepositorySettings;
        }

        private readonly string m_filepath;
        private GraphSettings m_graphSettings = new GraphSettings();
        private readonly LocalRepositorySettings m_localRepositorySettings;

        public override IEnumerable<object> Pull(IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig actionConfig = null)
        {
            Log.RecordWarning($"The {nameof(MarkdownAdapter)} is designed only for the Push action.");
            return new List<object>();
        }

        private bool IsValidPath(string path, bool allowRelativePaths = false)
        {
            bool isValid = true;

            try
            {
                string fullPath = Path.GetFullPath(path);

                if (allowRelativePaths)
                {
                    isValid = Path.IsPathRooted(path);
                }
                else
                {
                    string root = Path.GetPathRoot(path);
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                }
            }
            catch (Exception ex)
            {
                isValid = false;
            }

            return isValid;
        }
    }
}