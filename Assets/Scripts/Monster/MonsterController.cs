﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Photon.Pun.DeepUnder
{
    public class MonsterController : MonoBehaviourPun
    {

        NavMeshAgent navMesh;
        public GameObject Player;


        // Start is called before the first frame update
        void Start()
        {
            navMesh = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!GameManager.Instance.CheckGamePaused() && PhotonNetwork.IsMasterClient && Player != null){
                navMesh.SetDestination(Player.transform.position);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            //if what monster touch is a door and the door is on him, he tp
            Debug.Log(Vector3.Distance(other.transform.position, transform.position));
            if (other.tag == "linkedDoor" && Vector3.Distance(other.transform.position, transform.position)<15)
            {
                navMesh.Warp(other.GetComponent<DoorScript>()
                    .linkedDoor.GetComponent<DoorScript>()
                    .enterwaypoint.position);
            }
        }
    }
}
