namespace DiscordBot.Core.Helpers
{
    /// <summary>
    /// Static helper providing methods to run multiple tasks at once.
    /// </summary>
    public static class MultipleTaskRunner
    {
        /// <summary>
        /// Starts and waits untill all the tasks are completed.
        /// </summary>
        /// <param name="tasks">Initialized, not started tasks.</param>
        /// <returns>Task waiting for all given tasks.</returns>
        public static Task RunTasksAsync(params Task[] tasks)
        {
            return Task.Run(() =>
            {
                foreach (var task in tasks)
                    task.Start();
                Task.WaitAll(tasks);
            });
        }

        /// <summary>
        /// Starts and waits untill all the tasks are completed.
        /// </summary>
        /// <param name="tasks">Initialized, not started tasks.</param>
        /// <returns>Task waiting for all given tasks.</returns>
        public static Task RunTasksAsync(IEnumerable<Task> tasks)
            => RunTasksAsync(tasks.ToArray());
    }
}
