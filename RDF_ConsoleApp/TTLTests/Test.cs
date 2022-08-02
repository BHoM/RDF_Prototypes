using BH.oM.RDF;

namespace BH.Test.RDF
{
    /// <summary>
    /// Base class for tests, with customised settings.
    /// </summary>
    public abstract partial class Test
    {
        public static LocalRepositorySettings m_localRepositorySettings = new LocalRepositorySettings()
        {
            GitRootPath = @"C:\Users\alombardi\GitHub"
        };

        public static OntologySettings m_ontologySettings = new OntologySettings()
        {
            ABoxSettings = new ABoxSettings() { IndividualsBaseAddress = "individuals.Address" },
            TBoxSettings = new TBoxSettings() { CustomObjectTypesBaseAddress = "CustomObjectTypes.Address" }
        };
    }
}
