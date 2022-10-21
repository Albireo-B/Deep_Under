using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class cameraAnimation : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject monster;
    [SerializeField] private GameObject canvas;

    private List<GameObject> clues;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {

            //CLUES DISPLAY FOR P2 DO NOT DELETE
            /*for (int i = 0; i < clues.Count; i++)
            {
                transform.Find("evidence"+i).position = new Vector3(clues[i].transform.position.x * 4 + 640, clues[i].transform.position.z * 4 + 360, 0);
            }*/
        }

    }

    public void AddClues(GameObject newClue)
    {
        clues.Add(newClue);
    }

    
    public List<GameObject> GetClues()
    {
        return clues;
    }

}
