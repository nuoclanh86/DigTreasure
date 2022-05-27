using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GlobalGameSettings gameSettings;
    public GameObject[] spawnPoints;
    public TextMeshProUGUI timeleftText;

    float timeleft = 0f;

    public static GameManager Instance { get; private set; }
    void Awake()
    {
        //singleton instance
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeleft = gameSettings.timePerMatch;
        timeleftText.text = "Timeleft\n" + (int)timeleft;
    }

    private void Update()
    {
        if (timeleft < 0)
        {
            EndGame();
        }
        else
        {
            timeleft -= Time.deltaTime;
            timeleftText.text = "Timeleft\n" + (int)timeleft;
        }
    }

    public void EndGame()
    {

    }

    public void PauseGame()
    {

    }

    public void ResumeGame()
    {

    }

    public void BackToMainMenu()
    {

    }

    public void LoadMainMenuScene()
    {

    }
}
