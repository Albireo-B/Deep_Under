using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraAnimation : MonoBehaviour
{

    public GameObject Player;
    public List<GameObject> Evidences = new List<GameObject>();
    public GameObject Monster;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Find("player").position = new Vector3(Player.transform.position.x*4+640, Player.transform.position.z*4+360, 0);
        transform.Find("monster").position = new Vector3(Monster.transform.position.x*4+640, Monster.transform.position.z*4+360, 0);
        for (int i = 0; i < Evidences.Count; i++)
        {
            transform.Find("evidence"+i).position = new Vector3(Evidences[i].transform.position.x * 4 + 640, Evidences[i].transform.position.z * 4 + 360, 0);
        }
    }

}
