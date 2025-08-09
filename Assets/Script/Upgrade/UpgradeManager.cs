using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    public List<Upgrade> allUpgrades = new List<Upgrade>();
    private HashSet<int> purchasedUpgrades = new HashSet<int>();

    private int coins;
    private int currentLevel;

    private string saveFilePath;

    [Header("UI References")]
    [SerializeField] private TMP_Text coinsText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveFilePath = Path.Combine(Application.persistentDataPath, "upgrade_save.json");
        LoadData();
        UpdateCoinsUI(); 
    }

    public List<Upgrade> GetAvailableUpgrades()
    {
        return allUpgrades
            .Where(upg => !purchasedUpgrades.Contains(upg.id))
            .Where(upg => upg.prerequisiteId == 0 || purchasedUpgrades.Contains(upg.prerequisiteId))
            .OrderBy(x => Random.value)
            .Take(3)
            .ToList();
    }

    public void PurchaseUpgrade(int id)
    {
        if (allUpgrades.Any(upg => upg.id == id))
        {
            purchasedUpgrades.Add(id);
            SaveDataF();
        }
    }

    public bool IsPurchased(int id) => purchasedUpgrades.Contains(id);

    public int GetCoins() => coins;

    public void SetCoins(int value) 
    { 
        coins = value; 
        UpdateCoinsUI(); 
        SaveDataF(); 
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinsUI();
        SaveDataF();
    }

    public int GetCurrentLevel() => currentLevel;
    public void SetCurrentLevel(int value) { currentLevel = value; SaveDataF(); }

    [System.Serializable]
    private class SaveData
    {
        public List<int> purchasedUpgrades;
        public int coins;
        public int currentLevel;
    }

    private void SaveDataF()
    {
        var data = new SaveData
        {
            purchasedUpgrades = purchasedUpgrades.ToList(),
            coins = coins,
            currentLevel = currentLevel
        };

        File.WriteAllText(saveFilePath, JsonUtility.ToJson(data));
    }

    private void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            var json = File.ReadAllText(saveFilePath);
            var data = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();

            purchasedUpgrades = data.purchasedUpgrades != null
                ? new HashSet<int>(data.purchasedUpgrades)
                : new HashSet<int>();

            coins = data.coins;
            currentLevel = data.currentLevel;
        }
        else
        {
            purchasedUpgrades.Clear();
            coins = 0;
            currentLevel = 1;
        }
    }

    public static void EnsureExists()
    {
        if (Instance == null)
        {
            var prefab = Resources.Load<UpgradeManager>("UpgradeManager");
            if (prefab != null)
            {
                Instantiate(prefab);
            }
        }
    }

    public void ResetData()
    {
        purchasedUpgrades.Clear();
        coins = 0;
        currentLevel = 1;

        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        SaveDataF();
        UpdateCoinsUI(); 
    }

    private void UpdateCoinsUI()
    {
        if (coinsText != null)
            coinsText.text = $"Coins: {coins}";
    }
}
