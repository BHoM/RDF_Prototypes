using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.RDF;
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
        public static string TTLHeader(this OntologySettings ontologySettings, string ontologyTitle, string ontologyDescription, string ontologyAddress, 
            bool includeOwl = true, 
            bool includeRdf = true, bool includeRdfs = true, 
            bool includeXml = true, bool includeXsd = true)
        {
            string header = $"@prefix : <{ontologyAddress}/> .";
            if (includeOwl) header += "\n@prefix owl: <http://www.w3.org/2002/07/owl#> .";
            if (includeRdf) header += "\n@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .";
            if (includeXml) header += "\n@prefix xml: <http://www.w3.org/XML/1998/namespace> .";
            if (includeXsd) header += "\n@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .";
            if (includeRdfs) header += "\n@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .";
            if (includeRdfs) header += "\n@prefix dc: <http://purl.org/dc/elements/1.1/> .";


            
            header += "\n@base   " + $@"<{ontologyAddress}> .";
            

            header += "\n";

            header += "\n"+$@"<{ontologyAddress}> rdf:type owl:Ontology;
                          dc:title ""{ontologyTitle}""@en;
                          dc:description ""{ontologyDescription}""@en.";



            //header += "\n# TypeUris: " + $@"{string.Join(";", ontologySettings.TBoxSettings.TypeUris.Select(KV => KV.Value.ToString() + "," + KV.Key.ToString()))}";
            


            return header;
        }
    }
}
