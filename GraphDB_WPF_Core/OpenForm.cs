using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDB_WindowsFroms
{
    public static class OpenForm
    {
        public static void Run()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
    
}
