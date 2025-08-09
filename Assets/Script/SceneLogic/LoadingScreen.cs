using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public static string sceneName;
    public Slider progressBar;
    public TextMeshProUGUI tipText; 

    [TextArea]
    public string[] tips = new string[]
    {
        "Tip: Collected stars add to your energy too.",
        "Tip: The portal also refills your energy bar slowly.",
        "Tip: Your energy bar depletes faster the longer you stay.",
    };

    void Start()
    {
        ShowRandomTip();
        StartCoroutine(LoadSceneAsync());
    }

    void ShowRandomTip()
    {
        if (tips.Length > 0 && tipText != null)
        {
            int randomIndex = Random.Range(0, tips.Length);
            tipText.text = tips[randomIndex];
        }
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null) progressBar.value = progress;

            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
