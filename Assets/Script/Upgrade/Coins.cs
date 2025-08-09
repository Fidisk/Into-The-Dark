using UnityEngine;
using TMPro;

public class Coins : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;

    void Update()
    {
        if (UpgradeManager.Instance != null && coinsText != null)
        {
            coinsText.text = $"Coins: {UpgradeManager.Instance.GetCoins()}";
        }
    }
}
