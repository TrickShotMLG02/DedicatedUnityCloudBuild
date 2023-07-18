using DedicatedUnityCloudBuild.Log;
using DedicatedUnityCloudBuild.Variables;

namespace DedicatedUnityCloudBuild.UnityBuild
{
    internal class UnityBuildAgent
    {
        public static UnityBuildAgent? Instance { get; private set; }

        public UnityBuildAgent()
        {
            // check if there is already instance of ConfigManager
            if (Instance != null)
            {
                Logger.Instance.LogError("UnityBuildAgent Instance already exists!");

                // throw new Exception("ConfigManager already exists!");
            }
            else
            {
                // else set current object as Instance
                Instance = this;

                if (ProgramVariables.verbose)
                    Logger.Instance.LogInfo("Created new UnityBuildAgent Instance");
            }
        }

        public bool Build()
        {
            bool exitStatus = false;
            ProgramVariables.readyForBuild = false;

            // do build stuff here

            ProgramVariables.readyForBuild = true;

            return exitStatus;
        }

        public void Dispose()
        {
            // interrupt build process if running
            // clean up build directory

            if (ProgramVariables.verbose)
                Logger.Instance.LogInfo("Disposed UnityBuildAgent Instance");

            Instance = null;
        }
    }
}