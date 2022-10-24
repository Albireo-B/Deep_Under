using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Photon.Pun.DeepUnder
{
    public class GameManager : MonoBehaviourPunCallbacks//, IOnEventCallback
    {
        [Header("GameViews")]
        [SerializeField] private GameObject runnerView;
        [SerializeField] private GameObject observerView;

        private bool gamePaused = false;
        
        [Header("InfoText")]
        [SerializeField] private Text ui_infoText;

        public static GameManager Instance;

        [Header("Game Start")]
        [SerializeField] private GameObject countdownCanvas;
        
        [Header("Clues")]
        [SerializeField] private GameObject possibleClues;
        [SerializeField] private int gameCluesNb;
        private List<GameObject> clues;
        private int nbProofFound;

        [Header("Game End")]
        [SerializeField] private GameObject endGameCanvas;
        [SerializeField] private GameObject deathScreen;
        private bool gameEnded;
        
        [Header("Menu")]
        [SerializeField] private GameObject menuCanvas;

        private void Awake() {
            if (Instance == null) { Instance = this; } 
        }
        private void Start() {
            
            gameEnded = false;

            clues = new List<GameObject>();

            Hashtable props = new Hashtable
            {
                {DeepUnderGame.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            
            if (!PhotonNetwork.IsMasterClient)
            {
                runnerView.transform.Find("HUD").gameObject.SetActive(false);

            } else {
                
                nbProofFound = 0;
                observerView.SetActive(false);
            }
            gamePaused = true;
        }

        private void Update() {
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (menuCanvas.activeSelf)
                    menuCanvas.SetActive(false);
                else
                    menuCanvas.SetActive(true);
            }
        }

        #region Menu

        public void LeaveGame()
        {
            PhotonNetwork.LeaveRoom();
            //SceneManager.LoadScene("Lobby");
        }

        public void Back()
        {
            menuCanvas.SetActive(false);
        }

        #endregion

        #region Clues
        public int GetGameCluesNb()
        {
            return gameCluesNb;
        }

        public int GetNumberOfProofsFound()
        {
            return nbProofFound;
        }

        public void AddProofFound()
        {
            nbProofFound++;
        }

        
        private void AddClues(GameObject newClue)
        {
            clues.Add(newClue);
        }

        
        public List<GameObject> GetClues()
        {
            return clues;
        }

        private void SpawnClues()
        {
                for (int i = 0; i < gameCluesNb; i++)
                {
                    int randomChildIdx;
                    Transform randomChild;
                    do
                    {
                        randomChildIdx = UnityEngine.Random.Range(0, possibleClues.transform.childCount-1);
                        randomChild = possibleClues.transform.GetChild(randomChildIdx);
                    } while (clues.FindIndex(d => d == randomChild.gameObject) != -1);
                    Vector3 collidSize = randomChild.GetComponent<BoxCollider>().size;
                    photonView.RPC("ChangeObjectTag", RpcTarget.All, randomChild.gameObject.GetComponent<PhotonView>().ViewID, "Clue");
                }
        }

        [PunRPC]
        public void ChangeObjectTag(int gameObjectID, string newTag)
        {
            if (!PhotonNetwork.IsMasterClient && newTag == "Untagged")
            {
                AddClues(PhotonNetwork.GetPhotonView(gameObjectID).gameObject);
            }
            PhotonNetwork.GetPhotonView(gameObjectID).gameObject.tag = newTag;
            if (newTag == "Clue")
            {
                PhotonNetwork.GetPhotonView(gameObjectID).gameObject.transform.Find("ClueIconTransform").gameObject.SetActive(true);
            } else if (newTag == "Untagged")
            {
                PhotonNetwork.GetPhotonView(gameObjectID).gameObject.transform.Find("ClueIconTransform").gameObject.SetActive(false);
            }
        }

        #endregion

        #region Game end

        public bool CheckGameEnded()
        {
            return gameEnded;
        }

        [PunRPC]
        public void EndGame(bool newValGameWon)
        {
            gameEnded = true;
            GetComponent<MatchTimer>().SetTimerStarted(false);
            SetGameWon(newValGameWon);
            double gameTime = GetComponent<MatchTimer>().GetGameTime();
            if (PhotonNetwork.IsMasterClient)
            {
                runnerView.transform.Find("Player").gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                //To avoid the monster collider to push the player or force itself into walls
                Destroy(runnerView.transform.Find("Monster").gameObject);
                runnerView.transform.Find("HUD").gameObject.SetActive(false);
                runnerView.GetComponent<BackgroundVoiceScript>().enabled = false;
                //monster voices.
                deathScreen.GetComponent<Animation>().Play();
            } else {
                Destroy(runnerView.transform.Find("Monster").gameObject);
                Destroy(runnerView.transform.Find("Player").gameObject);
                endGameCanvas.SetActive(true);
                observerView.transform.Find("CanvasObserver").gameObject.SetActive(false);
                foreach (var clue in clues)
                {
                    clue.transform.Find("ClueIconTransform").gameObject.SetActive(false);
                }
            }
            GetComponent<EndGameScript>().DisplayInfos(gameTime);
        }

        public void SetGameWon(bool newVal)
        {
            GetComponent<EndGameScript>().SetGameWon(newVal);
        }

        #endregion

        #region Pause/Play

        void PauseGame ()
            {
                gamePaused = true;
            }
        void ResumeGame ()
            {
                gamePaused = false;
            }
        public bool CheckGamePaused(){return gamePaused;}

        #endregion

        #region GameStart

        private void OnCountdownTimerIsExpired()
        {
            countdownCanvas.SetActive(false);
            ResumeGame();
            if (PhotonNetwork.IsMasterClient)
                SpawnClues();
        }

        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(DeepUnderGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    if ((bool) playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }
            ui_infoText.text = string.Empty;
            return true;
        }


        #endregion

        #region Enable/Disable

        public override void OnEnable()
        {
            base.OnEnable();
            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnDisconnected(DisconnectCause cause)
        {
            SceneManager.LoadScene("Lobby");
            
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            //SceneManager.LoadScene("Lobby");
            //CheckEndOfGame();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start
            int startTimestamp;
            bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

            if (changedProps.ContainsKey(DeepUnderGame.PLAYER_LOADED_LEVEL))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                    if (!startTimeIsSet)
                    {
                        CountdownTimer.SetStartTime();
                    }
                }
                else
                {
                    // not all players loaded yet. wait:
                    Debug.Log("setting text waiting for players! ",this.ui_infoText);
                    ui_infoText.text = "Waiting for other players...";
                }
            }
        
        }

        #endregion

    }
}
