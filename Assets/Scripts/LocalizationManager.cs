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
        },
        {
                "hint_start_wave",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Становятся слышны звуки движений вдалеке, а потом и силуэты кого-то.Значит всё-таки тут есть жизнь!" },
                    { Language.English, "The sounds of movement in the distance become audible, and then the silhouettes of someone.So there is life here after all!" }
                }
        },
        {
                "hint_who_am_i",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Кто я?" },
                    { Language.English, "Who am I?" }
                }
},
        {
                "hint_start_game",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "КТО Я!?" },
                    { Language.English, "WHO AM I!?" }
                }
        },
        {
                "hint_tree_changing",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Похоже, это изменяет дерево..." },
                    { Language.English, "The tree is changing..." }
                }
        },
        {
                "hint_tree_hardens",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "ВАУ! Дерево определённо стало больше!" },
                    { Language.English, "Wow! The tree has obviously become bigger!" }
                }
        },
        {
                "hint_first_wave",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Он хочет полакомиться мной или помочь мне?" },
                    { Language.English, "Does he want to feast on me or help me?" }
                }
        },
        {
                "hint_first_remains",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "ОЙ! Я не хотел тебя есть! Это не я!" },
                    { Language.English, "Ow! I didn't want to eat you! It's not me!" }
                }
        },
        {
                "hint_second_wave",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "С каждым днём их становится больше..." },
                    { Language.English, "There are more of them every day..." }
                }
        },
        {
                "hint_third_wave",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Это они уничтожили эту планету? Откуда они здесь?" },
                    { Language.English, "Did they destroy this planet? Where did they come from?" }
                }
        },
        {
                "hint_tree_damaged",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Дереву больно, когда мне причиняют вред!" },
                    { Language.English, "It hurts a tree when I get hurt!" }
                }
        },
        {
                "hint_cord_growth",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Я чувствую больше свободы... кажется я смогу освободиться однажды" },
                    { Language.English, "I feel more freedom...I think I can get free one day" }
                }
        },
        {
                "hint_first_shot",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Что это вылетает из меня? Это дерево меня защищает?!" },
                    { Language.English, "What's coming out of me? Is this tree protecting me?!" }
                }
        },
        {
                "hint_tree_healed",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Ммммм, они восстанавливают дерево!" },
                    { Language.English, "Mmmmm, they're restoring the tree!" }
                }
        },
        {
                "hint_healing_fruit",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Видимо, метаморфозы дерева влияют на появление этих плодов..." },
                    { Language.English, "Apparently, the metamorphosis of the tree affects the appearance of these fruits..." }
                }
        },
        {
                "hint_cant_leave",
                new Dictionary<Language, string>()
                {
                    { Language.Russian, "Почему он удерживает меня? Я не могу от него оторваться!" },
                    { Language.English, "Why is he holding me back? I can't tear myself away!" }
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