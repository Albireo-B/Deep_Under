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
        public float gameTimeMultiplier = 1;
        Animator animator;
        GameObject movingBody;
        GameObject EvidenceInFront = null;
        bool atTheDoor = false;
        AudioClip hearthSound;
        private bool musicFadeOutEnabled = false;
        AudioClip deathScream;
        AudioSource playerAudioSource;
        AudioSource cluesAudioSource;
        private bool pickingClue;
        private GameManager gameManager;
        private float currentHeartSpeed;

        // Start is called before the first frame update
        void Start()
        {
            currentHeartSpeed = 1;
            pickingClue = false;
            gameManager = GameManager.Instance;
            cluesAudioSource = transform.parent.gameObject.GetComponent<BackgroundVoiceScript>().audioSourceClues;
            playerAudioSource = transform.parent.gameObject.GetComponent<BackgroundVoiceScript>().audioSourceHeart;
            deathScream = Resources.Load<AudioClip>("DeathSound");
            hearthSound = Resources.Load<AudioClip>("HeartBeat");
            movingBody = transform.Find("DeepUnderCharacter").gameObject;
            animator = transform.Find("DeepUnderCharacter").GetComponent<Animator>();
            body = GetComponent<Rigidbody>();
            playerAudioSource.clip = hearthSound;
            playerAudioSource.loop = true;
            playerAudioSource.Play();

            
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
                        cluesAudioSource.Stop();
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
                        if (!cluesAudioSource.isPlaying)
                            cluesAudioSource.Play();
                    }

                }
                else
                {
                    cluesAudioSource.Stop();
                    ui.transform.Find("loading").gameObject.SetActive(false);
                    ui.transform.Find("loading").GetComponent<UnityEngine.UI.Slider>().value = 0;
                }

            }
        }


        private void FixedUpdate()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Vector3 normalizedDirection = new Vector3(horizontal, 0, vertical).normalized;
                body.velocity = normalizedDirection * speed;
                if (gameManager.CheckGameEnded()){
                    playerAudioSource.Stop();
                    if (!gameManager.GetGameWon())
                    {
                        playerAudioSource.Stop();
                        if (!animator.GetBool("Death")){
                            animator.SetBool("Death",true);
                            playerAudioSource.clip = deathScream;
                            playerAudioSource.Play();
                            playerAudioSource.loop = false;
                            playerAudioSource.pitch = 1;
                            GetComponent<Animation>().Play();
                        }
                    }

                } else 
                {
                    float newSpeed;
                    if (currentHeartSpeed == 1)
                    {
                        newSpeed = 1;
                    } else {
                        newSpeed =  1 + currentHeartSpeed/2;
                    }
                    if (playerAudioSource.pitch != newSpeed){
                        playerAudioSource.pitch = newSpeed;
                        playerAudioSource.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", 1f / newSpeed);
                    }
                    if (!playerAudioSource.isPlaying){
                        playerAudioSource.Play();
                        
                        Debug.Log("dfsdf");
                    }
                        //change countdowncanvas trarget display



                    //ANIMATIONS REGION
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
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (PhotonNetwork.IsMasterClient)
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

                    /* //RELATED TO MONSTER SOUND
                    else if (other.tag == "Monster")
                    {
                        if (!monsterSound.isPlaying)
                        {
                            musicFadeOutEnabled = false;
                            monsterSound.volume = 1;
                            monsterSound.Play();
                            monsterSound.loop = true;
                        }
                    }*/
                }
            }
            
        }
        private void OnTriggerExit(Collider other)
        {
            if (PhotonNetwork.IsMasterClient)
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
            

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (collision.collider.tag == "Monster")
                {
                    ApplicationModel.ending = 0;
                    gameManager.photonView.RPC("EndGame",RpcTarget.All,false);
                }
            }
        }


        public void SetHeartSpeed(int newSpeed)
        {
            if (currentHeartSpeed != newSpeed)
            {
                currentHeartSpeed = newSpeed;
            }
        }
    }
}
