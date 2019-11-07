using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager
{
    // Set to true when system is ready for use
    static private bool loaded = false;

    // Current chosen language
    static private SystemLanguage currentLanguage;

    // Data for current language
    static private Dictionary<string, string> languageData;

    // List of all language files
    static private Dictionary<SystemLanguage, string> languageFiles;

    // Name of default language file
    static private string defaultLanguageFile;

    static private bool isRtl;

    /*
     * Returns localized string for key
     */
    public static string GetLocalizedText(string key)
    {
        if (!loaded) Initialize();
        if (languageData.ContainsKey(key))
            return languageData[key];
        else
            return String.Format("[{0}]", key);
    }

    /*
     * Returns true if current language is right-to-left
     */
    public static bool IsRTL()
    {
        if (!loaded) Initialize();
        return isRtl;
    }

    /*
     * Returns current active language - set with SetCurrentLanguage or from system
     */
    public static SystemLanguage GetCurrentLanguage()
    {
        if (!loaded) Initialize();
        return currentLanguage;
    }

    /*
     * Sets current active language and loads it's data
     */
    public static void SetCurrentLanguage(SystemLanguage language)
    {
        PlayerPrefs.SetString("Language", language.ToString());
        PlayerPrefs.Save();

        if (!loaded) Initialize();

        if (currentLanguage != language) LoadLanguageFile(language);
    }

    // Loads language data
    private static void LoadLanguageFile(SystemLanguage language)
    {
        if (languageFiles.ContainsKey(language))
            languageData = LoadTextFile(languageFiles[language]);
        else
            languageData = LoadTextFile(defaultLanguageFile);

        currentLanguage = language;

        if (languageData.ContainsKey("RTL"))
        {
            // Unity text rendering sucks so we flip RTL text to render correctly

            Dictionary<string, string> newData = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> entry in languageData)
            {
                newData.Add(entry.Key, ArabicSupport.ArabicFixer.Fix(entry.Value, false, false));
            }
            languageData = newData;
            isRtl = true;
        }
        else
        {
            isRtl = false;
        }
    }

    // Loads data from key=value text resource
    private static Dictionary<string, string> LoadTextFile(string file)
    {
        TextAsset targetFile = Resources.Load<TextAsset>(file);
        Dictionary<string, string> result = new Dictionary<string, string>();

        if (targetFile == null)
        {
            Debug.LogErrorFormat("Unable to load text resource {0}", file);
            return result;
        }

        char[] sep = "=".ToCharArray();

        foreach (string line in targetFile.text.Split('\n'))
        {
            string clean = line.Trim();
            if (clean.Length == 0 || clean.StartsWith("#")) continue;
            string[] split = line.Split(sep, 2);
            if (split.Length != 2)
            {
                Debug.LogWarningFormat("Error parsing line in {0}: {1}", file, clean);
            }
            else
            {
                result.Add(split[0].Trim(), split[1].Trim());
            }
        }

        return result;
    }

    private static void Initialize()
    {
        // Load and process settings 

        Dictionary<string, string> langs = LoadTextFile("localization");

        languageFiles = new Dictionary<SystemLanguage, string>();

        foreach (KeyValuePair<string, string> entry in langs)
        {
            if (entry.Key == "_Default")
            {
                defaultLanguageFile = entry.Value;
            }
            else
            {
                SystemLanguage lang = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), entry.Key);
                Debug.LogFormat("Adding language {0} with file {1}", lang, entry.Value);
                languageFiles.Add(lang, entry.Value);
            }
        }

        // Check that we have default set

        if (defaultLanguageFile == null)
        {
            Debug.LogError("Default language file not set!");
        }
        else
        {
            Debug.LogFormat("Loaded languages, default = {0}", defaultLanguageFile);
        }

        // Check if player chose a language

        string langName = PlayerPrefs.GetString("Language", "");

        if (langName == "")
        {
            // No player setting - use default
            LoadLanguageFile(Application.systemLanguage);
        }
        else
        {
            try
            {
                LoadLanguageFile((SystemLanguage)Enum.Parse(typeof(SystemLanguage), langName));
            }
            catch (ArgumentException)
            {
                LoadLanguageFile(Application.systemLanguage);
            }
        }

        loaded = true;
    }
}
