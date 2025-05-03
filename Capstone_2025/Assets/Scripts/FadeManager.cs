
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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            fadeImage.color = new Color(0, 0, 0, 1); // 씬 시작 시 완전 검정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName, float delay = 0.5f)
    {
        StartCoroutine(FadeRoutine(sceneName, delay));
    }

    private IEnumerator FadeRoutine(string sceneName, float delay)
    {
        yield return StartCoroutine(FadeOut());

        fadeImage.color = new Color(0, 0, 0, 1);
        fadeImage.enabled = true;

        yield return new WaitForSeconds(delay);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f) yield return null;
        yield return new WaitForEndOfFrame();
        op.allowSceneActivation = true;

        yield return null;
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
        fadeImage.color = new Color(0, 0, 0, 1);
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
