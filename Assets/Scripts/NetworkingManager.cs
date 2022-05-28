using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkingManager : MonoBehaviourPunCallbacks
{
    public GameObject connecting;
    public GameObject multiplayerBtn;

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            connecting.SetActive(true);
            multiplayerBtn.SetActive(false);
            Debug.Log("Connecting to Server ... ");
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Connected to Server ... ");
            connecting.SetActive(false);
            multiplayerBtn.SetActive(true);
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
        connecting.SetActive(false);
        multiplayerBtn.SetActive(true);
    }
    public void FindMatch()
    {
        Debug.Log("Finding Room ... ");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        MakeNewRoom();
    }

    void MakeNewRoom()
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
}
