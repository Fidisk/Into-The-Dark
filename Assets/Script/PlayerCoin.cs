using UnityEngine;
using TMPro;

public class PlayerCoins : MonoBehaviour
{
    public int coinCount = 0;
    public TextMeshProUGUI coinText;

    public void AddCoins(int amount)
    {
        coinCount += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = coinCount.ToString();
    }

    public int GetCoins() {
        return coinCount;
    }
}
