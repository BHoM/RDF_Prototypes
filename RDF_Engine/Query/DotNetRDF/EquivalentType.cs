/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Ontology;

namespace BH.Engine.Adapters.RDF
{
    public static partial class Query
    {
        [Description("Finds the equivalent System.Type for the given individual's class.")]
        public static Type EquivalentType(this OntologyResource individual)
        {
            Uri bhomUri = null;
            string bhomTypeFullName = null;

            // Get the equivalent type
            foreach (var type in individual.Types)
            {
                try
                {
                    bhomUri = type.AbsoluteUri();
                    bhomTypeFullName = bhomUri?.Segments.LastOrDefault();

                    if (!bhomTypeFullName.StartsWith("BH."))
                        continue;
                }
                catch { }
            }

            if (string.IsNullOrEmpty(bhomTypeFullName))
                return default(Type);

            string customTypeName = bhomTypeFullName.Split('.').Last();

            // Check if it is a custom type.
            if (bhomTypeFullName.Contains(typeof(BH.Engine.Adapters.RDF.Types.CustomObjectType).FullName))
                return new Types.CustomObjectType(customTypeName);

            // See if a match can be found in the complete Type list of BHoM.
            Type bhomType = BH.Engine.Base.Query.AllTypeList().Where(t => t.FullName == bhomTypeFullName).FirstOrDefault();
            if (bhomType != null)
                return bhomType;

            // See if the type can be found among the currently loaded types,
            // to see if there is a corresponding non-BHoM type for this class.
            // Because the type could have been stored simply with a custom name (not a FullName that includes the namespace/assembly info),
            // we must limit this search by using the type name only. 
            IEnumerable<Type> allLoadedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.TryGetTypes()).Except(BH.Engine.Base.Query.AllTypeList()).Where(t => t.Name == customTypeName);

            try
            {
                if (allLoadedTypes.Count() == 1)
                    bhomType = allLoadedTypes.FirstOrDefault();
            } catch(ReflectionTypeLoadException e)
            {
                Log.RecordWarning($"Could not load some of the types. Error: \n\t{string.Join("\n\t", e.LoaderExceptions.Select(ex => ex.Message))}");
            }

            if (bhomType == null)
            {
                // If no match was found, this is likely a Custom Type that was not correctly detected in the previous steps,
                // or a non-bhom type that was not found in the current Assembly Domain.
                // Regardless, we want to return this as a CustomObject. To be consistent with the rest of the workflows,
                // we return a Custom Type, which later will be translated as a CustomObject.
                Log.RecordNote($"Could not find equivalent Type for the class `{bhomTypeFullName}`. Returning it as a {typeof(BH.oM.Base.CustomObject).Name}.");
                return new Types.CustomObjectType(customTypeName);
            }

            return bhomType;
        }
    }
}
