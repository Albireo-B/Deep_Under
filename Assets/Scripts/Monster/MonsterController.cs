using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Photon.Pun.DeepUnder
{
    public class MonsterController : MonoBehaviourPun
    {

        NavMeshAgent navMesh;
        public GameObject Player;


        private NavMeshPath distancePath;

        // Start is called before the first frame update
        void Start()
        {
            distancePath = new NavMeshPath();
            navMesh = GetComponent<NavMeshAgent>();
        }


        public static float GetPathLength( NavMeshPath path )
            {
                float lng = 0.0f;
            
                if (( path.status != NavMeshPathStatus.PathInvalid ))// && ( path.corners.GetLongLength() > 1 ))
                {
                    for ( int i = 1; i < path.corners.Length; ++i )
                    {
                        lng += Vector3.Distance( path.corners[i-1], path.corners[i] );
                    }
                }
            
                return lng;
        }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.IsMasterClient){
                if (!GameManager.Instance.CheckGamePaused() && PhotonNetwork.IsMasterClient && Player != null){
                    navMesh.SetDestination(Player.transform.position);
                    NavMesh.CalculatePath(transform.position, Player.transform.position, NavMesh.AllAreas, distancePath);
                    float pathLength = GetPathLength(distancePath);
                    if(pathLength > 300)
                    {
                        Player.GetComponent<PlayerMovementScript>().SetHeartSpeed(1);
                    }else if (pathLength > 200)
                    {
                        Player.GetComponent<PlayerMovementScript>().SetHeartSpeed(2);
                    }else if (pathLength > 100)
                    {
                        Player.GetComponent<PlayerMovementScript>().SetHeartSpeed(3);
                    } else {
                        Player.GetComponent<PlayerMovementScript>().SetHeartSpeed(4);
                    }

                }
            }

        }
        private void OnTriggerEnter(Collider other)
        {
            //if what monster touch is a door and the door is on him, he tp
            if (other.tag == "linkedDoor")
            {
                navMesh.Warp(other.GetComponent<DoorScript>()
                    .linkedDoor.GetComponent<DoorScript>()
                    .enterwaypoint.position);
            }
        }
    }
}
