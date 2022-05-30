using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject connecting;
    public GameObject multiplayerBtn;

    private void Start()
    {
        if (!PhotonNetwork.InLobby)
        {
            connecting.SetActive(true);
            multiplayerBtn.SetActive(false);
        }
        else
        {
            Debug.Log("Connected to Server");
            connecting.SetActive(false);
            multiplayerBtn.SetActive(true);
        }
    }

    public void StartSinglePlayerGame()
    {
        SceneManager.LoadScene(1);
    }

    public void FindMatch()
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

    public void ShowMultiplayerBtn(bool val)
    {
        connecting.SetActive(val==false);
        multiplayerBtn.SetActive(val==true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
