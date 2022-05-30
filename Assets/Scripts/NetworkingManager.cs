using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkingManager : MonoBehaviourPunCallbacks
{
    public static NetworkingManager Instance;
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
        DontDestroyOnLoad(Instance);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to Server ... ");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Joining Lobby ... ");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Ready for Multiplayer");
        GameObject mainmenu = GameObject.FindGameObjectWithTag("MainMenuManager");
        if (mainmenu != null)
        {
            mainmenu.GetComponent<MainMenuManager>().ShowMultiplayerBtn(true);
        }
    }
 
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        MakeNewRoom();
    }

    public void MakeNewRoom()
    {
        int randomRoomName = Random.Range(0, 9999);
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 6,
            PublishUserId = true
        };
        PhotonNetwork.CreateRoom("RoomName_" + randomRoomName, roomOptions);
        Debug.Log("Room Made: " + randomRoomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room. Loading scene ... ");
        PhotonNetwork.LoadLevel(2);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(" ======================== OnDisconnected ======================== ");
        base.OnDisconnected(cause);
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
}
