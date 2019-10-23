using Penguin.Configuration.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;

namespace Penguin.Configuration.Providers
{
    /// <summary>
    /// Provides access to a web.config or app.config file for configurations
    /// </summary>
    public class XmlProvider : IProvideConfigurations
    {
        /// <summary>
        /// returns a dictionary of all configurations found in this file
        /// </summary>
        public Dictionary<string, string> AllConfigurations => AppSettings;

        /// <summary>
        /// returns a list of all connection strings found in this file
        /// </summary>
        public Dictionary<string, string> AllConnectionStrings => ConnectionStrings;

        bool IProvideConfigurations.CanWrite => false;
        private Dictionary<string, string> AppSettings { get; set; } = new Dictionary<string, string>();

        private Dictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Creates a new instance of this provider
        /// </summary>
        /// <param name="Path">The path to the .config</param>
        /// <param name="Required">if true, will error if the file is not found</param>
        public XmlProvider(string Path, bool Required = true)
        {
            if (!File.Exists(Path))
            {
                if (Required)
                {
                    throw new FileNotFoundException($"Required configuration {Path} not found");
                }
                else
                {
                    return;
                }
            }

            try
            {
                System.Configuration.ConfigurationFileMap fileMap = new ConfigurationFileMap(Path); //Path to your config file
                System.Configuration.Configuration configuration = System.Configuration.ConfigurationManager.OpenMappedMachineConfiguration(fileMap);

                foreach (ConnectionStringSettings connectionString in configuration.ConnectionStrings.ConnectionStrings)
                {
                    ConnectionStrings.Add(connectionString.Name, connectionString.ConnectionString);
                }

                foreach (string settingKey in configuration.AppSettings.Settings.AllKeys)
                {
                    AppSettings.Add(settingKey, configuration.AppSettings.Settings[settingKey].Value);
                }
            }
            catch (Exception)
            {
                ConnectionStrings.Clear();
                AppSettings.Clear();

                XmlDocument doc = new XmlDocument();
                doc.Load(Path);

                foreach (XmlNode node in doc.GetElementsByTagName("connectionStrings"))
                {
                    foreach (XmlNode connection in node.ChildNodes)
                    {
                        string Name = connection?.Attributes != null ? connection.Attributes["name"]?.Value : null;
                        string Value = connection?.Attributes != null ? connection.Attributes["connectionString"]?.Value : null;

                        if (Name != null && Value != null && !ConnectionStrings.ContainsKey(Name))
                        {
                            ConnectionStrings.Add(Name, Value);
                        }
                    }
                }

                foreach (XmlNode node in doc.GetElementsByTagName("appSettings"))
                {
                    foreach (XmlNode setting in node.ChildNodes)
                    {
                        string Name = setting?.Attributes != null ? setting.Attributes["key"]?.Value : null;
                        string Value = setting?.Attributes != null ? setting.Attributes["value"]?.Value : null;

                        if (Name != null && Value != null && !AppSettings.ContainsKey(Name))
                        {
                            AppSettings.Add(Name, Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new instance of this provider allowing access through ConfigurationManager
        /// </summary>
        public XmlProvider()
        {
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                ConnectionStrings.Add(connectionString.Name, connectionString.ConnectionString);
            }

            foreach (string settingKey in ConfigurationManager.AppSettings.AllKeys)
            {
                AppSettings.Add(settingKey, ConfigurationManager.AppSettings[settingKey]);
            }
        }

        /// <summary>
        /// Gets a configuration by the specified name
        /// </summary>
        /// <param name="Key">The name of the configuration to get</param>
        /// <returns>The value (or null) of the configuration</returns>
        public string GetConfiguration(string Key)
        {
            if (AppSettings.ContainsKey(Key))
            {
                return AppSettings[Key];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a connection string by the specified name
        /// </summary>
        /// <param name="Name">The name of the connection string</param>
        /// <returns>The value (or null) of the connection string</returns>
        public string GetConnectionString(string Name)
        {
            if (ConnectionStrings.ContainsKey(Name))
            {
                return ConnectionStrings[Name];
            }
            else
            {
                return null;
            }
        }

        bool IProvideConfigurations.SetConfiguration(string Name, string Value) => false;
    }
}