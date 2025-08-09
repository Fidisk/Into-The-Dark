using UnityEngine;

public class BootstrapLoader : MonoBehaviour
{
    void Awake()
    {
        UpgradeManager.EnsureExists();
    }
}
