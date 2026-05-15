using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    public Language CurrentLanguage { get; private set; }

    public event Action OnLanguageChanged;

    private Dictionary<string, string> localizedText;

    private Dictionary<string, Dictionary<Language, string>> database =
        new Dictionary<string, Dictionary<Language, string>>()
    {


        {
            "skip_cutscene",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Нажмите ПРОБЕЛ, чтобы пропустить" },
                    { Language.English, "Press SPACE to skip" }
                }
        },
        {
            "bad_end",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Дерево невероятно выросло и поглотило всё на планете" },
                    { Language.English, "The tree grew enormously and swallowed up everything on the planet" }
                }
        },
        {
             "good_end",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Теперь вы независимы от этого дерева!" },
                    { Language.English, "Now you are independent of this tree!" }
                }
        },
        {
             "game_over",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Дерево погибло" },
                    { Language.English, "The tree died" }
                }
        }
     };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadLanguage();
    }

    public void SetLanguage(Language language)
    {
        CurrentLanguage = language;

        localizedText = new Dictionary<string, string>();

        foreach (var item in database)
        {
            localizedText[item.Key] = item.Value[language];
        }

        PlayerPrefs.SetInt("Language", (int)language);
        PlayerPrefs.Save();

        OnLanguageChanged?.Invoke();
    }

    public string GetText(string key)
    {
        if (localizedText.ContainsKey(key))
            return localizedText[key];

        return $"MISSING: {key}";
    }

    private void LoadLanguage()
    {
        Language savedLanguage = (Language)PlayerPrefs.GetInt("Language", 1);
        SetLanguage(savedLanguage);
    }
}