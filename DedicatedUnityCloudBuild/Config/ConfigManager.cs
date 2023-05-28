using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace DedicatedUnityCloudBuild.Config
{
    internal class ConfigManager
    {
        // singleton pattern
        public static ConfigManager Instance { get; private set; }

        // path of the application currently executed
        private String applicationPath;

        // path of the config file
        private String configPath;

        // Config file container
        private Config config;

        // constructor
        public ConfigManager()
        {
            // check if there is already instance of ConfigManager
            if (Instance != null)
            {
                throw new Exception("ConfigManager already exists!");
            }
            else
            {
                // else set current object as Instance
                Instance = this;

                // initialize variables
                applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                configPath = applicationPath + "/config.xml";

                // try to load Config File
            }
        }

        #region Config Load/Save

        public void LoadConfig()
        {
            // check if config file exists at configPath
            if (!File.Exists(configPath))
            {
                // if not, create it
                CreateConfig();

                // Serialize default values of config file and save it
                SaveConfig();
            }

            // TODO: Load config file and deserialize it
            DeserializeConfig();
        }

        public void SaveConfig()
        {
            // TODO: save config file and serialize it
            SerializeConfig();
        }

        #endregion Config Load/Save

        #region Config Creation/Fixing

        private void CreateConfig()
        {
            // create config
            config = new Config();
            throw new NotImplementedException();
        }

        private void FixCorruptedConfig()
        {
            // fix corrupted config file
            throw new NotImplementedException();
        }

        #endregion Config Creation/Fixing

        #region XML Serialization/Deserialization

        private Config DeserializeConfig()
        {
            // deserialize config from file
            throw new NotImplementedException();
        }

        private bool SerializeConfig()
        {
            // serialize config to file
            throw new NotImplementedException();
        }

        #endregion XML Serialization/Deserialization
    }
}