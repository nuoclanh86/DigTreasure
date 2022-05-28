using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GlobalGameSettings gameSettings;
    public GameObject[] spawnPoints;
    public GameObject ingameUI;
    public GameObject player;

    float timeleft = 0f;

    [Header("TreasureChest")]
    public GameObject treasureChestObj;
    public float radiusSpawnChest = 22f;

    //[Header("Photon PUN")]
    //public PhotonView photonView;

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
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            if (timeleft > 0)
            {
                timeleft -= Time.deltaTime;
                if (PhotonNetwork.InRoom)
                {
                    ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
                    hash.Add("curTimeleft", timeleft);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                }
                else
                {
                    DisplayTimeleft(timeleft);
                }
            }
        }

        if (timeleft < 0)
        {
            EndGame("Time Out");
        }
    }

    private void DisplayTimeleft(float timeleft)
    {
        ingameUI.GetComponent<IngameUI>().UpdateTimeLeft((int)timeleft);
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
        SpawnTreasureChests();
    }

    public void BackToMainMenu()
    {

    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    void SpawnTreasureChests()
    {
        GameObject[] treasureChests = GameObject.FindGameObjectsWithTag("TreasureChest");
        //delete old chests
        if (treasureChests.Length != 0)
        {
            foreach (GameObject chest in treasureChests)
            {
                if (chest != null)
                    Destroy(chest);
            }
        }

        //init new chests
        for (int i = 0; i < gameSettings.maxTreasureChest; i++)
        {
            Vector3 pos = new Vector3(0, 0, 0);
            pos = Random.insideUnitCircle * radiusSpawnChest;
            //swap y->z , y=0f:under ground
            pos.y += pos.z;
            pos.z = pos.y - pos.z;
            pos.y = 0f;

            pos += transform.position;
            GameObject treasureChest = Instantiate(treasureChestObj);
            treasureChest.transform.position = pos;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 1);
        Gizmos.DrawSphere(transform.position, radiusSpawnChest);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("Player: " + targetPlayer + " changed " + changedProps);
        if (changedProps["curTimeleft"] != null)
        {
            DisplayTimeleft((float)changedProps["curTimeleft"]);
        }
    }
}
