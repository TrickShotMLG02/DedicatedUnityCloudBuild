using DedicatedUnityCloudBuild.Variables;
using System.Reflection;
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

        // Branch to use
        [JsonInclude]
        public string RepoBranch { get; private set; }

        // Access Token for github
        [JsonInclude]
        public string GitHubAccessToken { get; private set; }

        // Name for github
        [JsonInclude]
        public string GitName { get; private set; }

        // Email for github
        [JsonInclude]
        public string GitEmail { get; private set; }

        // Path for local git clone
        [JsonInclude]
        public string GitRepoPath { get; private set; }

        // Number of the build
        [JsonInclude]
        public string LastCommitId { get; set; }

        // Interval in seconds for fetching new commits
        [JsonInclude]
        public int? FetchInterval { get; private set; }

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

            if (RepoBranch == null)
                RepoBranch = "master";

            if (GitName == null)
                RepoBranch = "Enter your Git Name here";

            if (GitEmail == null)
                RepoBranch = "Enter your Git Email here";

            if (GitHubAccessToken == null)
                GitHubAccessToken = "Enter your PAT here";

            if (GitRepoPath == null)
                GitRepoPath = ProgramVariables.applicationPath + "clone";

            if (LastCommitId == null)
                LastCommitId = "This will be populated automatically";

            if (FetchInterval == null)
                FetchInterval = 60;

            if (WebServerURL == null)
                WebServerURL = "http://localhost";

            if (WebServerPort == null)
                WebServerPort = 8080;
        }

        public bool validateAllFIelds()
        {
            // check if all fields are set
            PropertyInfo[] properties = GetType().GetProperties();
            return !properties.Any(property => property.GetValue(this) == null);
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