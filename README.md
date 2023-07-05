# RDF_Prototypes
## Installation instructions
See installation and update instructions in the [Wiki](https://github.com/BHoM/RDF_Prototypes/wiki/Installation-and-update-instructions).

## Description
This repo contains a set of tools to interoperate between BHoM and Resource Description Framework (RDF) data. These tools are part of a research and development effort between BHoM and the [Cluster of Excellence Integrative Computational Design and Construction for Architecture (IntCDC)](https://www.intcdc.uni-stuttgart.de/). The project's name is: [Knowledge Representation for Multi-Disciplinary Co-Design of Buildings](https://www.intcdc.uni-stuttgart.de/research/research-projects/rp-20/).

This repository contains several tools, listed below.

### TTL_Adapter
This adapter converts BHoM objects and classes to a knowledge graph in Terse RDF Triple Language (TTL) format. 
This supports writing to and reading from TTL files.

A graph obtained with BHoM objects and classes is contains both the Terminology Layer (TBox) and the Assertional Layer (ABox).

### GraphDB_Adapter
The toolkit contains an adapter that connects to GraphDB, a graph database with RDF and SPARQL support.

BHoM (The Buildings and Habitats object Model) is a collaborative framework that runs within several AEC design software, and it represents data in an object-oriented database model. OWL (Web Ontology Language) provides a standardized and expressive language for representing knowledge and relationships within a domain. It allows for creating ontologies, which are formal descriptions of the concepts and relationships within a domain. Additionally, OWL can support reasoning and inference over ontologies, allowing for automated reasoning about the relationships between different concepts and data elements. This can be particularly useful in identifying inconsistencies or gaps in data or suggesting additional data sources or mappings that might be needed to support integration efforts.

### Examples
The repository also contains [Grasshopper examples](https://github.com/BHoM/RDF_Prototypes/tree/main/Grasshopper%20examples) that illustrate how to convert BHoM data to/from TTL and GraphDB.

## Team

### University of Stuttgart 
RA Ph.D. Cand. Diellza Elshani, Stu. Asst. Aaron Wagner, Tenure-Track Prof. Dr. Thomas Wortmann, Chair for Computing in Architecture, [Institute for Computational Design and Construction (ICD)](https://www.icd.uni-stuttgart.de/), University of Stuttgart 

Dr.rer.nat. Daniel Hernández, Prof. Dr. Steffen Staab, Department for Analytic Computing (AC), [Institute for Parallel and Distributed Systems](https://www.ipvs.uni-stuttgart.de/), University of Stuttgart 

### Buro Happold
Alessio Lombardi, Dr. Al Fisher, Isak Naslund, Computational Development Buro Happold, London, England https://www.burohappold.com/



