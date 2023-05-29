using System;

using DedicatedUnityCloudBuild.Config;
using DedicatedUnityCloudBuild.Log;
using DedicatedUnityCloudBuild.Variables;
using DedicatedUnityCloudBuild.WebServer;

namespace DedicatedUnityCloudBuild
{
    internal class Program
    {
        private static void initializeInstances()
        {
            // Initialize ConfigManager
            new ConfigManager();

            // Initialize Http Server
            new HttpServer();
        }

        private static void shutdownInstances()
        {
            HttpServer.Instance.Dispose();
            ConfigManager.Instance.Dispose();
            Logger.Instance.Dispose();
        }

        private static void Main(string[] args)
        {
            // Initialize Logger
            new Logger();
            Logger.Instance.LogInfoBlock(ProgramVariables.applicationName + " v" + ProgramVariables.applicationVersion, "Thank you for using this Program. If you have any issues, feel free to open an issue on the following page: " + ProgramVariables.repoURL);

            // Initialize all other instances
            initializeInstances();

            // for the termination

            // Save Config
            ConfigManager.Instance.SaveConfig();

            // shut down all instances
            shutdownInstances();
        }
    }
}