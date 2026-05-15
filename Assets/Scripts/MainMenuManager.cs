using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject tutorial;
    [Header("Sound Settings")]
    public AudioClip buttonClickSound;

    [Header("UI Elements")]
    public GameObject settingsPanel;
    public GameObject tutorialPanel;
    private void Start()
    {
        if (!PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 0.5f);
            PlayerPrefs.Save();
        }

        float savedVolume = PlayerPrefs.GetFloat("Volume");

        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.volume = savedVolume;
            audioSource.Play();
        }

        AudioListener.volume = savedVolume;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "MainMenu")
        {
            HandleBackAction();
        }
    }
    void HandleBackAction()
    {
        // 1. Если открыт туториал — закрываем
        if (tutorialPanel != null && tutorialPanel.activeSelf)
        {
            HideTutorial();
            return;
        }

        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            ToggleSettingsPanel();
            return;
        }

        QuitGame();
    }
    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        PlaySound(buttonClickSound);
    }
    
    public void ShowTutorial()
    {
        PlaySound(buttonClickSound);

        if (tutorialPanel)
        {
            tutorialPanel.SetActive(true);
        }
    }
    public void HideTutorial()
    {
        PlaySound(buttonClickSound);
        tutorial.SetActive(false);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(
                clip,
                Camera.main.transform.position,
                AudioListener.volume
            );
        }
    }

    public void StartGame()
    {
        PlaySound(buttonClickSound);
        StartCoroutine(StartGameWithDelay());
    }

    public void QuitGame()
    {
        PlaySound(buttonClickSound);
        StartCoroutine(QuitWithDelay());
    }
    IEnumerator StartGameWithDelay()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene("Cutscene");
    }

    IEnumerator QuitWithDelay()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Application.Quit();
    }
    public void SetFullHDResolution()
    {
        // true = fullscreen, false = windowed
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);

        PlaySound(buttonClickSound);
    }
}