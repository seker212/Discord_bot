namespace DiscordBot.Commands.Core.CommandAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
