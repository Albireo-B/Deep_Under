using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    [SerializeField] public Transform enterwaypoint;

    [SerializeField] public GameObject linkedDoor;
    [SerializeField] public bool isExitDoor = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //called once at start to link it with an oder door
    public void linkdoor(GameObject doortolink)
    {
        linkedDoor = doortolink;
    }
    //called once at start if it is the exit door
    public void makeExitDoor()
    {
        isExitDoor = true;
        transform.gameObject.tag = "door";
    }

}
