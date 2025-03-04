using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GitHub.DistributedTask.Pipelines;
using GitHub.DistributedTask.WebApi;
using GitHub.Services.Common;
using GitHub.Services.WebApi;

namespace GitHub.Runner.Common
{
    [ServiceLocator(Default = typeof(ActionsRunServer))]
    public interface IActionsRunServer : IRunnerService
    {
        Task ConnectAsync(Uri serverUrl, VssCredentials credentials);

        Task<AgentJobRequestMessage> GetJobMessageAsync(string id, CancellationToken token);
    }

    public sealed class ActionsRunServer : RunnerService, IActionsRunServer
    {
        private bool _hasConnection;
        private VssConnection _connection;
        private ActionsRunServerHttpClient _actionsRunServerClient;

        public async Task ConnectAsync(Uri serverUrl, VssCredentials credentials)
        {
            _connection = await EstablishVssConnection(serverUrl, credentials, TimeSpan.FromSeconds(100));
            _actionsRunServerClient = _connection.GetClient<ActionsRunServerHttpClient>();
            _hasConnection = true;
        }

        private void CheckConnection()
        {
            if (!_hasConnection)
            {
                throw new InvalidOperationException($"SetConnection");
            }
        }

        public Task<AgentJobRequestMessage> GetJobMessageAsync(string id, CancellationToken cancellationToken)
        {
            CheckConnection();
            var jobMessage = RetryRequest<AgentJobRequestMessage>(async () =>
                                                    {
                                                        return await _actionsRunServerClient.GetJobMessageAsync(id, cancellationToken);
                                                    }, cancellationToken);

            return jobMessage;
        }

        private bool ActionExistsInToolDirectory(string actionName)
        {
            var toolDirectory = HostContext.GetDirectory(WellKnownDirectory.Tools);
            var actionPath = Path.Combine(toolDirectory, actionName);
            return Directory.Exists(actionPath) && File.Exists(Path.Combine(actionPath, "action.yml"));
        }

        public async Task DownloadActionIfNotExists(string actionName, Uri actionUri)
        {
            if (!ActionExistsInToolDirectory(actionName))
            {
                // Logic to download the action from the provided URI
                // This is a placeholder and should be replaced with actual download logic
                Console.WriteLine($"Downloading action {actionName} from {actionUri}");
            }
            else
            {
                Console.WriteLine($"Action {actionName} already exists in {HostContext.GetDirectory(WellKnownDirectory.Tools)}");
            }
        }
    }
}
