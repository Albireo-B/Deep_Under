using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public override void OnStartServer()
    {
        Debug.Log("server started");
    }

    public override void OnStopServer()
    {
        Debug.Log("server stopped");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("connected to server");
    }


    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("disconnected from server");
    }
}
