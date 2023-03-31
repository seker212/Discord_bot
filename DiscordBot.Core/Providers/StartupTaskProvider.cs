using DiscordBot.Core.Helpers;

namespace DiscordBot.Core.Providers
{
    /// <summary>
    /// Providers <see cref="Task"/> preformed once after the bot has started.
    /// </summary>
    public interface IStartupTaskProvider
    {
        Task OnReady();
    }

    /// <inheritdoc cref="IStartupTaskProvider"/>
    public class StartupTaskProvider : IStartupTaskProvider
    {
        private readonly Task[] _startupTasks;

        /// <summary>
        /// Providers <see cref="Task"/> preformed once after the bot has started.
        /// </summary>
        /// <param name="startupTasks">All tasks that will be run on bot's startup.</param>
        public StartupTaskProvider(IEnumerable<Task> startupTasks)
        {
            _startupTasks = startupTasks.ToArray();
        }

        public Task OnReady()
            => MultipleTaskRunner.RunTasksAsync(_startupTasks);
    }
}
