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
            // check if a build is running currently
            if (!ProgramVariables.readyForBuild)
            {
                // dont update local repository, since we are building right now
                Logger.Instance.LogWarningBlock("Interrupted Git Check", "The scheduled git check was interrupted for security reasons, since a build process is currently running. The git check will run again in " + ConfigManager.Instance.cfg.FetchInterval + " seconds.");
                return;
            }

            // check for a new commit by grabbing latest commit from remote repository and comparing it to local stored value
            Logger.Instance.Log("Checking for new commits in remote Repository...");

            string latestRemoteCommitId = await GetLatestCommitId();

            if (ProgramVariables.verbose)
                Logger.Instance.Log("Latest Commit ID: " + latestRemoteCommitId);

            string localCommitId = "";

            using (var repo = new Repository(ConfigManager.Instance.cfg.GitRepoPath))
            {
                // Retrieve the current commit ID of the branch you're interested in
                var branch = repo.Head; // or specify a specific branch name: repo.Branches["branchName"]
                var commitId = branch.Tip.Id;

                // extract lates commit id from local repository
                localCommitId = commitId.ToString();
            }

            if (latestRemoteCommitId != localCommitId)
            {
                Logger.Instance.LogBlock("New Commit found", "Last downloaded commit was " + ConfigManager.Instance.cfg.LastCommitId + ".\nLatest CommitId of remote repository is " + latestRemoteCommitId + "\n\nCloning repository into " + ConfigManager.Instance.cfg.GitRepoPath);

                // store latest commit id in config and save it
                ConfigManager.Instance.cfg.LastCommitId = latestRemoteCommitId;
                ConfigManager.Instance.SaveConfig();

                // check if repository exists -> pull changes
                // otherwhise clone repository

                if (Repository.IsValid(ConfigManager.Instance.cfg.GitRepoPath))
                {
                    // pull changes since repository already exists
                    PullChanges();
                }
                else
                {
                    // clone repository since it doesnt exist yet
                    CloneRepository();
                }
            }
            else
            {
                Logger.Instance.LogBlock("Already on latest commit", "Latest commit id is " + latestRemoteCommitId);
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
                Logger.Instance.LogInfo("Cloning repository...");

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

        private void PullChanges()
        {
            string gitUrl = ConfigManager.Instance.cfg.GitUrl;

            // Branch name to clone
            string gitBranch = ConfigManager.Instance.cfg.RepoBranch;

            // Personal access token for accessing the repository
            string personalAccessToken = ConfigManager.Instance.cfg.GitHubAccessToken;

            // Local path where the repository will be cloned
            string localPath = ConfigManager.Instance.cfg.GitRepoPath;

            // Open the repository
            using (var repo = new Repository(localPath))
            {
                Logger.Instance.LogInfo("Pulling changes...");

                // Retrieve the reference to the remote branch you want to pull from
                var remote = repo.Network.Remotes["origin"];
                var remoteBranch = repo.Branches[$"{remote.Name}/{gitBranch}"];

                // Retrieve the local branch you want to merge into
                var localBranch = repo.Branches[gitBranch];

                // Define the pull options
                var pullOptions = new PullOptions
                {
                    MergeOptions = new MergeOptions
                    {
                        FileConflictStrategy = CheckoutFileConflictStrategy.Theirs
                    },

                    FetchOptions = new FetchOptions
                    {
                        CredentialsProvider = (_, __, ___) =>
                        new UsernamePasswordCredentials
                        {
                            // Set the username as the personal access token
                            Username = personalAccessToken,

                            // Set an empty password
                            Password = string.Empty
                        }
                    }
                };

                // Perform the pull, overwriting existing files
                var mergeResult = Commands.Pull(repo, new Signature(ConfigManager.Instance.cfg.GitName, ConfigManager.Instance.cfg.GitEmail, DateTimeOffset.Now), pullOptions);

                // Handle the merge result as needed
                if (mergeResult.Status == MergeStatus.Conflicts)
                {
                    // Handle merge conflicts
                    Logger.Instance.LogError("Merge conflicts occured");
                }
                else if (mergeResult.Status == MergeStatus.UpToDate)
                {
                    // Already up to date, no changes to pull
                    Logger.Instance.LogInfo("Already up to date, no changes to pull");
                }
                else if (mergeResult.Status == MergeStatus.FastForward || mergeResult.Status == MergeStatus.NonFastForward)
                {
                    // Merge was successful
                    Logger.Instance.LogInfo("Pull was successful");
                }
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