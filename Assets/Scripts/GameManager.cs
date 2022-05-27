using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GlobalGameSettings gameSettings;
    public GameObject[] spawnPoints;
    public GameObject ingameUI;
    public GameObject player;
    public GameObject treasureManager;

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
        StartNewGame();
    }

    private void Update()
    {
        if (timeleft < 0)
        {
            EndGame("Time Out");
        }
        else
        {
            timeleft -= Time.deltaTime;
            ingameUI.GetComponent<IngameUI>().UpdateTimeLeft(timeleft);
        }
    }

    public void EndGame(string resultGame)
    {
        Time.timeScale = 0;
        ingameUI.GetComponent<IngameUI>().ShowEndGameUI(resultGame);
    }

    public void StartNewGame()
    {
        Time.timeScale = 1;
        timeleft = gameSettings.timePerMatch;
        ingameUI.GetComponent<IngameUI>().ResetGameUI();
        player.GetComponent<PlayerController>().RandomPlayerPosition();
        treasureManager.GetComponent<TreasureManager>().SpawnTreasureChests();
    }

    public void BackToMainMenu()
    {

    }

    public void LoadMainMenuScene()
    {

    }
}
