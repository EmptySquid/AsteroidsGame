using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour

{

    public void ClickPlay()
    {
        SceneManager.LoadScene("AsteroidBelter");
    }
        
    public void ClickPlayPlayground()
    {
        SceneManager.LoadScene("Playground");
    }
    public void LoadNewScene(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void ClickQuit()
    {
        Application.Quit();
    }

    public void KillPlayer(GameObject player)
    {
        player.gameObject.SetActive(false);
        Debug.Log("You Lose!");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


}
