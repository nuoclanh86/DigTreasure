using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public GameManager gameManager;
    //[SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;

    protected CharacterController m_controller;
    protected PlayerActionsManager m_playerInput;
    private Vector3 m_playerVelocity;
    private bool m_groundedPlayer;

    GameObject character;
    protected Animator m_animator;
    public enum PlayerState { Idle = 0, Running, Walking, Digging };
    private PlayerState m_curPlayerState;
    private float m_moveSpeed = 1f;

    float m_diggingTimePressCountDown = 0f;
    Vector3 warpPosition = Vector3.zero;

    public PhotonView photonView;
    int m_NumberTreasureDigged = 0;

    private void Awake()
    {
        m_controller = GetComponent<CharacterController>();
        m_playerInput = new PlayerActionsManager();

        foreach (Transform child in this.transform)
        {
            if (child.gameObject.name == "character")
            {
                character = child.gameObject;
                break;
            }
        }
        if (character.name != "character") Debug.LogError("Missing gameobject character in Player");
        m_animator = character.GetComponent<Animator>();
        UpdatePlayerState(PlayerState.Idle);
    }

    private void Start()
    {
        warpPosition = Vector3.zero;
        RandomPlayerPosition();
        m_NumberTreasureDigged = 0;
    }

    public float MoveSpeed
    {
        get { return m_moveSpeed; }
        set { m_moveSpeed = value; }
    }
    public PlayerState CurPlayerState
    {
        get { return m_curPlayerState; }
        set { UpdatePlayerState(value); }
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }
        m_groundedPlayer = m_controller.isGrounded;
        if (m_groundedPlayer && m_playerVelocity.y < 0)
        {
            m_playerVelocity.y = 0f;
        }

        Vector2 movement = m_playerInput.Player.Move.ReadValue<Vector2>();
        if (movement != Vector2.zero)
        {
            PlayerMove(movement);
            m_diggingTimePressCountDown = -1f;
        }
        else if (m_curPlayerState == PlayerState.Idle || m_curPlayerState == PlayerState.Digging)
        {
            PlayerDigging();
        }
        else
            UpdatePlayerState(PlayerState.Idle);

        //PlayerJump();

        m_playerVelocity.y += gravityValue * Time.deltaTime;
        m_controller.Move(m_playerVelocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        m_animator.SetInteger("PlayerState", (int)m_curPlayerState);
        //Debug.Log("PlayerState:" + animator.GetInteger("PlayerState"));

        if (warpPosition != Vector3.zero)
        {
            transform.position = warpPosition;
            warpPosition = Vector3.zero;
        }
    }

    void PlayerMove(Vector2 movementInput)
    {
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
        m_controller.Move(move * Time.deltaTime * m_moveSpeed * gameManager.gameSettings.cheatSpeed);
        gameObject.transform.forward = move;
        if (m_moveSpeed > gameManager.gameSettings.walkSpeed)
            UpdatePlayerState(PlayerState.Running);
        else
            UpdatePlayerState(PlayerState.Walking);
    }

    //void PlayerJump()
    //{
    // bool jumpPress = playerInput.Player.Jump.IsPressed();
    //bool jumpPress = playerInput.Player.Jump.triggered;
    //if (jumpPress && groundedPlayer)
    //{
    //    playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    //}
    //}

    void PlayerDigging()
    {
        bool digPress = m_playerInput.Player.ButtonD.triggered;
        if (digPress)
            m_diggingTimePressCountDown = 2f;

        if (m_diggingTimePressCountDown > 0f)
        {
            m_diggingTimePressCountDown -= Time.deltaTime;
            UpdatePlayerState(PlayerState.Digging);
        }
        else
            UpdatePlayerState(PlayerState.Idle);
    }

    void UpdatePlayerState(PlayerState ps)
    {
        if (!PhotonNetwork.InRoom)
            UpdatePlayerStatePunRPC(ps);
        else
            photonView.RPC("UpdatePlayerStatePunRPC", RpcTarget.All, ps);
    }

    [PunRPC]
    void UpdatePlayerStatePunRPC(PlayerState ps)
    {
        m_curPlayerState = ps;
    }

    public int NumberTreasureDigged
    {
        get { return m_NumberTreasureDigged; }
        set { m_NumberTreasureDigged = value; }
    }

    private void OnEnable()
    {
        m_playerInput.Enable();
    }

    private void OnDisable()
    {
        m_playerInput.Disable();
    }

    public void RandomPlayerPosition()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        warpPosition = spawnPoint.transform.position;
    }
}
