using DedicatedUnityCloudBuild.Variables;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DedicatedUnityCloudBuild.Config
{
    public class Config
    {
        // This is the data structure that will be used to store the config data.

        #region JsonFields

        // Project Name
        [JsonInclude]
        public string ProjectName { get; private set; }

        // URL to github Repository
        [JsonInclude]
        public string GitUrl { get; private set; }

        // Path for local git clone
        [JsonInclude]
        public string GitRepoPath { get; private set; }

        // Number of the build
        [JsonInclude]
        public int? BuildNumber { get; set; }

        // url of the webserver
        [JsonInclude]
        public String WebServerURL { get; private set; }

        // port of the webserver
        [JsonInclude]
        public int? WebServerPort { get; private set; }

        #endregion JsonFields

        // constructor for Deserialization
        public Config() { }

        // constructor for creating new config with default values
        public void SetDefaults()
        {
            // set default Values below
            if (ProjectName == null)
                ProjectName = "Enter your Project name here";

            if (GitUrl == null)
                GitUrl = "Enter your git url here";

            if (GitRepoPath == null)
                GitRepoPath = ProgramVariables.applicationPath + "clone";

            if (BuildNumber == null)
                BuildNumber = -1;

            if (WebServerURL == null)
                WebServerURL = "http://localhost";

            if (WebServerPort == null)
                WebServerPort = 8080;
        }

        public bool validateAllFIelds()
        {
            // check if no variable is null
            if (ProjectName == null)
                return false;

            if (GitUrl == null)
                return false;

            if (GitRepoPath == null)
                return false;

            if (BuildNumber == null)
                return false;

            if (WebServerURL == null)
                return false;

            if (WebServerPort == null)
                return false;

            return true;
        }

        public override string ToString()
        {
            var properties = GetType().GetProperties();
            var values = properties.Select(property => $"{property.Name}: {property.GetValue(this)}");
            return string.Join("\n", values);

            //return "Project Name: " + ProjectName + "\nYour Git URL of your project: " + GitUrl + "\nLast Build Number: " + BuildNumber;
        }
    }
}