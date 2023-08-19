using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace DiscordBot.MessageHandlers.Helpers
{
    public interface IRegexResponseHelper
    {
        bool IsMatch(string text);
        string GetResponse(string text);
    }

    /// <summary>
    /// Helper for storing and comparing regexes and providing with their response
    /// </summary>
    public class RegexResponsesHelper : IRegexResponseHelper
    {
        private readonly ConcurrentDictionary<string, string> regexResponses;

        public RegexResponsesHelper() 
        {
            regexResponses = new ConcurrentDictionary<string, string>();

            AddOrUpdateRegexResponse(@"((dzie([ń|n]) dobry)|(cze([s|ś][c|ć]))) .*", "Dzień dobry! ");
            AddOrUpdateRegexResponse(@"dobranoc .*", "Dobranoc! ");
            AddOrUpdateRegexResponse(@"wypierdalaj .*", "Sam wypierdalaj ");
            AddOrUpdateRegexResponse(@"spierdalaj .*", "O ty chuju ");
        }

        private void AddOrUpdateRegexResponse(string key, string value)
        {
            regexResponses.AddOrUpdate(key, k => value, (k, v) => value);
        }

        public bool IsMatch(string text)
        {
            return regexResponses.Any(pair => Regex.IsMatch(text, pair.Key));
        }

        public string GetResponse(string text) 
        {
            return regexResponses.Where(pair => Regex.IsMatch(text, pair.Key)).First().Value;
        }
    }
}
