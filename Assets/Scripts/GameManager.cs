using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;
using System.Collections;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Photon.Pun.DeepUnder
{
    public class GameManager : MonoBehaviourPunCallbacks//, IOnEventCallback
    {

        [SerializeField] private GameObject runnerView;
        [SerializeField] private GameObject observerView;
        [SerializeField] private GameObject runnerWaitingText;
        [SerializeField] private GameObject observerWaitingText;

        [SerializeField] private int timerLength;

        private bool gameStarted = false;
        private bool gamePaused = false;
        
        [SerializeField] private Text ui_timer;
        [SerializeField] private Text ui_countdown;
        [SerializeField] private Text ui_infoText;

        private int currentMatchTime;
        private int currentCountdownTime;
        private Coroutine timerCoroutine;
        private Coroutine countdownCoroutine;

        public static GameManager Instance;
        
        public enum EventCodes : byte
        {
            StartCountdown,
            RefreshTimer
        }

        private void Awake() {
            if (Instance == null) { Instance = this; } 
        }

        private void Start() {
            
            Hashtable props = new Hashtable
            {
                {DeepUnderGame.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            
            if (!PhotonNetwork.IsMasterClient)
            {
                observerView.SetActive(true);
                runnerView.SetActive(false);
                //StartGameCountdown_S();
                //this.photonView.RPC("StartGameCountdownRPC", RpcTarget.All);
            }
            gamePaused = true;
            //PauseGame();
        }

        private void Update() {
            

        }


        private void StartGame()
        {
            ResumeGame();
            //InitalizeTimer();
        }




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


        #region EventsReception

        /*public void OnEvent(EventData photonEvent)
        {

            if (PhotonNetwork.IsMasterClient)
                Debug.Log("Event received as masterclient");
            else 
                Debug.Log("Event received");
            if (photonEvent.Code >= 200) return;

            EventCodes e = (EventCodes) photonEvent.Code;
            object[] o = (object[]) photonEvent.CustomData;

            switch (e) 
            {
                case EventCodes.RefreshTimer:
                    RefreshTimers_R(o);
                    break;
                case EventCodes.StartCountdown:
                    StartGameCountdown();
                    break;  
            }
        }*/

        #endregion

        private void OnCountdownTimerIsExpired()
        {
            //ui_infoText.text = string.Empty;
            StartGame();
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

        private void EndGame()
        {
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);

            //currentMatchTime = 0;
            //RefreshTimerUI();
        }

        private void HideStartPanel()
        {
            ui_countdown.transform.parent.parent.gameObject.SetActive(false);
        }

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




        #region PUN CALLBACKS

        public override void OnDisconnected(DisconnectCause cause)
        {
            SceneManager.LoadScene("Lobby");
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        //Maybe close the game -> meaning the runner left
       /* public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartCoroutine(SpawnAsteroid());
            }
        }*/

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
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
