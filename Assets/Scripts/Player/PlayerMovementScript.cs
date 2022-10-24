using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System;

namespace Photon.Pun.DeepUnder
{
    public class PlayerMovementScript : MonoBehaviourPun
    {

        float horizontal;
        float vertical;
        Rigidbody body;
        public Canvas ui;
        public float speed = 5.0f;

        public Canvas camera2;
        public float gameTimeMultiplier = 1;
        Animator animator;
        GameObject movingBody;
        GameObject EvidenceInFront = null;
        bool atTheDoor = false;
        AudioSource monsterSound;
        private bool musicFadeOutEnabled = false;
        AudioClip deathScream;
        AudioSource playerAudioSource;
        private bool pickingClue;
        private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
        {
            pickingClue = false;
            gameManager = GameManager.Instance;
            playerAudioSource = GetComponent<AudioSource>();
            deathScream = Resources.Load<AudioClip>("DeathScream");
            monsterSound = transform.Find("monsterSound").GetComponent<AudioSource>();
            movingBody = transform.Find("DeepUnderCharacter").gameObject;
            animator = transform.Find("DeepUnderCharacter").GetComponent<Animator>();
            body = GetComponent<Rigidbody>();

            
        }

        // Update is called once per frame
        void Update()
        {

            if (!gameManager.CheckGamePaused() && PhotonNetwork.IsMasterClient && !gameManager.CheckGameEnded()){

                
                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");
                if (Input.GetKey("space") && atTheDoor && gameManager.GetNumberOfProofsFound() == gameManager.GetGameCluesNb())
                {
                    ApplicationModel.ending = 1;
                    gameManager.photonView.RPC("EndGame",RpcTarget.All,true);
                }
                if (Input.GetKey("space") && EvidenceInFront != null)
                {
                    if (ui.transform.Find("loading").GetComponent<UnityEngine.UI.Slider>().value >= 100)
                    {
                        pickingClue = false;
                        playerAudioSource.Stop();
                        ui.transform.Find("DownPanel").Find("ClueText").GetComponent<UnityEngine.UI.Text>().text = "";
                        gameManager.AddProofFound();
                        ui.transform.Find("TopRightPanel").Find("Clues").GetComponent<UnityEngine.UI.Text>().text = "Clues : "+ gameManager.GetNumberOfProofsFound() + " / " + gameManager.GetGameCluesNb();
                        gameManager.photonView.RPC("ChangeObjectTag", RpcTarget.All, EvidenceInFront.GetComponent<PhotonView>().ViewID, "Untagged");
                        int index = gameManager.GetClues().FindIndex(d => d == EvidenceInFront.gameObject);
                        EvidenceInFront = null;
                    }
                    else
                    {
                        pickingClue = true;
                        ui.transform.Find("loading").gameObject.SetActive(true);
                        ui.transform.Find("loading").GetComponent<UnityEngine.UI.Slider>().value += 0.3f;
                        if (!playerAudioSource.isPlaying)
                            playerAudioSource.Play();
                    }

                }
                else
                {
                    playerAudioSource.Stop();
                    ui.transform.Find("loading").gameObject.SetActive(false);
                    ui.transform.Find("loading").GetComponent<UnityEngine.UI.Slider>().value = 0;
                }

                if (musicFadeOutEnabled)
                {
                    if (monsterSound.volume <= 0.1f)
                    {
                        monsterSound.Stop();
                        musicFadeOutEnabled = false;
                    }
                    else
                    {
                        float newVolume = monsterSound.volume - (0.1f * Time.deltaTime);  //change 0.1f to something else to adjust the rate of the volume dropping
                        if (newVolume < 0f)
                        {
                            newVolume = 0f;
                        }
                        monsterSound.volume = newVolume;
                    }
                }
            }
        }


        private void FixedUpdate()
        {
            Vector3 normalizedDirection = new Vector3(horizontal, 0, vertical).normalized;
            body.velocity = normalizedDirection * speed;
            if (gameManager.CheckGameEnded()){
                if (!animator.GetBool("Death")){
                    animator.SetBool("Death",true);
                    GetComponent<Animation>().Play();
                }
            } else 
            {
                int animationState;
                if (body.velocity != Vector3.zero)
                {
                    animationState = 1;
                    movingBody.transform.rotation = Quaternion.LookRotation(body.velocity, Vector3.up);
                }
                else if (pickingClue)
                {
                    animationState = 2;
                }
                else
                {
                    animationState = 0;
                }
                if (animator.GetInteger("AnimationState")!=animationState){
                    animator.SetInteger("AnimationState",animationState);
                }

            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!gameManager.CheckGameEnded()){
                if (other.tag == "door")
                {
                    atTheDoor = true;
                    if (gameManager.GetNumberOfProofsFound() == gameManager.GetGameCluesNb())
                    {
                        ui.transform.Find("DownPanel").Find("ClueText").GetComponent<UnityEngine.UI.Text>().text = "press space to exit";
                    }
                    else
                    {
                        ui.transform.Find("DownPanel").Find("ClueText").GetComponent<UnityEngine.UI.Text>().text = "not enough evidences to exit";
                    }
                    ui.transform.Find("DownPanel").Find("ClueText").gameObject.SetActive(true);
                }
                else if (other.tag == "Clue")
                {
                    EvidenceInFront = other.gameObject;
                    ui.transform.Find("DownPanel").Find("ClueText").GetComponent<UnityEngine.UI.Text>().text = "press space to search for evidence";
                    ui.transform.Find("DownPanel").Find("ClueText").gameObject.SetActive(true);
                }
                else if (other.tag == "linkedDoor")
                {
                    transform.position = other.GetComponent<DoorScript>()
                        .linkedDoor.GetComponent<DoorScript>()
                        .enterwaypoint.position;
                }
                else if (other.tag == "Monster")
                {
                    if (!monsterSound.isPlaying)
                    {
                        musicFadeOutEnabled = false;
                        monsterSound.volume = 1;
                        monsterSound.Play();
                        monsterSound.loop = true;
                    }
                }
            }
            
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Monster")
            {
                musicFadeOutEnabled = true;
            }
            else
            {
                pickingClue = false;
                ui.transform.Find("DownPanel").Find("ClueText").gameObject.SetActive(false);
                EvidenceInFront = null;
                atTheDoor = false;
            }
            

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "Monster")
            {
                ApplicationModel.ending = 0;
                gameManager.photonView.RPC("EndGame",RpcTarget.All,false);
            }
        }
    }
}
