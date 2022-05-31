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

    public enum GameState { Ingame, EndGame };
    GameState m_gameState = GameState.Ingame;

    public GameState CurGameState
    {
        get { return m_gameState; }
        set { m_gameState = value; }
    }

    float m_timeleft = 0f;

    [Header("TreasureChest")]
    public GameObject treasureChestObj;
    GameObject spawnPointCoordinates;

    //[Header("Photon PUN")]
    //public PhotonView photonView;

    bool m_masterClientResetGame = false;

    // Start is called before the first frame update
    void Start()
    {
        spawnPointCoordinates = GameObject.FindGameObjectWithTag("SpawnPointCoordinates");
        if (spawnPointCoordinates == null) Debug.LogError("Could not found SpawnPointCoordinates");
        if (PhotonNetwork.InRoom)
            player.name = "Player" + photonView.ViewID;
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
            //Debug.Log("ViewID : " + photonView.ViewID + " - IsMasterClient: " + PhotonNetwork.IsMasterClient + " - IsMine: " + photonView.IsMine);
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
        Time.timeScale = 0.5f;
        Debug.Log("EndGame ViewID : " + photonView.ViewID + " - resultGame: " + resultGame);
        if (!PhotonNetwork.InRoom || photonView.IsMine)
        {
            ingameUI.GetComponent<IngameUI>().ShowEndGameUI(resultGame);
            StartCoroutine(DelayAction(2f));
        }
        m_gameState = GameState.EndGame;
    }

    IEnumerator DelayAction(float delayTime)
    {
        //delete old chests
        GameObject[] treasureChests = GameObject.FindGameObjectsWithTag("TreasureChest");
        yield return new WaitForSeconds(delayTime);
        DestroyOldTreasure(treasureChests);
    }

    public void StartNewGame()
    {
        Debug.Log("StartNewGame - ViewID: " + photonView.ViewID);
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
        player.GetComponent<PlayerDisplay>().RandomPlayerPosition();
        player.GetComponent<PlayerController>().NumberTreasureDigged = 0;
        //Debug.Log("ViewID : " + photonView.ViewID + " - IsMasterClient: " + PhotonNetwork.IsMasterClient + " - IsMine: " + photonView.IsMine);
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
            SpawnTreasureChests();
        m_gameState = GameState.Ingame;
    }

    public void LoadMainMenuScene()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    void DestroyOldTreasure(GameObject[] treasureChests)
    {
        if (treasureChests.Length != 0)
        {
            foreach (GameObject chest in treasureChests)
            {
                if (chest != null)
                    Destroy(chest);
            }
        }
    }

    void SpawnTreasureChests()
    {
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
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log(" PhotonNetwork.Instantiate Treasure");
                    treasureChest = PhotonNetwork.Instantiate("Treasure", Vector3.up, Quaternion.identity);
                    treasureChest.transform.position = pos;
                }
            }
            else
            {
                Debug.Log("Instantiate Treasure");
                treasureChest = Instantiate(Resources.Load("Treasure"), Vector3.up, Quaternion.identity) as GameObject;
                treasureChest.transform.position = pos;
            }
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
