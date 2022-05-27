using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    GameObject player;
    Vector3 m_positionPlayerDiggingXZ;

    bool m_isDigged = false;

    // Start is called before the first frame update
    void Start()
    {
        m_positionPlayerDiggingXZ = Vector3.zero;
        m_isDigged = false;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        m_positionPlayerDiggingXZ = player.transform.position;
        m_positionPlayerDiggingXZ.y = this.transform.position.y;
        if (m_isDigged == false
            && Vector3.Distance(m_positionPlayerDiggingXZ, this.transform.position) <= GameManager.Instance.gameSettings.signal_lvl_3_distance
            && player.GetComponent<PlayerController>().CurPlayerState == PlayerController.PlayerState.Digging
            && this.transform.position.y <= player.transform.position.y)
        {
            this.transform.position += Vector3.up * Time.deltaTime * GameManager.Instance.gameSettings.digSpeed;
        }
        else if (this.transform.position.y >= player.transform.position.y)
            m_isDigged = true;
    }

    public bool IsDigged
    {
        get { return m_isDigged; }
        set { m_isDigged = value; }
    }
}