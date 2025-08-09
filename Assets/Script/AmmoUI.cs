using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    public TextMeshProUGUI ammoText;

    private static AmmoUI _instance;
    public static AmmoUI Instance => _instance;

    void Awake()
    {
        _instance = this;
    }

    public void UpdateAmmo(int current, int reserve)
    {
        if (current > 10000) {
            if (reserve > 10000) {
                if (ammoText != null)
                    ammoText.text = $"Unlimited";
            }
            else {
                if (ammoText != null)
                    ammoText.text = $"Unlimited";
            }
        }
        else {
            if (reserve > 10000) {
                if (ammoText != null)
                    ammoText.text = $"{current} / Unlimited";
            }
            else {
                if (ammoText != null)
                    ammoText.text = $"{current} / {reserve}";
            }
        }
    }
}
