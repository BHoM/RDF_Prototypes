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
    public static partial class Create
    {
        public static List<string> TTLAnnotationProperties()
        {
            List<string> annotationProperties = new List<string>();

            annotationProperties.Add("###  http://purl.org/dc/elements/1.1/#description\n<http://purl.org/dc/elements/1.1/#description> rdf:type owl:AnnotationProperty .");
            annotationProperties.Add("###  http://purl.org/dc/elements/1.1/#title\n<http://purl.org/dc/elements/1.1/#title> rdf:type owl:AnnotationProperty .");

            return annotationProperties;
        }
    }
}
