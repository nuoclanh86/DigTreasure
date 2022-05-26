using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    private bool m_isDigged = false;

    // Start is called before the first frame update
    void Start()
    {
        m_isDigged = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsDigged
    {
        get { return m_isDigged; }
        set { m_isDigged = value; }
    }
}
