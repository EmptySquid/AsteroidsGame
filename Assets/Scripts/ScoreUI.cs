using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreTextBox, hiscoreTextBox, gameOverScoreText;
    public GameObject scorePanel, celebrate;
    private SpaceShip ship;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ship = FindFirstObjectByType<SpaceShip>();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if(ship != null)
        {
            scoreTextBox.text = ship.score.ToString();
        }
    }

    public void Show(bool celebrateHiscore)
    {
        gameOverScoreText.text = "Score: " + ship.score.ToString();
        hiscoreTextBox.text = "High Score: " + ship.GetHighScore().ToString();

        scorePanel.SetActive(true);
        celebrate.SetActive(celebrateHiscore);
    }

    public void ClickedPlayAgain()
    {
        SceneManager.LoadScene("Asteroids");
    }
    public void ClickedMainMenu()
    {
        SceneManager.LoadScene("AsstoidsUI");
    }

    public void Hide()
    {
        scorePanel.SetActive(false);
    }

}
