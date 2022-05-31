using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject connecting;
    public GameObject multiplayerBtn;

    public PanelAnimator panel01, panel02;

    private void Start()
    {
        if (!PhotonNetwork.InLobby)
        {
            ShowMultiplayerBtn(false);
            ConnectServer();
        }
        else
        {
            Debug.Log("Connected to Server");
            ShowMultiplayerBtn(true);
        }
    }

    public void ShowMultiplayerBtn(bool val)
    {
        connecting.SetActive(val == false);
        multiplayerBtn.SetActive(val == true);
    }

    public void StartSinglePlayerGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ConnectServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to Server ... ");
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Already Connected to Server");
        }
        ShowMultiplayerBtn(false);
    }

    public void ShowJoinRoomPanel(bool val)
    {
        Debug.Log("ShowJoinRoomPanel: " + val);
        if (val == true)
        {
            panel01.StartAnimOut();
            panel02.StartAnimIn();
        }
        else
        {
            panel01.StartAnimIn();
            panel02.StartAnimOut();
        }
    }

    public void JoinRandomRoom()
    {
        Debug.Log("Finding Room ... ");
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateRoom()
    {
        NetworkingManager.Instance.MakeNewRoom();
    }

    public void JoinSelectedRoom(string roomName)
    {
        Debug.Log("Join Room : " + roomName);
        PhotonNetwork.JoinRoom(roomName);
    }
    public void ShowRoomList()
    {
        Debug.Log("ShowRoomList");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
