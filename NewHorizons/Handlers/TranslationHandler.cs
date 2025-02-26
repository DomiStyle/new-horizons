using NewHorizons.External.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using Logger = NewHorizons.Utility.Logger;

namespace NewHorizons.Handlers
{
    public static class TranslationHandler
    {
        private static Dictionary<TextTranslation.Language, Dictionary<string, string>> _shipLogTranslationDictionary = new Dictionary<TextTranslation.Language, Dictionary<string, string>>();
        private static Dictionary<TextTranslation.Language, Dictionary<string, string>> _dialogueTranslationDictionary = new Dictionary<TextTranslation.Language, Dictionary<string, string>>();
        private static Dictionary<TextTranslation.Language, Dictionary<string, string>> _uiTranslationDictionary = new Dictionary<TextTranslation.Language, Dictionary<string, string>>();

        public enum TextType
        {
            SHIPLOG,
            DIALOGUE,
            UI
        }

        public static string GetTranslation(string text, TextType type)
        {
            Dictionary<TextTranslation.Language, Dictionary<string, string>> dictionary;
            var language = TextTranslation.Get().m_language;

            switch (type)
            {
                case TextType.SHIPLOG:
                    dictionary = _shipLogTranslationDictionary;
                    break;
                case TextType.DIALOGUE:
                    dictionary = _dialogueTranslationDictionary;
                    break;
                case TextType.UI:
                    dictionary = _uiTranslationDictionary;
                    break;
                default:
                    Logger.LogVerbose($"Invalid TextType {type}");
                    return text;
            }

            // Get the translated text
            if (dictionary.TryGetValue(language, out var table))
                if (table.TryGetValue(text, out var translatedText))
                    return translatedText;

            Logger.LogVerbose($"Defaulting to english for {text}");

            // Try to default to English
            if (dictionary.TryGetValue(TextTranslation.Language.ENGLISH, out var englishTable))
                if (englishTable.TryGetValue(text, out var englishText))
                    return englishText;

            Logger.LogVerbose($"Defaulting to key for {text}");

            // Default to the key
            return text;
        }

        public static void RegisterTranslation(TextTranslation.Language language, TranslationConfig config)
        {
            if (config.ShipLogDictionary != null && config.ShipLogDictionary.Count > 0)
            {
                if (!_shipLogTranslationDictionary.ContainsKey(language)) _shipLogTranslationDictionary.Add(language, new Dictionary<string, string>());
                foreach (var originalKey in config.ShipLogDictionary.Keys)
                {
                    var key = originalKey.Replace("&lt;", "<").Replace("&gt;", ">").Replace("<![CDATA[", "").Replace("]]>", "");

                    if (!_shipLogTranslationDictionary[language].ContainsKey(key)) _shipLogTranslationDictionary[language].Add(key, config.ShipLogDictionary[originalKey]);
                    else _shipLogTranslationDictionary[language][key] = config.ShipLogDictionary[originalKey];
                }
            }

            if (config.DialogueDictionary != null && config.DialogueDictionary.Count > 0)
            {
                if (!_dialogueTranslationDictionary.ContainsKey(language)) _dialogueTranslationDictionary.Add(language, new Dictionary<string, string>());
                foreach (var originalKey in config.DialogueDictionary.Keys)
                {
                    var key = originalKey.Replace("&lt;", "<").Replace("&gt;", ">").Replace("<![CDATA[", "").Replace("]]>", "");

                    if (!_dialogueTranslationDictionary[language].ContainsKey(key)) _dialogueTranslationDictionary[language].Add(key, config.DialogueDictionary[originalKey]);
                    else _dialogueTranslationDictionary[language][key] = config.DialogueDictionary[originalKey];
                }
            }

            if (config.UIDictionary != null && config.UIDictionary.Count() > 0)
            {
                if (!_uiTranslationDictionary.ContainsKey(language)) _uiTranslationDictionary.Add(language, new Dictionary<string, string>());
                foreach (var originalKey in config.UIDictionary.Keys)
                {
                    var key = originalKey.Replace("&lt;", "<").Replace("&gt;", ">").Replace("<![CDATA[", "").Replace("]]>", "");

                    if (!_uiTranslationDictionary[language].ContainsKey(key)) _uiTranslationDictionary[language].Add(key, config.UIDictionary[originalKey]);
                    else _uiTranslationDictionary[language][key] = config.UIDictionary[originalKey];
                }
            }
        }

        public static void AddDialogue(string rawText, params string[] rawPreText)
        {
            var key = string.Join(string.Empty, rawPreText) + rawText;

            var text = GetTranslation(rawText, TextType.DIALOGUE);

            TextTranslation.Get().m_table.Insert(key, text);
        }

        public static void AddShipLog(string rawText, params string[] rawPreText)
        {
            var key = string.Join(string.Empty, rawPreText) + rawText;

            string text = GetTranslation(rawText, TextType.SHIPLOG);

            TextTranslation.Get().m_table.InsertShipLog(key, text);
        }

        public static int AddUI(string rawText)
        {
            var uiTable = TextTranslation.Get().m_table.theUITable;

            var text = GetTranslation(rawText, TextType.UI).ToUpper();

            var key = uiTable.Keys.Max() + 1;
            try
            {
                // Ensure it doesn't already contain our UI entry
                KeyValuePair<int, string> pair = uiTable.First(x => x.Value.Equals(text));
                if (pair.Equals(default(KeyValuePair<int, string>))) key = pair.Key;
            }
            catch (Exception) { }

            TextTranslation.Get().m_table.Insert_UI(key, text);

            return key;
        }

        public static void ClearTables()
        {
            _shipLogTranslationDictionary.Clear();
            _dialogueTranslationDictionary.Clear();
            _uiTranslationDictionary.Clear();
        }
    }
}
