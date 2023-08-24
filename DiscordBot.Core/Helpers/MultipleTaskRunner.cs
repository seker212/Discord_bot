namespace DiscordBot.Core.Helpers
{
    public static class MultipleTaskRunner
    {
        public static Task RunTasksAsync(params Task[] tasks)
        {
            return Task.Run(() =>
            {
                foreach (var task in tasks)
                    task.Start();
                Task.WaitAll(tasks);
            });
        }

        public static Task RunTasksAsync(IEnumerable<Task> tasks)
            => RunTasksAsync(tasks.ToArray());
    }
}
