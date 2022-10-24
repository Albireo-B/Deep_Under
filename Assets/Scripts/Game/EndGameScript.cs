
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Photon.Pun.DeepUnder
{

        
    public class EndGameScript : MonoBehaviour
    {

        [SerializeField] private GameObject endGameCanvas;

        
        private bool gameWon = false;

        // Start is called before the first frame update
        void Start()
        {
            /*if (ApplicationModel.ending == 1)
            {
                endGameCanvas.transform.Find("Text").GetComponent<Text>().text = "You win !";

            }
            else
            {
                endGameCanvas.transform.Find("Text").GetComponent<Text>().text = "You loose !";
            }*/
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void DisplayInfos(double gameTime)
        {
            if (gameWon)
                endGameCanvas.transform.Find("Text").gameObject.GetComponent<Text>().text = "Your friend managed to escape with the documents, you win !";
            else    
                endGameCanvas.transform.Find("Text").gameObject.GetComponent<Text>().text = "Your friend has been caught, you loose !";

            endGameCanvas.transform.Find("TimeText").gameObject.GetComponent<Text>().text = "Time survived : " + Mathf.Floor((float)gameTime/60) + " minutes and " + 
            Mathf.Floor((float)gameTime%60) + " seconds";
            
        }

        public void SetGameWon(bool newGameWon)
        {
            gameWon = newGameWon;

        }

        public void Quit()
        {
            Application.Quit();
        }

        public void SwitchScene(string sceneToLoad)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}