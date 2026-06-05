using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    public string localizationKey;

    public bool showOnlyOnce = true;

    private bool triggered = false;

    private HintPopup hintPopup;

    void Start()
    {
        hintPopup = FindAnyObjectByType<HintPopup>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (showOnlyOnce && triggered)
            return;

        triggered = true;

        if (hintPopup != null)
        {
            hintPopup.ShowHint(
                LocalizationManager.Instance.GetText(localizationKey)
            );
        }
    }
}