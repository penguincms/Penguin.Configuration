using Penguin.Configuration.Abstractions.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.Configuration.Providers
{
    /// <summary>
    /// Concats an ordered list of configuration providers into a single configuration provider with precidence determined by the order the children are sorted in when constructing
    /// </summary>
    public class ConfigurationProviderList : IProvideConfigurations
    {
        /// <summary>
        /// Returns a dictionary representing all distinct configurations with value determined by child order
        /// </summary>
        public Dictionary<string, string> AllConfigurations
        {
            get
            {
                Dictionary<string, string> toReturn = new Dictionary<string, string>();

                foreach (IProvideConfigurations provider in Providers)
                {
                    foreach (KeyValuePair<string, string> config in provider.AllConfigurations)
                    {
                        if (!toReturn.ContainsKey(config.Key))
                        {
                            toReturn.Add(config.Key, config.Value);
                        }
                    }
                }

                return toReturn;
            }
        }

        /// <summary>
        /// Returns a dictionary representing all distinct connection strings with value determined by child order
        /// </summary>
        public Dictionary<string, string> AllConnectionStrings
        {
            get
            {
                Dictionary<string, string> toReturn = new Dictionary<string, string>();

                foreach (IProvideConfigurations provider in Providers)
                {
                    foreach (KeyValuePair<string, string> config in provider.AllConnectionStrings)
                    {
                        if (!toReturn.ContainsKey(config.Key))
                        {
                            toReturn.Add(config.Key, config.Value);
                        }
                    }
                }

                return toReturn;
            }
        }

        public bool CanWrite => this.Providers.Any(p => p.CanWrite);

        /// <summary>
        /// The child configurations used when constructing this instance
        /// </summary>
        public IEnumerable<IProvideConfigurations> Providers { get; protected set; }

        /// <summary>
        /// Constructs a new instance of this configuration provider
        /// </summary>
        /// <param name="providers">An ordered list of children to use when constructing this object, with the most important first</param>
        public ConfigurationProviderList(params IProvideConfigurations[] providers)
        {
            Providers = providers;
        }

        /// <summary>
        /// Returns the first non-null value of a configuration from the list of children
        /// </summary>
        /// <param name="Key">The key of the configuration to use</param>
        /// <returns>The value of the configuration</returns>
        public virtual string GetConfiguration(string Key)
        {
            string toReturn = null;

            foreach (IProvideConfigurations provider in Providers)
            {
                toReturn = provider.GetConfiguration(Key);

                if (toReturn != null)
                {
                    return toReturn;
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Returns the first non-null value of a connection string from the list of children
        /// </summary>
        /// <param name="ConnectionStringName">The name of the connection string to return</param>
        /// <returns>The value of the connection string</returns>
        public string GetConnectionString(string ConnectionStringName)
        {
            foreach (IProvideConfigurations provider in Providers)
            {
                string ConnectionString = provider.GetConnectionString(ConnectionStringName);
                if (ConnectionString != null)
                {
                    return ConnectionString;
                }
            }

            return null;
        }

        public bool SetConfiguration(string Name, string Value)
        {
            bool toReturn = false;

            foreach (IProvideConfigurations pc in Providers)
            {
                if (pc.CanWrite && pc.SetConfiguration(Name, Value))
                {
                    toReturn = true;
                }
            }

            return toReturn;
        }
    }
}