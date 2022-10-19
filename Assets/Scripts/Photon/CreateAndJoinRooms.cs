using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{

    [SerializeField] private InputField createInput;
    [SerializeField] private InputField joinInput;

    [SerializeField] private int maxPlayers = 2;
    [SerializeField] private bool privateRoom;


    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte) maxPlayers;
        if (privateRoom)
            roomOptions.IsVisible = false;
        PhotonNetwork.CreateRoom(createInput.text, roomOptions, null);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("3DHorrorScenePun");
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void TogglePrivateRoom(bool newVal)
    {
        privateRoom = newVal;
    }


}
