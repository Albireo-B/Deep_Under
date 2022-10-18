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
    List<KeyValuePair<string, AudioClip>> listVoicesSubtitles = new List<KeyValuePair<string, AudioClip>>();
    public Text txt;

    private float time = 0.0f;

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
        txt.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (time >= 15)
        {
            time = 0.0f;
            if (listVoicesSubtitles.Count == 0)
                FillAudioList();
            PlayNextClip();
        }
        time += UnityEngine.Time.deltaTime;

    }

    void PlayNextClip()
    {
        audioSource.clip = listVoicesSubtitles[listVoicesSubtitles.Count - 1].Value;
        audioSource.Play();
        txt.text = listVoicesSubtitles[listVoicesSubtitles.Count - 1].Key;
        txt.gameObject.SetActive(true);
        StartCoroutine(EraseText());
        listVoicesSubtitles.RemoveAt(listVoicesSubtitles.Count - 1);
    }
    IEnumerator EraseText()
    {
        yield return new WaitForSeconds(5);
        txt.gameObject.SetActive(false);
    }

    void FillAudioList()
    {
        listVoicesSubtitles.Add(new KeyValuePair<string,AudioClip>("The whole building is collapsing, you need to get out now !",audio1));
        listVoicesSubtitles.Add(new KeyValuePair<string, AudioClip>("It's behind you, hurry up !", audio2));
        listVoicesSubtitles.Add(new KeyValuePair<string, AudioClip>("Find these documents and get the hell out !", audio3));
        listVoicesSubtitles.Add(new KeyValuePair<string, AudioClip>("There is something else here...", audio4));
        listVoicesSubtitles.Add(new KeyValuePair<string, AudioClip>("We're running out of time !", audio5));
        //Shuffle(listVoicesSubtitles);
    }

    void Shuffle(List<KeyValuePair<string, AudioClip>> listToRandomize)
    {
        for (int i = 0; i < listToRandomize.Count; i++)
        {
            int randIndex = Random.Range(0, listToRandomize.Count);
            listToRandomize[i] = listToRandomize[randIndex];
        }
    }
}
