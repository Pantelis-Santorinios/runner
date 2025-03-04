using System;
using System.IO;
using GitHub.Runner.Sdk;

namespace GitHub.Runner.Common
{
    public class ActionManager
    {
        private readonly IHostContext _hostContext;
        private readonly string _toolDirectory;

        public ActionManager(IHostContext hostContext)
        {
            _hostContext = hostContext ?? throw new ArgumentNullException(nameof(hostContext));
            _toolDirectory = _hostContext.GetDirectory(WellKnownDirectory.Tools);
        }

        public bool ActionExistsInToolDirectory(string actionName)
        {
            var actionPath = Path.Combine(_toolDirectory, actionName);
            return Directory.Exists(actionPath) && File.Exists(Path.Combine(actionPath, "action.yml"));
        }

        public void DownloadActionIfNotExists(string actionName, Uri actionUri)
        {
            if (!ActionExistsInToolDirectory(actionName))
            {
                // Logic to download the action from the provided URI
                // This is a placeholder and should be replaced with actual download logic
                Console.WriteLine($"Downloading action {actionName} from {actionUri}");
            }
            else
            {
                Console.WriteLine($"Action {actionName} already exists in {_toolDirectory}");
            }
        }
    }
}
