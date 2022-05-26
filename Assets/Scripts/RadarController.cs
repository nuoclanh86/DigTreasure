using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarController : MonoBehaviour
{
    public GameObject treasureManager;
    public GameObject player;

    float m_delayEachScan = -1f;

    // Start is called before the first frame update
    void Start()
    {
        m_delayEachScan = -1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_delayEachScan <= 0f)
        {
            m_delayEachScan = GameManager.Instance.gameSettings.delayEachScan;
            int signal = RadarScan();
            UpdateRadarSignal(signal);
        }
        else
            m_delayEachScan -= Time.deltaTime;
    }

    int RadarScan()
    {
        int signal = 0;
        Vector3[] treasureChestPosisions = treasureManager.GetComponent<TreasureManager>().GetTreasureChestPosisions();
        foreach (Vector3 pos in treasureChestPosisions)
        {
            Vector3 posXZ = new Vector3(pos.x, player.transform.position.y, pos.z);
            float distance = Vector3.Distance(posXZ, player.transform.position);
            Debug.Log("distance to treasure : " + distance);

            if (distance < GameManager.Instance.gameSettings.signal_lvl_3_distance)
                return 3; // max signal
            else if (distance < GameManager.Instance.gameSettings.signal_lvl_2_distance)
                signal = 2;
            else if (distance < GameManager.Instance.gameSettings.signal_lvl_1_distance)
                signal = Mathf.Max(1, signal);
        }
        return signal;
    }

    void UpdateRadarSignal(int levelSignal)
    {
        foreach (Transform child in this.transform)
        {
            if (child.gameObject.name.Contains("Level") == true)
            {
                bool shouldActive = child.gameObject.name == "Level_" + levelSignal;
                child.gameObject.SetActive(shouldActive);
            }
        }
    }
}
