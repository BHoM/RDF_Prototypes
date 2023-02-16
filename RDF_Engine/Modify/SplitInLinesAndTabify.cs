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
using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VDS.RDF;


namespace BH.Engine.Adapters.RDF
{
    public static partial class Modify
    {
        [Description("Split the input string at whitespaces distant at most Nth characters. Join the result with newlines and a specified amount of tab every new line.")]
        public static string SplitInLinesAndTabify(this string longText, int tabAmount = 1, int maxCharsPerSplit = 0)
        {
            string tab = "    ";

            if (string.IsNullOrWhiteSpace(longText))
                return longText;

            if (tabAmount < 0)
                tabAmount = 0;

            if (maxCharsPerSplit < 1)
                maxCharsPerSplit = 115 - (tabAmount - 1) * 30;

            maxCharsPerSplit = maxCharsPerSplit < 15 ? 15 : maxCharsPerSplit;

            string reg = @"(.{1," + maxCharsPerSplit.ToString() + @"})(?:\s|$)|(.{" + maxCharsPerSplit.ToString() + @"})";

            List<string> output = Regex.Split(longText, reg)
                              .Where(x => x.Length > 0)
                              .ToList();

            var tabs = string.Join("", Enumerable.Repeat(tab, tabAmount));
            string newLineJoiner = "\n" + tabs;

            return tabs + string.Join(newLineJoiner, output);
        }
    }
}
