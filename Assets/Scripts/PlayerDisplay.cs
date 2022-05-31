using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    GameObject character;
    public GameObject[] models;
    protected Animator m_animator;

    Vector3 warpPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        warpPosition = Vector3.zero;
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
        int randomValue = Random.Range(0, models.Length);
        character = Instantiate(models[0]);
        character.transform.SetParent(this.transform);
        character.transform.position = Vector3.zero;
        if (character == null) Debug.LogError("Missing gameobject character in Player");
        m_animator = character.GetComponent<Animator>();
    }
}

