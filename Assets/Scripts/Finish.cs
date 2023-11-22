using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Finish : MonoBehaviour
{
    [SerializeField] Text ScoreText;
    [SerializeField] public int Score;
    // Start is called before the first frame update
    void Start()
    {
        
        if (PlayerPrefs.HasKey("gameScore"))  // проверяем, есть ли в сохранении подобная информация
        {
            Score  = PlayerPrefs.GetInt("gameScore");
        }
        else
        {
            Score = 1;
        }
        ScoreText.text = Score.ToString() + "  Score";
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = Score.ToString() + "  Score";
        
    }
     public void StartGame()
    {
        SceneManager.LoadScene(1);

    }
    public void FinishGame()
    {
        SceneManager.LoadScene(1);

    }
    private void OnTriggerEnter(Collider colider)
    {
        if (colider.tag == "Finish")
        {
            Score += 1 * 2;
            Destroy(colider.gameObject);
            PlayerPrefs.SetInt("gameScore", Score);

        }
        if (colider.tag == "Kill")
        {
            FinishGame();

        }

        if (colider.tag == "Gold")
        {
            Score += 3;
            Destroy(colider.gameObject);
            PlayerPrefs.SetInt("gameScore", Score);

        }
        if (colider.tag == "Silver")
        {
            Score += 2;
            Destroy(colider.gameObject);
            PlayerPrefs.SetInt("gameScore", Score);

        }
        if (colider.tag == "Bronze")
        {
            Score += 1;
            Destroy(colider.gameObject);
            PlayerPrefs.SetInt("gameScore", Score);

        }
    }

}

