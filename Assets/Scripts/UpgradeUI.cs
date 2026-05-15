using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeUI : MonoBehaviour
{
    public Button[] buttons;

    [Header("Icons")]
    public Sprite speedIcon;
    public Sprite cordIcon;
    public Sprite fireRateIcon;

    private UpgradeManager manager;

    public void Setup(List<UpgradeType> upgrades, UpgradeManager m)
    {
        manager = m;

        for (int i = 0; i < buttons.Length; i++)
        {
            UpgradeType type = upgrades[i];

            Image icon = buttons[i].GetComponentInChildren<Image>();

            icon.sprite = GetIcon(type);

            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => manager.ChooseUpgrade(type));
        }
    }

    Sprite GetIcon(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.MoveSpeed:
                return speedIcon;

            case UpgradeType.UmbilicalLength:
                return cordIcon;

            case UpgradeType.FireRate:
                return fireRateIcon;
        }

        return null;
    }
}