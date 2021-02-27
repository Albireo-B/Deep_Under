using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementScript : MonoBehaviour
{

    float horizontal;
    float vertical;
    Rigidbody body;
    public Canvas ui;
    public float speed = 5.0f;
    public int nbProofFound;


    // Start is called before the first frame update
    void Start()
    {
        nbProofFound = 0;
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

    }

    private void FixedUpdate()
    {
        body.velocity = new Vector3(horizontal, 0,  vertical).normalized * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "door")
        {
            if (nbProofFound>2)
            {
                ui.transform.Find("text").GetComponent<UnityEngine.UI.Text>().text = "press space to exit";
            }
            else
            {
                ui.transform.Find("text").GetComponent<UnityEngine.UI.Text>().text = "not enough evidences to exit";
            }
            ui.transform.Find("text").gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "door")
        {
            ui.transform.Find("text").gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Monster")
        {
            //ApplicationModel.ending = 0;
            //SceneManager.LoadScene("EndGame");
            Debug.Log("PAN T MORT");
        }
    }




    //TODO : créer portes de sortie collider, créer scenes de win et loose, créee trucs trouvables et ramassables, me sucer 4 fois. #DébilitéDeGuillaume
}
