using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarController : MonoBehaviour
{
    public GameObject player;
    public GameObject digButton;
    public PhotonView photonView;
    public GameManager gameManager;

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
            m_delayEachScan = gameManager.gameSettings.delayEachScan;
            SignalStrength signal = RadarScan();
            UpdateRadarSignal(signal);
            //Debug.Log("signal: " + signal);
            if (signal < SignalStrength.Level2_Medium) // weak signal - run
                player.GetComponent<PlayerController>().MoveSpeed = gameManager.gameSettings.runSpeed;
            else // strong signal - walk
                player.GetComponent<PlayerController>().MoveSpeed = gameManager.gameSettings.walkSpeed;
        }
        else
            m_delayEachScan -= Time.deltaTime;
    }

    bool AreAllTreasureChestsDigged(GameObject[] treasureChests)
    {
        bool result = true;
            foreach (GameObject chest in treasureChests)
            {
                if (chest.GetComponent<TreasureChest>().IsDigged == false)
                    return false;
            }
        return result;
    }

    SignalStrength RadarScan()
    {
        SignalStrength signal = SignalStrength.None;
        GameObject[] treasureChests = GameObject.FindGameObjectsWithTag("TreasureChest");
        if (treasureChests.Length != 0 && AreAllTreasureChestsDigged(treasureChests) == true)
        {
            if (gameManager.CurGameState == GameManager.GameState.Ingame)
            {
                if (photonView.IsMine && player.name == RoomManager.Instance.HighestPlayerDigged())
                    gameManager.EndGame("You Won");
                else
                    gameManager.EndGame("You Lose");
            }
            return signal;
        }

        foreach (GameObject chest in treasureChests)
        {
            if (chest.GetComponent<TreasureChest>().IsDigged == false)
            {
                Vector3 posXZ = new Vector3(chest.transform.position.x, player.transform.position.y, chest.transform.position.z);
                float distance = Vector3.Distance(posXZ, player.transform.position);
                //Debug.Log("distance to treasure : " + distance);

                if (distance < gameManager.gameSettings.signal_lvl_3_distance)
                    return SignalStrength.Level3_Strong;
                else if (distance < gameManager.gameSettings.signal_lvl_2_distance)
                    signal = SignalStrength.Level2_Medium;
                else if (distance < gameManager.gameSettings.signal_lvl_1_distance)
                    signal = (SignalStrength)Mathf.Max((int)SignalStrength.Level1_Weak, (int)signal);
            }
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
