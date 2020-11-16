using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFader : MonoBehaviour
{
    private bool tFaded = true;
    [SerializeField] private float duration = 1f;

    public void Fade()
    {
        var canv = GetComponent<CanvasGroup>();

        StartCoroutine(DoFade(canv, canv.alpha, tFaded ? 1 : 0));
        
        tFaded = !tFaded;
    }

    IEnumerator DoFade(CanvasGroup canv, float start, float end)
    {
        float counter = 0f;

        while (counter< duration)
        {
            counter += Time.deltaTime;
            canv.alpha = Mathf.Lerp(start, end, counter / duration);

            yield return null;
        }
    }
}
