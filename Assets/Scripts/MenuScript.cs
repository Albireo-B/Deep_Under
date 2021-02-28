using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    Ray ray;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            print(hit.collider.name);
        }
    }

    public void SwitchScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }



}

public class ApplicationModel
{
    static public int ending = 0;    // this is reachable from everywhere
}