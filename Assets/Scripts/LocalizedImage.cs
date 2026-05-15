using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LocalizedImage : MonoBehaviour
{
    public Sprite russianSprite;
    public Sprite englishSprite;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        UpdateImage();

        LocalizationManager.Instance.OnLanguageChanged += UpdateImage;
    }

    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= UpdateImage;
    }

    private void UpdateImage()
    {
        switch (LocalizationManager.Instance.CurrentLanguage)
        {
            case Language.Russian:
                image.sprite = russianSprite;
                break;

            case Language.English:
                image.sprite = englishSprite;
                break;
        }
    }
}