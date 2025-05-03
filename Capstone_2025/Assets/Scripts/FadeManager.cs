using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void FadeToScene(string sceneName, float delay = 2f)
    {
        StartCoroutine(FadeRoutine(sceneName, delay));
    }

    private IEnumerator FadeRoutine(string sceneName, float delay)
    {
        yield return StartCoroutine(FadeOut());
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(0.1f); // 씬 로드 후 한 프레임 대기
        yield return StartCoroutine(FadeIn());
    }

    public IEnumerator FadeOut()
    {
        float t = 0;
        Color color = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        float t = 0;
        Color color = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }
}
