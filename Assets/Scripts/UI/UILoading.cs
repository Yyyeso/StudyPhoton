using TMPro;
using UnityEngine;
using System.Collections;

public class UILoading : UIBase
{
    [SerializeField] private TMP_Text txtLoading;


    public void Setup() => StartCoroutine(nameof(LoadingCoroutine));

    public void Stop() => StopCoroutine(nameof(LoadingCoroutine));

    IEnumerator LoadingCoroutine()
    {
        WaitForSeconds sec = new(0.5f);
        char dot = '.';

        while (true)
        {
            txtLoading.text = "Loading";

            yield return sec;

            txtLoading.text += dot;

            yield return sec;

            txtLoading.text += dot;

            yield return sec;

            txtLoading.text += dot;

            yield return sec;
        }
    }
}