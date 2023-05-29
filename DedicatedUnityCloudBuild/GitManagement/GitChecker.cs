using DedicatedUnityCloudBuild.Config;
using DedicatedUnityCloudBuild.Log;
using DedicatedUnityCloudBuild.Variables;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Timers;
using LibGit2Sharp;
using System.IO;

namespace DedicatedUnityCloudBuild.GitManagement
{
    public class GitChecker
    {
        public System.Timers.Timer scheduler;

        public static GitChecker? Instance { get; private set; }

        public GitChecker()
        {
            // check if there is already instance of ConfigManager
            if (Instance != null)
            {
                Logger.Instance.LogError("GitChecker Instance already exists!");

                // throw new Exception("ConfigManager already exists!");
            }
            else
            {
                // else set current object as Instance
                Instance = this;

                if (ProgramVariables.verbose)
                    Logger.Instance.LogInfo("Created new GitChecker Instance");

                StartDispatcher();
            }
        }

        private async void ScheduleGitCheck(object state, ElapsedEventArgs e)
        {
            // check for a new commit by grabbing latest commit from remote repository and comparing it to local stored value
            Logger.Instance.Log("Checking for new commits in remote Repository...");

            string latestCommitId = await GetLatestCommitId();
            if (ProgramVariables.verbose)
                Logger.Instance.Log("Latest Commit ID: " + latestCommitId);

            if (latestCommitId != ConfigManager.Instance.cfg.LastCommitId)
            {
                Logger.Instance.LogBlock("New Commit found", "Last downloaded commit was " + ConfigManager.Instance.cfg.LastCommitId + ".\nLatest CommitId of remote repository is " + latestCommitId + "\n\nCloning repository into " + ConfigManager.Instance.cfg.GitRepoPath);

                // store latest commit id in config and save it
                ConfigManager.Instance.cfg.LastCommitId = latestCommitId;
                ConfigManager.Instance.SaveConfig();

                //clone repository
                CloneRepository();
            }
            else
            {
                Logger.Instance.LogBlock("Already on latest commit", "Latest commit id is " + latestCommitId);
            }
        }

        private void CloneRepository()
        {
            // Get configuration values

            // Clone URL of the repository
            string gitUrl = ConfigManager.Instance.cfg.GitUrl;

            // Branch name to clone
            string gitBranch = ConfigManager.Instance.cfg.RepoBranch;

            // Personal access token for accessing the repository
            string personalAccessToken = ConfigManager.Instance.cfg.GitHubAccessToken;

            // Local path where the repository will be cloned
            string localPath = ConfigManager.Instance.cfg.GitRepoPath;

            // Delete the existing repository, if it exists
            if (Directory.Exists(localPath))
            {
                var directory = new DirectoryInfo(localPath) { Attributes = FileAttributes.Normal };

                foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    info.Attributes = FileAttributes.Normal;
                }

                directory.Delete(true);

                Logger.Instance.LogInfo("Deleted existing repository");
            }

            // Set up clone options with credentials
            var cloneOptions = new CloneOptions
            {
                CredentialsProvider = (_, __, ___) =>
                    new UsernamePasswordCredentials
                    {
                        // Set the username as the personal access token
                        Username = personalAccessToken,

                        // Set an empty password
                        Password = string.Empty
                    }
            };

            try
            {
                // Clone the repository
                Repository.Clone(gitUrl, localPath, cloneOptions);

                // Checkout the specified branch
                using (var repo = new Repository(localPath))
                {
                    // Get the branch to checkout
                    Branch branch = repo.Branches[gitBranch];

                    // Get the commit associated with the branch
                    Commit commit = branch.Tip;

                    // Get the tree associated with the commit
                    Tree tree = commit.Tree;

                    // Set up checkout options
                    var checkoutOptions = new CheckoutOptions
                    {
                        CheckoutModifiers = CheckoutModifiers.Force    // Optionally, force the checkout
                    };

                    // Perform the checkout
                    repo.Checkout(commit.Tree, new[] { "*.*" }, checkoutOptions);
                }

                Logger.Instance.LogInfo("Repository cloned successfully");
            }
            catch (Exception e)
            {
                Logger.Instance.LogErrorBlock("Clone of repository failed", e.StackTrace);
            }
        }

        public async Task<string> GetLatestCommitId()
        {
            // load repository url and access token from config
            String repositoryUrl = ConfigManager.Instance.cfg.GitUrl;
            String accessToken = ConfigManager.Instance.cfg.GitHubAccessToken;

            // Extract the repository owner and name from the URL
            string apiUrl = "https://api.github.com/repos/";
            string[] segments = repositoryUrl.Split('/');
            string owner = segments[segments.Length - 2];
            string repoName = segments[segments.Length - 1].Replace(".git", "");

            // Create a new HttpClient and set the required headers
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "DedicatedUnityCloudBuild");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", accessToken);

                // Make a GET request to the GitHub API to retrieve the latest commit
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}{owner}/{repoName}/commits/master");

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the response content as JSON
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var commit = JsonSerializer.Deserialize<JsonDocument>(responseContent, options);
                    string latestCommitId = commit.RootElement.GetProperty("sha").GetString();

                    return latestCommitId;
                }
                else
                {
                    // Handle the case where the request was not successful
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }

            return null;
        }

        #region start/stop dispatcher

        public void StartDispatcher()
        {
            int interval = (int)ConfigManager.Instance.cfg.FetchInterval;
            Logger.Instance.LogInfo("Scheduled Git Dispatcher started...");
            scheduler = new System.Timers.Timer(interval * 1000);

            // Hook up the Elapsed event for the timer.
            scheduler.Elapsed += ScheduleGitCheck;

            // auto restart timer
            scheduler.AutoReset = true;

            // enable timer
            scheduler.Enabled = true;
        }

        public void StopDispatcher()
        {
            Logger.Instance.LogInfo("Scheduled Git Dispatcher stopped...");
            scheduler.Stop();
        }

        #endregion start/stop dispatcher

        public void Dispose()
        {
            StopDispatcher();

            if (ProgramVariables.verbose)
                Logger.Instance.LogInfo("Disposed GitChecker Instance");

            Instance = null;
        }
    }
}