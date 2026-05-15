using TMPro;
using UnityEngine;

public class LanguageButton : MonoBehaviour
{
    [SerializeField] private TMP_Text buttonText;

    private void Start()
    {
        UpdateButtonText();

        LocalizationManager.Instance.OnLanguageChanged += UpdateButtonText;
    }

    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateButtonText;
        }
    }

    public void ToggleLanguage()
    {
        if (LocalizationManager.Instance.CurrentLanguage == Language.Russian)
        {
            LocalizationManager.Instance.SetLanguage(Language.English);
        }
        else
        {
            LocalizationManager.Instance.SetLanguage(Language.Russian);
        }
    }

    private void UpdateButtonText()
    {
        if (LocalizationManager.Instance.CurrentLanguage == Language.Russian)
        {
            buttonText.text = "EN";
        }
        else
        {
            buttonText.text = "RU";
        }
    }
}