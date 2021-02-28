using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundVoiceScript : MonoBehaviour
{

    AudioSource audioSource;
    AudioClip audio1;
    AudioClip audio2;
    AudioClip audio3;
    AudioClip audio4;
    AudioClip audio5;
    List<KeyValuePair<string, AudioClip>> listVoicesSubtitles;
    Text txt;

    // Start is called before the first frame update
    void Start()
    {
        audio1 = Resources.Load<AudioClip>("Batiment");
        audio2 = Resources.Load<AudioClip>("derriere");
        audio3 = Resources.Load<AudioClip>("documents");
        audio4 = Resources.Load<AudioClip>("somethingelse");
        audio5 = Resources.Load<AudioClip>("stop");
        audioSource = GetComponent<AudioSource>();
        FillAudioList();
        StartCoroutine("PlayClip");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator PlayClip()
    {
        for (; ; )
        {
            if (listVoicesSubtitles.Count == 0)
                FillAudioList();
            PlayNextClip();
            print("okok");
            yield return new WaitForSeconds(10f);
        }
    }

    void PlayNextClip()
    {
        audioSource.clip = listVoicesSubtitles[listVoicesSubtitles.Count - 1].Value;
        audioSource.Play();
        showToast(listVoicesSubtitles[listVoicesSubtitles.Count - 1].Key, 5);
        listVoicesSubtitles.RemoveAt(listVoicesSubtitles.Count - 1);
    }

    void FillAudioList()
    {
        listVoicesSubtitles.Add(new KeyValuePair<string,AudioClip>("The whole building is collapsing, you need to get out now !",audio1));
        listVoicesSubtitles.Add(new KeyValuePair<string, AudioClip>("It's behind you, hurry up !", audio2));
        listVoicesSubtitles.Add(new KeyValuePair<string, AudioClip>("Find these documents and get the hell out !", audio3));
        listVoicesSubtitles.Add(new KeyValuePair<string, AudioClip>("There is something else here...", audio4));
        listVoicesSubtitles.Add(new KeyValuePair<string, AudioClip>("We're running out of time !,", audio5));
        Shuffle(listVoicesSubtitles);
    }

    void Shuffle(List<KeyValuePair<string, AudioClip>> listToRandomize)
    {
        for (int i = 0; i < listToRandomize.Count; i++)
        {
            int randIndex = Random.Range(0, listToRandomize.Count);
            listToRandomize[i] = listToRandomize[randIndex];
        }
    }


    void showToast(string text,
    int duration)
    {
        StartCoroutine(showToastCOR(text, duration));
    }

    private IEnumerator showToastCOR(string text,
        int duration)
    {
        Color orginalColor = txt.color;

        txt.text = text;
        txt.enabled = true;

        //Fade in
        yield return fadeInAndOut(txt, true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(txt, false, 0.5f);

        txt.enabled = false;
        txt.color = orginalColor;
    }

    IEnumerator fadeInAndOut(Text targetText, bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = Color.clear;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }
}
