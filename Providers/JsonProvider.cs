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
                Dictionary<string, string> toReturn = new Dictionary<string, string>();

                foreach (IConfigurationSection section in this.SourceConfiguration.GetSection(this.AppSettingsSectionName).GetChildren())
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
                Dictionary<string, string> toReturn = new Dictionary<string, string>();

                foreach (IConfigurationSection section in this.SourceConfiguration.GetSection(this.ConnectionStringsSectionName).GetChildren())
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
            this.SourceConfiguration = provider;
            this.AppSettingsSectionName = appSettingsSectionName;
        }

        /// <summary>
        /// Returns the value of a configuration
        /// </summary>
        /// <param name="Key">The key of the configuration to get</param>
        /// <returns>The value (or null) of the requested configuration</returns>
        public string GetConfiguration(string Key)
        {
            if (this.AllConfigurations.ContainsKey(Key))
            {
                return this.AllConfigurations[Key];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a connection string from the connection string section
        /// </summary>
        /// <param name="Name">The name of the connection string to get</param>
        /// <returns>The value (or null) of the requested connection string</returns>
        public string GetConnectionString(string Name)
        {
            if (this.AllConnectionStrings.ContainsKey(Name))
            {
                return this.AllConnectionStrings[Name];
            }
            else
            {
                return null;
            }
        }

        bool IProvideConfigurations.SetConfiguration(string Name, string Value) => false;
    }
}