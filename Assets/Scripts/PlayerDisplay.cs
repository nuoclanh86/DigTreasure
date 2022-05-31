using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    GameObject character;
    protected Animator m_animator;

    Vector3 warpPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        RandomPlayerModels();
        RandomPlayerPosition();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        m_animator.SetInteger("PlayerState", (int)this.GetComponent<PlayerController>().CurPlayerState);
        //Debug.Log("PlayerState:" + animator.GetInteger("PlayerState"));

        if (warpPosition != Vector3.zero)
        {
            transform.position = warpPosition;
            warpPosition = Vector3.zero;
        }
    }
    public void RandomPlayerPosition()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        warpPosition = spawnPoint.transform.position;
    }

    void RandomPlayerModels()
    {
        //try noob way when searching the best : hardcode ... lol
        int countModel = 0;
        foreach (Transform child in this.transform)
        {
            if (child.gameObject.name.Contains("character"))
            {
                countModel++;
            }
        }
        int randomValue = Random.Range(0, countModel);
        Debug.Log("randomValue: " + randomValue + "/" + countModel);
        int i = 0;
        foreach (Transform child in this.transform)
        {
            if (child.gameObject.name.Contains("character"))
            {
                if (i == randomValue)
                {
                    child.gameObject.SetActive(true);
                    character = child.gameObject;
                }
                else
                    child.gameObject.SetActive(false);
                i++;
            }
        }
        if (character == null) Debug.LogError("Missing gameobject character in Player");
        m_animator = character.GetComponent<Animator>();
    }
}

