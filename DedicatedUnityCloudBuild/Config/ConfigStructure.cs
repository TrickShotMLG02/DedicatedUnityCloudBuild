using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedUnityCloudBuild.Config
{
    public class Config
    {
        // This is the data structure that will be used to store the config data.

        // Project Name
        public string ProjectName { get; set; }

        // URL to github Repository
        public string GitUrl { get; set; }

        // Number of the build
        public int BuildNumber { get; set; }

        public String WebServerURL { get; set; }

        public int WebServerPort { get; set; }

        // constructor
        public Config()
        {
            // set default Values below
            ProjectName = "Enter your Project name here";
            GitUrl = "Enter your git url here";
            BuildNumber = -1;

            WebServerURL = "http://localhost";
            WebServerPort = 8080;
        }

        public override string ToString()
        {
            return "Project Name: " + ProjectName + "\nYour Git URL of your project: " + GitUrl + "\nLast Build Number: " + BuildNumber;
        }
    }
}