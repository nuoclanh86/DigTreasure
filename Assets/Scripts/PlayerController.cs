using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //[SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;

    protected CharacterController m_controller;
    protected PlayerActionsManager m_playerInput;
    private Vector3 m_playerVelocity;
    private bool m_groundedPlayer;

    GameObject character;
    protected Animator m_animator;
    enum PlayerState { Idle=0, Running, Walking, Digging };
    private PlayerState m_curPlayerState;
    private float m_moveSpeed = 1f;

    float m_diggingTimePressCountDown = 0f;

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
        m_curPlayerState = PlayerState.Idle;
    }

    public float MoveSpeed
    {
        get { return m_moveSpeed; }
        set { m_moveSpeed = value; }
    }

    private void Update()
    {
        m_groundedPlayer = m_controller.isGrounded;
        if (m_groundedPlayer && m_playerVelocity.y < 0)
        {
            m_playerVelocity.y = 0f;
        }

        Vector2 movement = m_playerInput.Player.Move.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        m_controller.Move(move * Time.deltaTime * m_moveSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
            if (m_moveSpeed > GameManager.Instance.gameSettings.walkSpeed)
                m_curPlayerState = PlayerState.Running;
            else
                m_curPlayerState = PlayerState.Walking;

            m_diggingTimePressCountDown = -1f;
        }
        else if (m_curPlayerState == PlayerState.Idle || m_curPlayerState == PlayerState.Digging)
        {
            bool digPress = m_playerInput.Player.ButtonD.triggered;
            if (digPress)
                m_diggingTimePressCountDown = 2f;

            if (m_diggingTimePressCountDown > 0f)
            {
                m_diggingTimePressCountDown -= Time.deltaTime;
                m_curPlayerState = PlayerState.Digging;
            }
            else
                m_curPlayerState = PlayerState.Idle;
        }
        else
            m_curPlayerState = PlayerState.Idle;

        // bool jumpPress = playerInput.Player.Jump.IsPressed();
        //bool jumpPress = playerInput.Player.Jump.triggered;
        //if (jumpPress && groundedPlayer)
        //{
        //    playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        //}

        m_playerVelocity.y += gravityValue * Time.deltaTime;
        m_controller.Move(m_playerVelocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        m_animator.SetInteger("PlayerState", (int)m_curPlayerState);
        //Debug.Log("PlayerState:" + animator.GetInteger("PlayerState"));
    }

    private void OnEnable()
    {
        m_playerInput.Enable();
    }

    private void OnDisable()
    {
        m_playerInput.Disable();
    }
}
