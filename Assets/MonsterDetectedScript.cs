using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDetectedScript : MonoBehaviour
{

    AudioSource audioSource;
    private bool musicFadeOutEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (musicFadeOutEnabled)
        {
            if (audioSource.volume <= 0.1f)
            {
                audioSource.Stop();
                musicFadeOutEnabled = false;
            }
            else
            {
                float newVolume = audioSource.volume - (0.01f * Time.deltaTime);  //change 0.01f to something else to adjust the rate of the volume dropping
                if (newVolume < 0f)
                {
                    newVolume = 0f;
                }
                audioSource.volume = newVolume;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            if (!audioSource.isPlaying) {
                musicFadeOutEnabled = false;
                audioSource.Play();
                audioSource.loop = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Monster")
        {
            FadeOutMusic();
        }
    }

    public void FadeOutMusic()
    {
        musicFadeOutEnabled = true;
    }


}
