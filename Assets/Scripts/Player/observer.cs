using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class observer : MonoBehaviour
{
    [SerializeField] List<GameObject> cameralist;

    [SerializeField] GameObject activeCamera;
    // Start is called before the first frame update
    void Start()
    {
        activeCamera.active = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void switchcamera(int camera)
    {
        Debug.Log("clicked");
        activeCamera.active = false;
        activeCamera = cameralist[camera];
        activeCamera.active = true;
    }
}
