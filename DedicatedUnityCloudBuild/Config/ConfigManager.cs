using System.Text.Json;

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
        public Config cfg;

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
            // create new config with default values
            cfg = new Config();
            Logger.Instance.LogInfo("Created new config with default values at path " + _configPath);
        }

        private void FixCorruptedConfig()
        {
            // fix corrupted config file
            Logger.Instance.LogWarningBlock("Config File is corrupted", "Fixing corrupted config file by copying default values to corrupted fields...");

            // Valid config values have been loaded, but some are missing
            // set Default Values to those fields which are null
            cfg.SetDefaults();

            // Save fixed config to disk
            SerializeConfig();
        }

        #endregion Config Creation/Fixing

        #region XML Serialization/Deserialization

        // deserialize config from file
        private bool DeserializeConfig()
        {
            try
            {
                // read config file to jsonString variable
                String jsonString = File.ReadAllText(_configPath);

                // deserialize string to config object
                cfg = JsonSerializer.Deserialize<Config>(jsonString);

                // return true for success
                if (ProgramVariables.verbose)
                    Logger.Instance.Log("Config file successfully deserialized and loaded");

                // check if any field is null
                if (!cfg.validateAllFIelds())
                {
                    // fix config since there was a null field
                    FixCorruptedConfig();
                }

                Logger.Instance.LogInfoBlock("Loaded Config with following settings", cfg.ToString());
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
                // set json file to be indented (pretty printed)
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                // serialize config object
                string jsonString = JsonSerializer.Serialize(cfg, options);
                File.WriteAllText(_configPath, jsonString);

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