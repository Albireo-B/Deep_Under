using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class cameraAnimation : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject monster;
    [SerializeField] private GameObject canvas;

    private List<GameObject> clues;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Vector3 WorldToScreenSpace(Vector3 worldPos, Camera cam, RectTransform area)
    {
        Vector3 screenPoint = cam.WorldToScreenPoint(worldPos);
        screenPoint.z = 0;
    
        Vector2 screenPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(area, screenPoint, cam, out screenPos))
        {
            return screenPos;
        }
    
        return screenPoint;
    }

    public Vector3 ViewportToCanvasPosition(Canvas canvas, Vector3 viewportPosition)
    {
        var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
        var canvasRect = canvas.GetComponent<RectTransform>();
        var scale = canvasRect.sizeDelta;
        return Vector3.Scale(centerBasedViewPortPosition, scale);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {

            //var viewportPosition = Camera.main.WorldToViewportPoint(player.transform.position);
            //Debug.Log(ViewportToCanvasPosition(canvas,viewportPosition));
            //transform.Find("player").position = ViewportToCanvasPosition(canvas,viewportPosition);



            Mesh planeMesh = canvas.GetComponent<MeshFilter>().mesh;
            Bounds bounds = planeMesh.bounds;
            // size in pixels
            float boundsX = canvas.transform.localScale.x * bounds.size.x;
            float boundsY = canvas.transform.localScale.y * bounds.size.y;
            float boundsZ = canvas.transform.localScale.z * bounds.size.z;

            //position du joueur into position ce rectangle de lma map -> réduit par rapport a mon canvas de screen
            //<Vector from player to Area center> = <Position of AreaCenter> - <Position of player>
            
            Vector3 playerPosRelativeToMap = canvas.transform.position - player.transform.position;
            Debug.Log(playerPosRelativeToMap);


            //Vector3 screenPos = Camera.main.WorldToScreenPoint(player.transform.position);
            


            //transform.Find("player").position = new Vector3(player.transform.position.x*4+640, player.transform.position.z*4+360, 0);
            //transform.Find("monster").position = new Vector3(monster.transform.position.x*4+640, monster.transform.position.z*4+360, 0);
            //CLUES DISPLAY FOR P2 DO NOT DELETE
            /*for (int i = 0; i < clues.Count; i++)
            {
                transform.Find("evidence"+i).position = new Vector3(clues[i].transform.position.x * 4 + 640, clues[i].transform.position.z * 4 + 360, 0);
            }*/
        }

    }

    public void AddClues(GameObject newClue)
    {
        clues.Add(newClue);
    }

    
    public List<GameObject> GetClues()
    {
        return clues;
    }

}
