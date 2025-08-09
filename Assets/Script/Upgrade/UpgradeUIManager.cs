using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradeUIManager : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeButton
    {
        public Button button;
        public Image iconImage;
        public TMP_Text descriptionText;
        public TMP_Text priceText;
    }

    public List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();

    public List<Upgrade> currentUpgrades = new List<Upgrade>();

    void OnEnable()
    {
        UpgradeManager.EnsureExists();
        if (UpgradeManager.Instance != null)
        {
            RefreshUI();
        }
        else
        {
            Debug.LogWarning("FUCKKKKK.");
        }
    }

    public void RefreshUI()
    {
        currentUpgrades = UpgradeManager.Instance.GetAvailableUpgrades();

        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            var ui = upgradeButtons[i];

            if (i < currentUpgrades.Count)
            {
                var upgrade = currentUpgrades[i];

                ui.button.gameObject.SetActive(true);
                ui.iconImage.sprite = upgrade.icon;
                ui.descriptionText.text = upgrade.description;
                ui.priceText.text = upgrade.cost.ToString();

                bool alreadyPurchased = UpgradeManager.Instance.IsPurchased(upgrade.id);
                ui.button.interactable = !alreadyPurchased;

                ui.button.onClick.RemoveAllListeners();
                if (!alreadyPurchased)
                {
                    int capturedIndex = i;
                    ui.button.onClick.AddListener(() => OnUpgradeSelected(capturedIndex));
                }
            }
            else
            {
                ui.button.gameObject.SetActive(false);
            }
        }
    }


    private void OnUpgradeSelected(int index)
    {
        if (index >= 0 && index < currentUpgrades.Count)
        {
            var upgrade = currentUpgrades[index];
            if (UpgradeManager.Instance.GetCoins() >= upgrade.cost)
            {
                UpgradeManager.Instance.SetCoins(UpgradeManager.Instance.GetCoins() - upgrade.cost);
                UpgradeManager.Instance.PurchaseUpgrade(upgrade.id);
                RefreshUI();
            }
            else
            {
                Debug.Log("Not enough coins!");
            }
        }
    }
}