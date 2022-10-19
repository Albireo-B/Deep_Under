using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{

    [SerializeField] private GameObject runnerView;
    [SerializeField] private GameObject observerView;
    [SerializeField] private GameObject runnerWaitingText;
    [SerializeField] private GameObject observerWaitingText;

    [SerializeField] private int timerLength;

    private bool gameStarted = false;
    
    [SerializeField] private Text ui_timer;
    [SerializeField] private Text ui_countdown;
    
    private int currentMatchTime;
    private int currentCountdownTime;
    private Coroutine timerCoroutine;
    private Coroutine countdownCoroutine;

    public enum EventCodes : byte
    {
        StartCountdown,
        RefreshTimer
    }

    private void Start() {
        
        if (!PhotonNetwork.IsMasterClient)
        {
            observerView.SetActive(true);
            runnerView.SetActive(false);
            //StartGameCountdown_S();
            //this.photonView.RPC("StartGameCountdownRPC", RpcTarget.All);
        }
        PauseGame();
    }

    private void Update() {
        
        /*if(PhotonNetwork.IsMasterClient)
        {
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
            if (!gameStarted && PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                Debug.Log("2 joueurs");
                gameStarted = true;
                //StartCountdown();
            }
        }*/

    }

    //Sending the refreshtimer information to all clients
    public void StartGameCountdown_S()
    {
        object[] package = new object[] {};

        Debug.Log("raising Event");
        PhotonNetwork.RaiseEvent(
            (byte) EventCodes.StartCountdown,
            package,
            new RaiseEventOptions {Receivers = ReceiverGroup.MasterClient},
            new ExitGames.Client.Photon.SendOptions {Reliability = true}
        );
    }  

    //[PunRPC]
    void StartGameCountdown()
    {
        Debug.Log("GO2");
         if (!gameStarted){
            InitalizeCountdown();
            gameStarted = true;
        }
    }


    private void StartGame()
    {
        ResumeGame();
        InitalizeTimer();
    }

    //NOT USED YET (not sure if it works though)
    public override void OnPlayerLeftRoom(Player other)
    {
        if (PhotonNetwork.IsMasterClient)
        {

        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }




    #region Pause/Play

    void PauseGame ()
        {
            Time.timeScale = 0;
        }
    void ResumeGame ()
        {
            Time.timeScale = 1;
        }

    #endregion

    #region Timer

    private void InitalizeTimer()
    {
        RefreshTimerUI();
        if (PhotonNetwork.IsMasterClient)
        {
            timerCoroutine = StartCoroutine(Timer());
        }
    }

    private void RefreshTimerUI()
    {
        string minutes = (currentMatchTime / 60).ToString("00");
        string seconds = (currentMatchTime % 60).ToString("00");
        ui_timer.text = $"{minutes}:{seconds}";
    }

    //Sending the refreshtimer information to all clients
    public void RefreshTimers_S()
    {
        object[] package = new object[] {currentMatchTime, currentCountdownTime};

        PhotonNetwork.RaiseEvent(
            (byte) EventCodes.RefreshTimer,
            package,
            new RaiseEventOptions {Receivers = ReceiverGroup.All},
            new ExitGames.Client.Photon.SendOptions {Reliability = true}
        );
    }   

    public void RefreshTimers_R(object[] data)
    {
        currentMatchTime = (int) data[0];
        if (currentCountdownTime != -1){
            currentCountdownTime = (int) data[1];
            RefreshCountdownUI();
        } else {
            HideStartPanel();
        }
        RefreshTimerUI();
    }
    
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(1f);

        currentMatchTime += 1;

        RefreshTimerUI();
        RefreshTimers_S();
        timerCoroutine = StartCoroutine(Timer());
    }


    private void EndGame()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        //currentMatchTime = 0;
        //RefreshTimerUI();
    }

    #endregion

    #region Countdown

    private void InitalizeCountdown()
    {
        currentCountdownTime = timerLength;
        RefreshCountdownUI();
        if (PhotonNetwork.IsMasterClient)
        {
            countdownCoroutine = StartCoroutine(Countdown());
        }
    }

    private void RefreshCountdownUI()
    {
        string seconds = (currentCountdownTime % 60).ToString("0");
        ui_countdown.text = $"{seconds}";
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSecondsRealtime(1f);

        currentCountdownTime -= 1;

        RefreshCountdownUI();

        if(currentCountdownTime <= 0)
        {
            countdownCoroutine = null;
            currentCountdownTime = -1;
            HideStartPanel();
            StartGame();
        } else {
            RefreshTimers_S();
            countdownCoroutine = StartCoroutine(Countdown());
        }
    }

    #endregion countdown

    #region EventsReception

    public void OnEvent(EventData photonEvent)
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
    
    }

    #endregion

    private void HideStartPanel()
    {
        ui_countdown.transform.parent.parent.gameObject.SetActive(false);
    }

}
