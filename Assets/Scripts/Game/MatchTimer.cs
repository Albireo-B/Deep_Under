using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Photon.Pun.DeepUnder{
    public class MatchTimer : MonoBehaviour
    {
        bool timerStarted = false;
        double timerIncrementValue;
        double startTime;
        ExitGames.Client.Photon.Hashtable CustomeValue;
        
        void Start()
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    CustomeValue = new ExitGames.Client.Photon.Hashtable();
                    startTime = PhotonNetwork.Time;
                    timerStarted = true;
                    CustomeValue.Add("StartTime", startTime);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
                }
                else
                {
                    startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
                    timerStarted = true;
                }
            }
             

        public void SetTimerStarted(bool newVal)
        {
            timerStarted = newVal;
        }

        public double GetGameTime(){
            return PhotonNetwork.Time - startTime;
        }
    }
}

