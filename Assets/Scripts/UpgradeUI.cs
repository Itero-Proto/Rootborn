using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradeUI : MonoBehaviour
{
    public Button[] buttons;

    private UpgradeManager manager;

    public void Setup(List<UpgradeType> upgrades, UpgradeManager m)
    {
        manager = m;

        for (int i = 0; i < buttons.Length; i++)
        {
            UpgradeType type = upgrades[i];

            buttons[i].GetComponentInChildren<TMP_Text>().text = GetName(type);

            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => manager.ChooseUpgrade(type));
        }
    }

    string GetName(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.MoveSpeed: return "Speed +1";
            case UpgradeType.UmbilicalLength: return "Cord Length +1";
            case UpgradeType.FireRate: return "Fire Rate +";
        }
        return "";
    }
}