using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    private AudioSource backgroundMusicSource;

    private void Start()
    {
        // Если громкость ещё не сохранена — устанавливаем 50%
        if (!PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 0.5f);
            PlayerPrefs.Save();
        }

        // Загружаем сохранённое значение
        float savedVolume = PlayerPrefs.GetFloat("Volume");
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;

        // Находим AudioSource фона (если он есть)
        backgroundMusicSource = FindAnyObjectByType<AudioSource>();
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = savedVolume;
        }

        // Добавляем слушателя для изменений громкости
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }


    private void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);

        // Обновляем громкость фоновой музыки
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = volume;
        }
    }
}
