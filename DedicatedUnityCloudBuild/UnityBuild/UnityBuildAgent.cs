using DedicatedUnityCloudBuild.Config;
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
            bool buildSuccessful = false;
            ProgramVariables.readyForBuild = false;

            // check if main build directory exists, otherwhise create it
            if (!Directory.Exists(ProgramVariables.buildPath))
            {
                if (ProgramVariables.verbose)
                    Logger.Instance.LogInfo("Created root build directory");
                Directory.CreateDirectory(ProgramVariables.buildPath);
            }

            // store path for new build in string
            string currentBuildPath = ProgramVariables.buildPath + "\\" + "Build_" + CurrTime();

            // create currentBuildDirectory
            Directory.CreateDirectory(currentBuildPath);
            if (ProgramVariables.verbose)
                Logger.Instance.LogInfo("Created current build directory at " + currentBuildPath);

            Logger.Instance.LogInfoBlock("Build Process started", "Running Unity Build for the following commit\n" + ConfigManager.Instance.cfg.LastCommitId + "\n\nBuild will be created at the following Path\n" + currentBuildPath);

            // get the timestamp when the build started
            DateTime startingTimeStamp = DateTime.Now;
            string buildLogPath = "{ACTUAL PATH HERE}";

            //
            //
            // do build stuff here
            //
            //

            // get the timestamp when the build finished
            DateTime finishedTimeStamp = DateTime.Now;

            // calculate the build duration
            TimeSpan buildDuration = finishedTimeStamp.Subtract(startingTimeStamp);

            if (buildSuccessful)
                Logger.Instance.LogInfoBlock("Build Process finished", $"Build was successful!" + currentBuildPath + "\nBuild took " + buildDuration.ToString());
            else
                Logger.Instance.LogErrorBlock("Build Process failed", "Build took " + buildDuration.ToString() + "\n\nSee log at " + buildLogPath + " for more info");

            ProgramVariables.readyForBuild = true;

            return buildSuccessful;
        }

        private string CurrTime()
        {
            return DateTime.Now.ToString("YYYY_MM_HH_mm_ss");
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