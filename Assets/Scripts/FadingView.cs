using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingView : MonoBehaviour {

    static public FadingView instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    Color bloodViewFlashColor;
    [SerializeField]
    float bloodViewFlashSpeed = 10f;
    [SerializeField]
    float bloodViewFadeOutSpeed = 10f;

    public void FlashBloodView()
    {
        this.StopAllCoroutines();
        this.StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        Image[] images = transform.GetComponentsInChildren<Image>();

        foreach(Image image in images)
        {
            image.color = bloodViewFlashColor;
        }

        while (images[0].color.a > 0)
        {
            //Debug.Log("images[0].color:" + images[0].color);
            Color newColor = Color.Lerp(images[0].color, Color.clear, bloodViewFlashSpeed * Time.deltaTime);
            foreach (Image image in images)
            {
                image.color = newColor;
            }
            yield return null;
        }
    }

    public void FadeOutBloodView()
    {
        this.StopAllCoroutines();
        this.StartCoroutine(BloodFadeOutCoroutine());
    }

    IEnumerator BloodFadeOutCoroutine()
    {
        Image[] images = transform.GetComponentsInChildren<Image>();

        foreach (Image image in images)
        {
            image.color = bloodViewFlashColor;
        }

        while (images[0].color.a < 1)
        {
            Color newColor = Color.Lerp(images[0].color, Color.black, bloodViewFadeOutSpeed * Time.deltaTime);
            foreach (Image image in images)
            {
                image.color = newColor;
            }
            yield return null;
        }
    }

    public void FadeOutView()
    {
        this.StopAllCoroutines();
        this.StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        Image[] images = transform.GetComponentsInChildren<Image>();

        foreach (Image image in images)
        {
            image.color = Color.clear;
        }

        while (images[0].color.a < 1)
        {
            Color newColor = Color.Lerp(images[0].color, Color.black, bloodViewFadeOutSpeed * Time.deltaTime);
            foreach (Image image in images)
            {
                image.color = newColor;
            }
            yield return null;
        }
    }
}
