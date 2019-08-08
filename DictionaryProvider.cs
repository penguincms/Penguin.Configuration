using System;
using System.Collections.Generic;
using System.Text;
using Penguin.Configuration.Abstractions;

namespace Penguin.Configuration
{
    public class DictionaryProvider : IProvideConfigurations
    {
        /// <summary>
        /// The underlying configuration source
        /// </summary>
        public Dictionary<string, string> AllConfigurations { get; protected set; }

        /// <summary>
        /// The underlying connection string source
        /// </summary>
        public Dictionary<string, string> AllConnectionStrings { get; protected set; }

        /// <summary>
        /// If true, will throw an error instead of returning null on a missing key
        /// </summary>
        public bool ErrorOnMissingKey { get; set; }

        /// <summary>
        /// A configuration provider that uses dictionaries as the data source for configurations and connection strings
        /// </summary>
        /// <param name="Configurations">The dictionary to use for configurations</param>
        /// <param name="ConnectionStrings">The optional dictionary to use for connection strings</param>
        /// <param name="errorOnMissingKey">If true, will throw an error instead of returning null on a missing key</param>
        public DictionaryProvider(Dictionary<string, string> Configurations, Dictionary<string, string> ConnectionStrings = null, bool errorOnMissingKey = false)
        {
            AllConfigurations = Configurations;
            AllConnectionStrings = ConnectionStrings ?? new Dictionary<string, string>();
            ErrorOnMissingKey = errorOnMissingKey;
        }

        /// <summary>
        /// A configuration provider that uses dictionaries as the data source for configurations and connection strings
        /// </summary>
        /// <param name="Configurations">The dictionary to use for configurations</param>
        /// <param name="errorOnMissingKey">If true, will throw an error instead of returning null on a missing key</param>
        public DictionaryProvider(Dictionary<string, string> Configurations, bool errorOnMissingKey = false)
        {
            AllConfigurations = Configurations;
            AllConnectionStrings =  new Dictionary<string, string>();
            ErrorOnMissingKey = errorOnMissingKey;
        }

        /// <summary>
        /// Gets a configuration from the underlying data store
        /// </summary>
        /// <param name="Key">The configuration key</param>
        /// <returns>The configuration value</returns>
        public string GetConfiguration(string Key)
        {
            if(AllConfigurations.ContainsKey(Key))
            {
                return AllConfigurations[Key];
            } else if (ErrorOnMissingKey)
            {
                throw new KeyNotFoundException($"The requested configuration {Key} was not found in the underlying dictionary");
            } else {
                return null;
            }
        }

        /// <summary>
        /// Gets a connection string from the underlying data store
        /// </summary>
        /// <param name="Name">The connection string name</param>
        /// <returns>The connection string value value</returns>
        public string GetConnectionString(string Name)
        {
            if (AllConnectionStrings.ContainsKey(Name))
            {
                return AllConnectionStrings[Name];
            }
            else if (ErrorOnMissingKey)
            {
                throw new KeyNotFoundException($"The requested connection string {Name} was not found in the underlying dictionary");
            }
            else
            {
                return null;
            }
        }
    }
}
