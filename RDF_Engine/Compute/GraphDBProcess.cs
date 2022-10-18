using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        [Description("Input filepath to GraphDB.exe on your machine. To start GraphDB switch toggle to True, to close GraphDB switch toggle to False.")]

        public static void GraphDBProcess(string GraphDBfilePath,bool run)
        {
            
            if (run == true)
            {
                Process.Start(GraphDBfilePath);
                //Task.Delay(500).Wait();

            }
            else if (run == false & Process.GetProcessesByName("GraphDB Desktop").Any())
            {
                Process[] processes = Process.GetProcessesByName("GraphDB Desktop");
                foreach (var process in processes)
                {
                    process.Kill();
                }
            }

        }


    }
       
}
