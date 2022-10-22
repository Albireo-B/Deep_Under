
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Photon.Pun.DeepUnder
{

        
    public class EndGameScript : MonoBehaviour
    {

        
        private bool gameWon = false;

        // Start is called before the first frame update
        void Start()
        {
            if (ApplicationModel.ending == 1)
            {
                transform.Find("Text").GetComponent<Text>().text = "You win !";

            }
            else
            {
                transform.Find("Text").GetComponent<Text>().text = "You loose !";
            }
        }

        // Update is called once per frame
        void Update()
        {
            
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
            PhotonNetwork.LeaveLobby();
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}