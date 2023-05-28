using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

using DedicatedUnityCloudBuild.Variables;
using DedicatedUnityCloudBuild.Log;

namespace DedicatedUnityCloudBuild.Config
{
    internal class ConfigManager
    {
        // singleton pattern
        public static ConfigManager Instance { get; private set; }

        // path of the config file
        private String _configPath = ProgramVariables.configPath;

        // Config file container
        private Config _config;

        // constructor
        public ConfigManager()
        {
            // check if there is already instance of ConfigManager
            if (Instance != null)
            {
                Logger.Instance.LogError("ConfigManager Instance already exists!");

                // throw new Exception("ConfigManager already exists!");
            }
            else
            {
                // else set current object as Instance
                Instance = this;

                if (ProgramVariables.verbose)
                    Logger.Instance.LogInfo("Created new ConfigManager Instance");

                // initialize variables

                // try to load Config File
                LoadConfig();
            }
        }

        #region Config Load/Save

        public void LoadConfig()
        {
            // check if config file exists at configPath
            if (!File.Exists(_configPath))
            {
                Logger.Instance.LogInfo("Config File doesn't exist yet!");
                Logger.Instance.LogInfo("Creating new one...");

                // if not, create it
                CreateConfig();

                // Serialize default values of config file and save it
                SaveConfig();
            }

            //Load config file and deserialize it
            if (!DeserializeConfig())
            {
                // if deserialization fails, try to fix corrupted config file
                FixCorruptedConfig();
            }
        }

        public void SaveConfig()
        {
            //save config file and serialize it
            SerializeConfig();
        }

        #endregion Config Load/Save

        #region Config Creation/Fixing

        private void CreateConfig()
        {
            // create config
            _config = new Config();
            Logger.Instance.LogInfo("Config created at path " + _configPath);

            // throw new NotImplementedException();
        }

        private void FixCorruptedConfig()
        {
            // fix corrupted config file
            Logger.Instance.LogInfo("Fixing corrupted config file...");

            throw new NotImplementedException();
        }

        #endregion Config Creation/Fixing

        #region XML Serialization/Deserialization

        // deserialize config from file
        private bool DeserializeConfig()
        {
            try
            {
                // create new XML Serializer
                XmlSerializer x = new XmlSerializer(typeof(Config));

                // create new stream reader
                TextReader _reader = new StreamReader(_configPath);

                // deserialize config file to config object
                _config = (Config)x.Deserialize(_reader);

                _reader.Close();

                // return true for success
                if (ProgramVariables.verbose)
                    Logger.Instance.Log("Config file successfully deserialized and loaded");

                Logger.Instance.LogInfoBlock("Loaded Config with following settings", _config.ToString());
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.LogErrorBlock(e);
                return false;
            }
        }

        // serialize config to file
        private bool SerializeConfig()
        {
            try
            {
                // create new XML Serializer
                XmlSerializer x = new XmlSerializer(typeof(Config));

                // create new stream writer
                TextWriter _writer = new StreamWriter(_configPath);

                // serialize config object
                x.Serialize(_writer, _config);

                _writer.Close();

                // return true for success
                if (ProgramVariables.verbose)
                    Logger.Instance.Log("Config file successfully serialized");
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.LogErrorBlock(e);
                return false;
            }
        }

        #endregion XML Serialization/Deserialization
    }
}