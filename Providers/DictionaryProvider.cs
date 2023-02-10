using Penguin.Configuration.Abstractions.Interfaces;
using System.Collections.Generic;

namespace Penguin.Configuration.Providers
{
    /// <summary>
    /// Wraps a dictionary (and optional connection string representation) so it can be used as a source of configurations
    /// </summary>
    public class DictionaryProvider : IProvideConfigurations
    {
        /// <summary>
        /// The underlying configuration source
        /// </summary>
        public Dictionary<string, string> AllConfigurations { get; }

        /// <summary>
        /// The underlying connection string source
        /// </summary>
        public Dictionary<string, string> AllConnectionStrings { get; }

        bool IProvideConfigurations.CanWrite => true;

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
            AllConnectionStrings = new Dictionary<string, string>();
            ErrorOnMissingKey = errorOnMissingKey;
        }

        /// <summary>
        /// Gets a configuration from the underlying data store
        /// </summary>
        /// <param name="Key">The configuration key</param>
        /// <returns>The configuration value</returns>
        public string GetConfiguration(string Key)
        {
            return AllConfigurations.TryGetValue(Key, out string value)
                ? value
                : ErrorOnMissingKey
                    ? throw new KeyNotFoundException($"The requested configuration {Key} was not found in the underlying dictionary")
                    : null;
        }

        /// <summary>
        /// Gets a connection string from the underlying data store
        /// </summary>
        /// <param name="Name">The connection string name</param>
        /// <returns>The connection string value value</returns>
        public string GetConnectionString(string Name)
        {
            return AllConnectionStrings.TryGetValue(Name, out string value)
                ? value
                : ErrorOnMissingKey
                    ? throw new KeyNotFoundException($"The requested connection string {Name} was not found in the underlying dictionary")
                    : null;
        }
        /// <inheritdoc/>

        public bool SetConfiguration(string Name, string Value)
        {
            if (AllConfigurations.ContainsKey(Name))
            {
                AllConfigurations[Name] = Value;
            }
            else
            {
                AllConfigurations.Add(Name, Value);
            }

            return true;
        }
    }
}