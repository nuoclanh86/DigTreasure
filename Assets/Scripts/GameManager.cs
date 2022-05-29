using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GlobalGameSettings gameSettings;
    public GameObject ingameUI;
    public GameObject player;

    float m_timeleft = 0f;

    [Header("TreasureChest")]
    public GameObject treasureChestObj;
    GameObject spawnPointCoordinates;

    //[Header("Photon PUN")]
    //public PhotonView photonView;

    bool m_masterClientResetGame = false;

    public static GameManager Instance { get; private set; }
    void Awake()
    {
        //singleton instance
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnPointCoordinates = GameObject.FindGameObjectWithTag("SpawnPointCoordinates");
        if (spawnPointCoordinates == null) Debug.LogError("Could not found SpawnPointCoordinates");
        m_timeleft = gameSettings.timePerMatch;
        StartNewGame();
    }

    private void Update()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            if (m_timeleft > 0)
            {
                m_timeleft -= Time.deltaTime;
                if (PhotonNetwork.InRoom)
                {
                    ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
                    hash.Add("curTimeleft", m_timeleft);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                }
                else
                {
                    DisplayTimeleft(m_timeleft);
                }
            }
        }

        if (m_masterClientResetGame == true)
        {
            //Debug.Log("m_masterClientResetGame : " + m_masterClientResetGame + " IsMasterClient: " + PhotonNetwork.IsMasterClient);
            if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient && photonView.IsMine)
                StartNewGame();
            m_masterClientResetGame = false;
        }
    }

    private void DisplayTimeleft(float timeleft)
    {
        ingameUI.GetComponent<IngameUI>().UpdateTimeLeft((int)timeleft);
        if (timeleft < 0)
        {
            EndGame("Time Out");
        }
    }

    public void EndGame(string resultGame)
    {
        Time.timeScale = 0.01f;
        ingameUI.GetComponent<IngameUI>().ShowEndGameUI(resultGame);
    }

    public void StartNewGame()
    {
        if ((PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && photonView.IsMine))
        {
            //Debug.Log("update m_masterClientResetGame : " + true);
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add("masterClientResetGame", true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        Time.timeScale = 1;
        m_timeleft = gameSettings.timePerMatch;
        ingameUI.GetComponent<IngameUI>().ResetGameUI();
        player.GetComponent<PlayerController>().RandomPlayerPosition();
        SpawnTreasureChests();
    }

    public void LoadMainMenuScene()
    {
        if (PhotonNetwork.InRoom)
        {
            GameObject roomManager = GameObject.FindGameObjectWithTag("RoomManager");
            roomManager.GetComponent<RoomManager>().LoadMainMenuScene();
        }
        else
        {
            SceneManager.LoadScene(0);
        }
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
            pos = Random.insideUnitCircle * gameSettings.radiusSpawnChest;
            //swap y->z , y=0f:under ground
            pos.y += pos.z;
            pos.z = pos.y - pos.z;
            pos.y = 0f;

            pos += spawnPointCoordinates.transform.position;
            GameObject treasureChest;
            if (PhotonNetwork.InRoom)
            {
                treasureChest = PhotonNetwork.Instantiate("Treasure", Vector3.up, Quaternion.identity);
            }
            else
            {
                treasureChest = Instantiate(Resources.Load("Treasure"), Vector3.up, Quaternion.identity) as GameObject;
            }
            //GameObject treasureChest = Instantiate(treasureChestObj);
            treasureChest.transform.position = pos;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 1);
        if (spawnPointCoordinates != null)
            Gizmos.DrawSphere(spawnPointCoordinates.transform.position, gameSettings.radiusSpawnChest);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //Debug.Log("Player: " + targetPlayer + " changed " + changedProps);
        if (changedProps["curTimeleft"] != null)
        {
            float curTimeleft = (float)changedProps["curTimeleft"];
            DisplayTimeleft(curTimeleft);
        }

        if (changedProps["masterClientResetGame"] != null)
        {
            m_masterClientResetGame = (bool)changedProps["masterClientResetGame"];
        }
    }
}
