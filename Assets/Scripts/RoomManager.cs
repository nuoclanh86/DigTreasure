using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Cinemachine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject virtualCamera;

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
            PhotonNetwork.Instantiate("Player", Vector3.up * 6, Quaternion.identity);
        }
        else
        {
            Instantiate(Resources.Load("Player"), Vector3.up * 6, Quaternion.identity);
        }

        GameObject playerOnline = GameObject.FindGameObjectWithTag("Player");
        virtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = playerOnline.transform;
    }
}
