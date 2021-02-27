using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
	Documentation: https://mirror-networking.com/docs/Articles/Components/NetworkManager.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

namespace Mirror.Examples.Basic
{
    [AddComponentMenu("")]
    public class BasicNetManager : NetworkManager
    {
        int playerNumberPublic=0;

        // Players List to manage playerNumber
        public List<Player> playersList = new List<Player>();

        [Header("Canvas UI")]

        [Tooltip("Assign Main Panel so it can be turned on from Player:OnStartClient")]
        public RectTransform mainPanel;

        [Tooltip("Assign Players Panel for instantiating PlayerUI as child")]
        public RectTransform playersPanel;


        public GameObject boutonStart;
        string tructoprint;

        /// <summary>
        /// Called on the server when a client adds a new player with ClientScene.AddPlayer.
        /// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
            ResetPlayerNumbers();
        }

        /// <summary>
        /// Called on the server when a client disconnects.
        /// <para>This is called on the Server when a Client disconnects from the Server. Use an override to decide what should happen when a disconnection is detected.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            ResetPlayerNumbers();
        }

        private void Update()
        {


        }

        private void OnGUI()
        {
           //GUI.Label(new Rect(10, 10, 100, 20), tructoprint);
        }

        void ResetPlayerNumbers()
        {
            int playerNumber = 0;
            foreach (Player player in playersList)
            {
                player.playerNumber = playerNumber;
                playerNumber++;
            }
            if (playerNumber == 2 && NetworkServer.active)
            {
                boutonStart.SetActive(true);
            } else if (playerNumber != 2 && boutonStart.activeSelf)
            {
                boutonStart.SetActive(false);
            }
            playerNumberPublic = playerNumber;
        }

        public void SwitchScene(string sceneToLoad)
        {
            foreach (Player player in playersList)
            {
                if (player.isServer)
                {
                    player.switchViews = true;
                    player.RpcSyncVarWithClients(player.switchViews);
                }
            }
        }



    }
}
