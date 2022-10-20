using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Photon.Pun.DeepUnder
{
    public class MonsterController : MonoBehaviour
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
            if (!GameManager.Instance.CheckGamePaused()){
                navMesh.SetDestination(Player.transform.position);
            }
        }
    }
}
