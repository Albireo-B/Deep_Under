using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{

    void handleMovement()
    {
        if (isLocalPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal * 0.1f, moveVertical * 0.1f, 0);
            transform.position = transform.position + movement;
        }
    }

    private void Update()
    {
        handleMovement();
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Sending hola to server");
            Hola();
        }
    }

    [Command]
    void Hola()
    {
        Debug.Log("Received Hola from clien");
    }
}

