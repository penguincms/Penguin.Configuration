using Microsoft.Extensions.Configuration;
using Penguin.Configuration.Abstractions.Interfaces;
using System.Collections.Generic;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Penguin.Configuration.Providers
{
    /// <summary>
    /// Provides configurations from the Microsoft IConfiguration class used by .net core applications
    /// </summary>
    public class JsonProvider : IProvideConfigurations
    {
        /// <summary>
        /// Returns a dictionary of all configuraitons found in the IConfiguration
        /// </summary>
        public Dictionary<string, string> AllConfigurations
        {
            get
            {
                Dictionary<string, string> toReturn = new();

                foreach (IConfigurationSection section in SourceConfiguration.GetSection(AppSettingsSectionName).GetChildren())
                {
                    toReturn.Add(section.Key, section.Value);
                }

                return toReturn;
            }
        }

        /// <summary>
        /// Returns a dictionary of all connection strings found in the IConfiguration
        /// </summary>
        public Dictionary<string, string> AllConnectionStrings
        {
            get
            {
                Dictionary<string, string> toReturn = new();

                foreach (IConfigurationSection section in SourceConfiguration.GetSection(ConnectionStringsSectionName).GetChildren())
                {
                    toReturn.Add(section.Key, section.Value);
                }

                return toReturn;
            }
        }

        /// <summary>
        /// The name of the json section to use for application settings
        /// </summary>
        public string AppSettingsSectionName { get; protected set; }

        bool IProvideConfigurations.CanWrite => false;

        /// <summary>
        /// The name of the json section to use for connection strings
        /// </summary>
        public string ConnectionStringsSectionName { get; protected set; } = "ConnectionStrings";

        /// <summary>
        /// The IConfiguration used when creating this provider
        /// </summary>
        public IConfiguration SourceConfiguration { get; protected set; }

        /// <summary>
        /// Creates a new instance of this configuration provider
        /// </summary>
        /// <param name="provider">The IConfiguration to wrap</param>
        /// <param name="appSettingsSectionName">The json name of the default application settings section</param>
        public JsonProvider(IConfiguration provider, string appSettingsSectionName = "appSettings")
        {
            SourceConfiguration = provider;
            AppSettingsSectionName = appSettingsSectionName;
        }

        /// <summary>
        /// Returns the value of a configuration
        /// </summary>
        /// <param name="Key">The key of the configuration to get</param>
        /// <returns>The value (or null) of the requested configuration</returns>
        public string GetConfiguration(string Key)
        {
            return AllConfigurations.TryGetValue(Key, out string value) ? value : null;
        }

        /// <summary>
        /// Returns a connection string from the connection string section
        /// </summary>
        /// <param name="Name">The name of the connection string to get</param>
        /// <returns>The value (or null) of the requested connection string</returns>
        public string GetConnectionString(string Name)
        {
            return AllConnectionStrings.TryGetValue(Name, out string value) ? value : null;
        }

        /// <inheritdoc/>

        public bool SetConfiguration(string Name, string Value)
        {
            return false;
        }
    }
}