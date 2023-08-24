namespace DiscordBot.Commands.Core.CommandAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NameAttribute : Attribute
    {
        public NameAttribute(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
