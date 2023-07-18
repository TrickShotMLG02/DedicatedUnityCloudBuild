using System.Text.Json;

using DedicatedUnityCloudBuild.Variables;
using DedicatedUnityCloudBuild.Log;

namespace DedicatedUnityCloudBuild.Config
{
    #region disableWarnings

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

    #endregion disableWarnings

    internal class ConfigManager
    {
        // singleton pattern
        public static ConfigManager? Instance { get; private set; }

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

        public void Dispose()
        {
            if (ProgramVariables.verbose)
                Logger.Instance.LogInfo("Disposed ConfigManager Instance");

            Instance = null;
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
                // if deserialization failed, file was fixed, so load config again
                LoadConfig();

                // ASK USER TO CHECK CONFIG VALUES AND PRESS n TO CONTINUE or y to exit
                String title = "Corrupted Config File was fixed";
                String promt = "Please check all entries above and verify that they have the correct values assigned.\nPlease note that invalid values could lead to unwanted behaviour and could potentionally perform harmful operations!\n\nDo you want to terminate the program to edit the values? (y/n)";

                if (CmdUtil.Utilities.AskConfirm(title, promt))
                {
                    // terminate program
                    Environment.Exit(0);
                }
                else
                {
                    // continue
                }
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
                    return false;
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