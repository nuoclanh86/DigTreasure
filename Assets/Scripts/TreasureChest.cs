using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public GlobalGameSettings gameSettings;
    public PhotonView photonView;
    GameObject player;
    Vector3 m_positionPlayerDiggingXZ;

    bool m_isDigged = false;

    // Start is called before the first frame update
    void Start()
    {
        m_positionPlayerDiggingXZ = Vector3.zero;
        m_isDigged = false;
        player = GameObject.FindGameObjectWithTag("Player");
        TreasureChestOpened(false);
    }

    // Update is called once per frame
    void Update()
    {
        m_positionPlayerDiggingXZ = player.transform.position;
        m_positionPlayerDiggingXZ.y = this.transform.position.y;
        if (m_isDigged == false
            && Vector3.Distance(m_positionPlayerDiggingXZ, this.transform.position) <= gameSettings.signal_lvl_3_distance
            && player.GetComponent<PlayerController>().CurPlayerState == PlayerController.PlayerState.Digging
            && this.transform.position.y <= player.transform.position.y)
        {
            this.transform.position += Vector3.up * Time.deltaTime * gameSettings.digSpeed;
        }
        else if (this.transform.position.y >= player.transform.position.y)
        {
            TreasureChestOpened(true);
        }
    }

    public bool IsDigged
    {
        get { return m_isDigged; }
        set { m_isDigged = value; }
    }

    void TreasureChestOpened(bool val)
    {
        if (PhotonNetwork.InRoom)
            photonView.RPC("OpenTheTreasureChest", RpcTarget.All, val, photonView.ViewID);
        else
            OpenTheTreasureChest(val, -1);
    }

    [PunRPC]
    public void OpenTheTreasureChest(bool val, int viewID)
    {
        if (!PhotonNetwork.InRoom || viewID == photonView.ViewID)
        {
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
}