# BHoM to bhOWL Converter

"BHoM to bhOWL Converter" is part of ongoing research by University of Stuttgart and Buro Happold. It is developed as part of a research project of the [Cluster of Excellence Integrative Computational Design and Construction for Architecture (IntCDC)](https://www.intcdc.uni-stuttgart.de/). The project's name is: [Knowledge Representation for Multi-Disciplinary Co-Design of Buildings](https://www.intcdc.uni-stuttgart.de/research/research-projects/rp-20/).

The developed tool helps convert BHoM data to a knowledge graph in any software BHoM supports. As a result, the converter returns a Resource Description Framework (RDF) graph serialized in Terse RDF Triple Language (TTL) format. The toolkit contains a live connector the GraphDB, a highly efficient and robust graph database with RDF and SPARQL support, compliant with W3C standards.

BHoM (The Buildings and Habitats object Model) is a collaborative framework that runs within several AEC design software, and it represents data in an object-oriented database model. OWL (Web Ontology Language) provides a standardized and expressive language for representing knowledge and relationships within a domain. It allows for creating ontologies, which are formal descriptions of the concepts and relationships within a domain. Additionally, OWL can support reasoning and inference over ontologies, allowing for automated reasoning about the relationships between different concepts and data elements. This can be particularly useful in identifying inconsistencies or gaps in data or suggesting additional data sources or mappings that might be needed to support integration efforts.

Our BHoM to bhOWL converter is developed as an extension to the BHoM framework and runs within any software that BHoM supports (eg. Grasshopper 3D, Excel, etc.).The resulting graph contains both the Terminology Layer TBox and the Assertional Layer ABox. The dataset also contains example Grasshopper 3D files that exemplify the process of converting BHoM data to bhOWL.

See installation and update instructions in the [Wiki.](https://github.com/BHoM/RDF_Prototypes/wiki/Installation-and-update-instructions)


## Team

### University of Stuttgart 
RA Ph.D. Cand. Diellza Elshani, Stu. Asst. Aaron Wagner, Tenure-Track Prof. Dr. Thomas Wortmann, Chair for Computing in Architecture, [Institute for Computational Design and Construction (ICD)](https://www.icd.uni-stuttgart.de/), University of Stuttgart 

Dr.rer.nat. Daniel Hernández, Prof. Dr. Steffen Staab, Department for Analytic Computing (AC), [Institute for Parallel and Distributed Systems](https://www.ipvs.uni-stuttgart.de/), University of Stuttgart 

### Buro Happold
Alessio Lombardi, Dr. Al Fisher, Isak Naslund, Computational Development Buro Happold, London, England https://www.burohappold.com/



