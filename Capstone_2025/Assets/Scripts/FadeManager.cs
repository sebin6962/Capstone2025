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

    public void FadeToScene(string sceneName, float delay = 1f)
    {
        StartCoroutine(FadeRoutine(sceneName, delay));
    }

    private IEnumerator FadeRoutine(string sceneName, float delay)
    {
        yield return StartCoroutine(FadeOut());

        //�� �ε� ���� Ȯ���� ���� ����
        var color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(0.1f); // �� �ε� �� �� ������ ���

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
        //���� �ʱ�ȭ
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
