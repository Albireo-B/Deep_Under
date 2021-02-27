﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (ApplicationModel.ending == 1)
        {
            transform.Find("Text").GetComponent<Text>().text = "You win !";

        }
        else
        {
            transform.Find("Text").GetComponent<Text>().text = "You loose !";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SwitchScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}