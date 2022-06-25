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


namespace BH.Engine.RDF
{
    public static partial class Modify
    {
        [Description("Split the input string at whitespaces distant at most Nth characters. Join the result with newlines and a specified amount of tab every new line.")]
        public static string SplitInLinesAndTabify(this string longText, int tabAmount = 1, int maxCharsPerSplit = 0)
        {
            if (tabAmount < 0)
                tabAmount = 0;

            if (maxCharsPerSplit < 1)
                maxCharsPerSplit = 100 - (tabAmount - 1) * 30;

            maxCharsPerSplit = maxCharsPerSplit < 15 ? 15 : maxCharsPerSplit;

            string reg = @"(.{1," + maxCharsPerSplit.ToString() + @"})(?:\s|$)|(.{" + maxCharsPerSplit.ToString() + @"})";

            List<string> output = Regex.Split(longText, reg)
                              .Where(x => x.Length > 0)
                              .ToList();

            string newLineJoiner = "\n" + string.Join("", Enumerable.Repeat("\t", tabAmount));

            return string.Join(newLineJoiner, output);
        }
    }
}
