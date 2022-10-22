using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Photon.Pun.DeepUnder{
    public class MatchTimer : MonoBehaviour
    {
        bool timerStarted = false;
        double timerIncrementValue;
        double startTime = -1;
        ExitGames.Client.Photon.Hashtable CustomeValue;
        
        void Start()
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    CustomeValue = new ExitGames.Client.Photon.Hashtable();
                    startTime = PhotonNetwork.Time;
                    timerStarted = true;
                    CustomeValue.Add("TimerStartTime", startTime);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
                }
                else
                {
                    if (PhotonNetwork.CurrentRoom.CustomProperties["TimerStartTime"] != null){
                        startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["TimerStartTime"].ToString());
                    }
                    timerStarted = true;
                }
            }
             

        public void SetTimerStarted(bool newVal)
        {
            timerStarted = newVal;
        }

        public double GetGameTime(){
            if (startTime == -1){
                startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["TimerStartTime"].ToString());
            }
            return PhotonNetwork.Time - startTime;
        }
    }
}

