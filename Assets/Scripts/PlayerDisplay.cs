using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    GameObject character;
    protected Animator m_animator;
    public GameObject[] characterModelsPrefab;

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
        int randomValue = Random.Range(1, characterModelsPrefab.Length);
        character = Instantiate(characterModelsPrefab[randomValue], this.transform);
        if (character == null) Debug.LogError("Missing gameobject character in Player");
        m_animator = character.GetComponent<Animator>();
    }
}

