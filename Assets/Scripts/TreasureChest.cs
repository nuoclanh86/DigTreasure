using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public GlobalGameSettings gameSettings;
    public PhotonView photonView;
    public GameObject CheatShowPosition;
    GameObject player;
    Vector3 m_positionPlayerDiggingXZ;

    bool m_isDigged = false;

    // Start is called before the first frame update
    void Start()
    {
        m_positionPlayerDiggingXZ = Vector3.zero;
        m_isDigged = false;
        player = GetMyPlayer();
        TreasureChestOpened(false);
    }

    GameObject GetMyPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            //Debug.LogFormat("player.name: {0} - viewID={1} - isMine={2}", p.name, p.GetComponent<PlayerController>().photonView.ViewID, p.GetComponent<PlayerController>().photonView.IsMine);
            if (!PhotonNetwork.InRoom || p.GetComponent<PlayerController>().photonView.IsMine)
            {
                return p;
            }
        }
        Debug.LogError("player == null");
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        m_positionPlayerDiggingXZ = player.transform.position;
        m_positionPlayerDiggingXZ.y = this.transform.position.y;

        //Debug.LogFormat("player.name: {3}, m_isDigged: {0}, Distance: {1}, CurPlayerState: {2}", m_isDigged, Vector3.Distance(m_positionPlayerDiggingXZ, this.transform.position), 
        //player.GetComponent<PlayerController>().CurPlayerState, player.name);
        if (m_isDigged == false
            && Vector3.Distance(m_positionPlayerDiggingXZ, this.transform.position) <= gameSettings.signal_lvl_3_distance
            && player.GetComponent<PlayerController>().CurPlayerState == PlayerController.PlayerState.Digging
            && this.transform.position.y <= player.transform.position.y)
        {
            this.transform.position += Vector3.up * Time.deltaTime * gameSettings.digSpeed;
            CheatShowPosition.SetActive(false);
        }
        else if (m_isDigged == false && this.transform.position.y >= player.transform.position.y)
        {
            TreasureChestOpened(true);
        }

        ShowCheatPosition();
    }

    public bool IsDigged
    {
        get { return m_isDigged; }
        set { m_isDigged = value; }
    }

    void ShowCheatPosition()
    {
        if (m_isDigged == false && PhotonNetwork.InRoom)
        {
            CheatShowPosition.SetActive(RoomManager.Instance.IsCheated);
        }
        else if (m_isDigged == true)
        {
            CheatShowPosition.SetActive(false);
        }
    }

    void TreasureChestOpened(bool val)
    {
        if (PhotonNetwork.InRoom)
            photonView.RPC("OpenTheTreasureChest", RpcTarget.All, val);
        else
            OpenTheTreasureChest(val);
    }

    [PunRPC]
    public void OpenTheTreasureChest(bool val)
    {
        //Debug.Log("TreasureChestOpened " + val + " by : " + player.name + " - photonView.IsMine: " + photonView.IsMine);
        if (val == true && (!PhotonNetwork.InRoom || photonView.IsMine))
        {
            player.GetComponent<PlayerController>().NumberTreasureDigged++;
        }
        foreach (Transform child in this.transform)
        {
            m_isDigged = val;
            if (child.gameObject.name.Contains("open") == true)
                child.gameObject.SetActive(val == true);
            else if (child.gameObject.name.Contains("close") == true)
                child.gameObject.SetActive(val == false);
        }

    }
}
