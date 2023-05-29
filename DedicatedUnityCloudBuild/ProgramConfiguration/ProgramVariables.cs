using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedUnityCloudBuild.Variables
{
    internal class ProgramVariables
    {
        #region Application Variables

        // github url for this program
        public static string repoURL = "https://github.com/TrickShotMLG02/DedicatedUnityCloudBuild";

        // current application name
        public static string applicationName = "DedicatedUnityCloudBuild";

        // current application version
        public static string applicationVersion = "0.0.1";

        // path of the application currently executed
        public static string applicationPath = AppDomain.CurrentDomain.BaseDirectory;

        #endregion Application Variables

        #region Config Variables

        // path of the config file
        public static string configPath = applicationPath + "/config.json";

        #endregion Config Variables

        #region Log Variables

        // path of the log file
        public static string logPath = applicationPath + "/log.txt";

        // log verbose or not
        public static bool verbose = false;

        // number of new lines before and after log block
        public static int numberOfLineBreaks = 1;

        #endregion Log Variables

        #region Build Variables

        // path of the build folder
        public static string buildPath = applicationPath + "/build";

        #endregion Build Variables
    }
}