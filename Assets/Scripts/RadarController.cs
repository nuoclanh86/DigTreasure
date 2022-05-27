using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarController : MonoBehaviour
{
    public GameObject treasureManager;
    public GameObject player;
    public GameObject digButton;

    enum SignalStrength { None = 0, Level1_Weak, Level2_Medium, Level3_Strong };
    float m_delayEachScan = -1f;

    // Start is called before the first frame update
    void Start()
    {
        m_delayEachScan = -1f;
        digButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_delayEachScan <= 0f)
        {
            m_delayEachScan = GameManager.Instance.gameSettings.delayEachScan;
            SignalStrength signal = RadarScan();
            UpdateRadarSignal(signal);
            Debug.Log("signal: " + signal);
            if (signal < SignalStrength.Level2_Medium) // weak signal - run
                player.GetComponent<PlayerController>().MoveSpeed = GameManager.Instance.gameSettings.runSpeed;
            else // strong signal - walk
                player.GetComponent<PlayerController>().MoveSpeed = GameManager.Instance.gameSettings.walkSpeed;
        }
        else
            m_delayEachScan -= Time.deltaTime;
    }

    SignalStrength RadarScan()
    {
        SignalStrength signal = SignalStrength.None;
        Vector3[] treasureChestPosisions = treasureManager.GetComponent<TreasureManager>().GetTreasureChestPosisions();
        foreach (Vector3 pos in treasureChestPosisions)
        {
            Vector3 posXZ = new Vector3(pos.x, player.transform.position.y, pos.z);
            float distance = Vector3.Distance(posXZ, player.transform.position);
            //Debug.Log("distance to treasure : " + distance);

            if (distance < GameManager.Instance.gameSettings.signal_lvl_3_distance)
                return SignalStrength.Level3_Strong;
            else if (distance < GameManager.Instance.gameSettings.signal_lvl_2_distance)
                signal = SignalStrength.Level2_Medium;
            else if (distance < GameManager.Instance.gameSettings.signal_lvl_1_distance)
                signal = (SignalStrength)Mathf.Max((int)SignalStrength.Level1_Weak, (int)signal);
        }
        return signal;
    }

    void UpdateRadarSignal(SignalStrength levelSignal)
    {
        foreach (Transform child in this.transform)
        {
            if (child.gameObject.name.Contains("Level") == true)
            {
                bool shouldActive = child.gameObject.name == "Level_" + (int)levelSignal;
                child.gameObject.SetActive(shouldActive);
            }
        }
        digButton.SetActive(levelSignal == SignalStrength.Level3_Strong);
    }
}
