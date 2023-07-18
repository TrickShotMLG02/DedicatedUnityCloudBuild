using System;

using DedicatedUnityCloudBuild.Config;
using DedicatedUnityCloudBuild.GitManagement;
using DedicatedUnityCloudBuild.Log;
using DedicatedUnityCloudBuild.Variables;
using DedicatedUnityCloudBuild.WebServer;
using DedicatedUnityCloudBuild.UnityBuild;

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

            // Initialize Git Checker
            new GitChecker();

            // Initialize UnityBuild
            new UnityBuildAgent();
        }

        private static void shutdownInstances()
        {
            // Stop HttpServer
            HttpServer.Instance.Dispose();

            // Stop GitChecker
            GitChecker.Instance.Dispose();

            // Stop UnityBuildAgent
            UnityBuildAgent.Instance.Dispose();

            // Stop ConfigManager
            ConfigManager.Instance.Dispose();

            // Stop Logger
            Logger.Instance.Dispose();
        }

        private static void Main(string[] args)
        {
            // Initialize Logger
            new Logger();

            // Greet user
            Logger.Instance.LogInfoBlock(ProgramVariables.applicationName + " v" + ProgramVariables.applicationVersion, "Thank you for using this Program. If you have any issues, feel free to open an issue on the following page:\n" + ProgramVariables.repoURL);

            // Initialize all other instances
            initializeInstances();

            // keep program running
            while (true) ;

            /////////////////////////
            //                     //
            // for the termination //
            //                     //
            /////////////////////////

            // Save Config
            ConfigManager.Instance.SaveConfig();

            // shut down all instances
            shutdownInstances();
        }
    }
}