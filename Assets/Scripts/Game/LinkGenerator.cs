using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkGenerator : MonoBehaviour
{
    [SerializeField] GameObject StartingDoor;
    //create a list of room in wich there is a list of doors of the room
    //there must be a pair number of doors in total in this list
    List<List<GameObject>> roomstolink;

    //this is because unity don't support serialization of list of list
    [SerializeField] List<GameObject> room1;
    [SerializeField] List<GameObject> room2;
    [SerializeField] List<GameObject> room3;
    [SerializeField] List<GameObject> room4;
    [SerializeField] List<GameObject> room5;
    [SerializeField] List<GameObject> room6;
    [SerializeField] List<GameObject> room7;

    // Start is called before the first frame update
    void Start()
    {
        roomstolink = new List<List<GameObject>>();
        //this is because unity don't support serialization of list of list
        roomstolink.Add(room1);
        roomstolink.Add(room2);
        roomstolink.Add(room3);
        roomstolink.Add(room4);
        roomstolink.Add(room5);
        roomstolink.Add(room6);
        roomstolink.Add(room7);

        shufflerooms();
        //link starting room
        roomstolink[0][0].GetComponent<DoorScript>().linkdoor(StartingDoor);
        StartingDoor.GetComponent<DoorScript>().linkdoor(roomstolink[0][0]);
        //make a skeleton of the rooms to be sure every one is link to every others
        for (int i = 0; i < roomstolink.Count-1; i++)
        {
            roomstolink[i][1].GetComponent<DoorScript>().linkdoor(roomstolink[i+1][0]);
            roomstolink[i + 1][0].GetComponent<DoorScript>().linkdoor(roomstolink[i][1]);
        }
        
        while (roomstolink.Count > 1)
        {
            shufflerooms();
            //fill the rest of the room door link one by one
            bool linkdone = false;
            int nbdoorlinked = 0;
            for (int i = 0; i < roomstolink[0].Count; i++)
            {
                //count nb of linked door
                if (roomstolink[0][i].GetComponent<DoorScript>().linkedDoor != null)
                {
                    nbdoorlinked++;
                }
                else
                {
                    if (!linkdone)
                    {
                        linkdone = true;
                        //find a unlinked door on the next room
                        int j = 0;
                        while (roomstolink[1][j].GetComponent<DoorScript>().linkedDoor != null)
                        {
                            j++;
                        }
                        //link the rooms
                        roomstolink[0][i].GetComponent<DoorScript>().linkdoor(roomstolink[1][j]);
                        roomstolink[1][j].GetComponent<DoorScript>().linkdoor(roomstolink[0][i]);
                        nbdoorlinked++;
                        //check if we filled the next room door links
                        int nbdoorlinkedbis = 0;
                        for (int k = 0; k < roomstolink[1].Count; k++)
                        {
                            if (roomstolink[1][k].GetComponent<DoorScript>().linkedDoor != null)
                            {
                                nbdoorlinkedbis++;
                            }
                        }
                        if (nbdoorlinkedbis == roomstolink[1].Count)
                        {
                            roomstolink.RemoveAt(1);
                        }
                    }
                }
            }
            //if the room is totaly linked, remove from the room to link list
            if (nbdoorlinked == roomstolink[0].Count)
            {
                roomstolink.RemoveAt(0);
            }
        }
        //the room will have only one door left to fill
        int doornb = 0;
        while (roomstolink[0][doornb].GetComponent<DoorScript>().linkedDoor != null)
        {
                doornb++;
        }
        roomstolink[0][doornb].GetComponent<DoorScript>().makeExitDoor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void shufflerooms()
    {
        for (int i = 0; i < roomstolink.Count; i++)
        {
            List<GameObject> temp = roomstolink[i];
            int randomIndex = Random.Range(i, roomstolink.Count);
            roomstolink[i] = roomstolink[randomIndex];
            roomstolink[randomIndex] = temp;
        }
    }
}
