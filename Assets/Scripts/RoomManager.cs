using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject virtualCamera;
    public TextMeshProUGUI debugText;
    public GameObject testBtn;
    public GlobalGameSettings gameSettings;

    public static RoomManager Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            ShowDebugLog();
        }
    }

    void ShowDebugLog()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] chests = GameObject.FindGameObjectsWithTag("TreasureChest");
        string debugLog = "";
        foreach (GameObject player in players)
            debugLog += player.name + " - " + player.GetComponent<PlayerController>().NumberTreasureDigged + "\n";
        int countChestsDigged = 0;
        foreach (GameObject chest in chests)
        {
            if (chest.GetComponent<TreasureChest>().IsDigged == true)
                countChestsDigged++;
        }
        debugLog += "\nChests: " + countChestsDigged + "/" + chests.Length;
        debugText.text = debugLog;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded");
        if (PhotonNetwork.InRoom)
        {
            GameObject playerOnline = PhotonNetwork.Instantiate("Player", Vector3.up * 6, Quaternion.identity);
            virtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = playerOnline.transform;
        }
    }

    public void LoadMainMenuScene()
    {
        Debug.Log("LoadMainMenuScene");
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }

    public void CheatBtn()
    {
        if (gameSettings.cheatSpeed == 1)
            gameSettings.cheatSpeed = 2;
        else
            gameSettings.cheatSpeed = 1;
    }

    public string HighestPlayerDigged()
    {
        string result = "playerNull";
        int highestPoint = 0;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerController>().NumberTreasureDigged >= highestPoint)
            {
                highestPoint = player.GetComponent<PlayerController>().NumberTreasureDigged;
                result = player.name;
            }
        }
        return result;
    }
}
