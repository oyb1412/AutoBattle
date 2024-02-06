using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using UnityEngine.UI;
using System;

public class GotoScene : MonoBehaviour
{
    [SerializeField] Image fadeImage;
    float fadeAlpha;
    private void Awake()
    {
        fadeImage.gameObject.SetActive(true);
        fadeAlpha = 1f;
    }

    private void Start()
    {
        StartCoroutine(FadeOutCorutine());
    }
    public void GoToScene(int index)
    {
        StartCoroutine(FadeOnCorutine(index));
    }

    IEnumerator FadeOnCorutine(int index)
    {
        while (true)
        {
            fadeAlpha += Time.deltaTime * 0.6f;
            fadeImage.color = new Color(0f, 0f, 0f, fadeAlpha);
            yield return null;

            if (fadeAlpha >= 1f)
            {
                fadeAlpha = 1f;
                SceneManager.LoadScene(index);
                break;
            }
        }

    }

    IEnumerator FadeOutCorutine()
    {
        while (true)
        {
            fadeAlpha -= Time.deltaTime * 0.6f;
            fadeImage.color = new Color(0f, 0f, 0f, fadeAlpha);
            yield return null;

            if(fadeAlpha <= 0)
            {
                fadeAlpha = 0;
                break;
            }
        }
    }
}
