using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    public GameObject treasureChestObj;
    public int maxTreasureChest = 2;
    public float radiusSpawnChest = 22f;

    private GameObject[] treasureChests;

    // Start is called before the first frame update
    void Start()
    {
        treasureChests = new GameObject[maxTreasureChest];
        SpawnTreasureChests();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 1);
        Gizmos.DrawSphere(transform.position, radiusSpawnChest);
    }

    protected void SpawnTreasureChests()
    {
        //delete old chests
        if (treasureChests[0] != null)
        {
            foreach (GameObject chest in treasureChests)
            {
                if (chest != null)
                    Destroy(chest);
            }
        }

        //init new chests
        for (int i = 0; i < maxTreasureChest; i++)
        {
            Vector3 pos = new Vector3(0, 0, 0);
            pos = Random.insideUnitCircle * radiusSpawnChest;
            //swap y->z , y=0f:under ground
            pos.y += pos.z;
            pos.z = pos.y - pos.z;
            pos.y = 0f;
            //pos.y += 6f;//cheat

            pos += transform.position;
            treasureChests[i] = Instantiate(treasureChestObj, this.transform, true);
            treasureChests[i].transform.position = pos;
        }
    }
}
