using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public GameObject upgradePanel;
    public UmbilicalCord cord;
    public PlayerController player;

    public bool isChoosing = false;

    [Header("Audio")]
    public AudioClip cordUpgradeSound;
    public AudioClip moveSpeedUpgradeSound;
    public AudioClip fireRateUpgradeSound;   // 👈 ДОБАВИЛИ

    private AudioSource audioSource;

    private List<UpgradeType> allUpgrades = new List<UpgradeType>()
    {
        UpgradeType.MoveSpeed,
        UpgradeType.UmbilicalLength,
        UpgradeType.FireRate
    };

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void ShowUpgrades()
    {
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
        isChoosing = true;

        if (player != null)
            player.inputBlocked = true;

        List<UpgradeType> choices = GetRandomUpgrades(3);

        UpgradeUI ui = upgradePanel.GetComponentInChildren<UpgradeUI>();

        if (ui == null)
        {
            Debug.LogError("UpgradeUI NOT FOUND on panel or children!");
            return;
        }

        ui.Setup(choices, this);
    }
    public void ChooseUpgrade(UpgradeType type)
    {
        ApplyUpgrade(type);

        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
        isChoosing = false;

        if (player != null)
            player.inputBlocked = false;
    }

    void ApplyUpgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.MoveSpeed:
                player.moveSpeed += 1f;

                if (moveSpeedUpgradeSound != null && audioSource != null)
                {
                    audioSource.pitch = Random.Range(0.95f, 1.05f);
                    audioSource.PlayOneShot(moveSpeedUpgradeSound, 0.8f);
                }
                break;

            case UpgradeType.UmbilicalLength:
                if (cord != null)
                {
                    cord.IncreaseLength(1f);

                    if (cordUpgradeSound != null && audioSource != null)
                    {
                        audioSource.pitch = Random.Range(0.95f, 1.05f);
                        audioSource.PlayOneShot(cordUpgradeSound, 0.8f);
                    }
                }
                break;

            case UpgradeType.FireRate:
                player.fireRate *= 0.9f;

                // 🔊 звук скорострельности
                if (fireRateUpgradeSound != null && audioSource != null)
                {
                    audioSource.pitch = Random.Range(1.05f, 1.2f); // чуть “быстрее” звук
                    audioSource.PlayOneShot(fireRateUpgradeSound, 0.8f);
                }
                break;
        }
    }

    List<UpgradeType> GetRandomUpgrades(int count)
    {
        List<UpgradeType> copy = new List<UpgradeType>(allUpgrades);
        List<UpgradeType> result = new List<UpgradeType>();

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, copy.Count);
            result.Add(copy[index]);
            copy.RemoveAt(index);
        }

        return result;
    }
}