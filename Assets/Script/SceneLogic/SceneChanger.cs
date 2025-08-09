using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneChanger : MonoBehaviour
{
#if UNITY_EDITOR
    public SceneAsset targetScene; 
#endif

    [SerializeField] private string targetSceneName; 

    public void LoadSceneWithLoadingScreen()
    {
        LoadingScreen.sceneName = targetSceneName;
        SceneManager.LoadScene("LoadingScreen");
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (targetScene != null)
        {
            targetSceneName = targetScene.name;
        }
    }
#endif
}
