using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    private bool m_isDigging = false;

    // Start is called before the first frame update
    void Start()
    {
        m_isDigging = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isDigging && this.transform.position.y >= 1f)
        {
            this.transform.position += Vector3.up * Time.deltaTime * GameManager.Instance.gameSettings.digSpeed;
        }
    }

    public bool IsDigging
    {
        get { return m_isDigging; }
        set { m_isDigging = value; }
    }
}
