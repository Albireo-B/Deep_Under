using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class networkbehaviour : NetworkBehaviour
{

    
    [SyncVar] bool switchViews = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    //this will sync var from server to all clients by calling the "SyncVarWithClientsRpc" funtion on the clients with the value of the variable "varToSync" equals to the value of "example1"
    [TargetRpc]
    void RpcSyncVarWithClient(NetworkConnection conn)
    {
        switchViews = true;
    }


    // Update is called once per frame
    void Update()
    {

        if (switchViews && !transform.parent.parent.Find("GamePart").gameObject.activeSelf && !transform.parent.parent.Find("MapPart").gameObject.activeSelf)
        {
            transform.parent.parent.Find("LobbyPart").gameObject.SetActive(false);
            if (NetworkClient.isConnected && NetworkClient.isLocalClient)
            {
                transform.parent.parent.Find("GamePart").gameObject.SetActive(true);
            }
            else
            {
                transform.parent.parent.Find("MapPart").gameObject.SetActive(true);
            }
        }
    }

    public void SwitchScene(string sceneToLoad)
    {
        //SceneManager.LoadScene(sceneToLoad);
        switchViews = true;
        RpcSyncVarWithClient(connectionToClient);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), switchViews.ToString());
    }
}
