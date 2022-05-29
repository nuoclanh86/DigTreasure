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

    public static RoomManager Instance;
    void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject[] chests = GameObject.FindGameObjectsWithTag("TreasureChest");
            string debugLog = "";
            foreach (GameObject player in players)
                debugLog += player.name + "\n";
            int countChestsDigged = 0;
            foreach (GameObject chest in chests)
            {
                if (chest.GetComponent<TreasureChest>().IsDigged == true)
                    countChestsDigged++;
            }
            debugLog += "\nChests: " + countChestsDigged + "/" + chests.Length;
            debugText.text = debugLog;
        }
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
        if(PhotonNetwork.InRoom)
        PhotonNetwork.LeaveRoom();
        //StartCoroutine(LoadMainMenu());
    }

    public override void OnLeftLobby()
    {
        Debug.Log(" ======================== OnLeftLobby ======================== ");
        base.OnLeftLobby();
    }

    public override void OnLeftRoom()
    {
        Debug.Log(" ======================== OnLeftRoom ======================== ");
        base.OnLeftRoom();
        SceneManager.LoadScene(0);
    }

    public void CheatBtn()
    {
        if (GameManager.Instance.gameSettings.cheatSpeed == 1)
            GameManager.Instance.gameSettings.cheatSpeed = 2;
        else
            GameManager.Instance.gameSettings.cheatSpeed = 1;
    }
}
