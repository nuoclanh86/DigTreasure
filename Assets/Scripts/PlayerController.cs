using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float playerSpeed = 4.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;

    protected CharacterController controller;
    protected PlayerActionsManager playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    GameObject character;
    protected Animator animator;
    private int animationPlayerState;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = new PlayerActionsManager();

        foreach (Transform child in this.transform)
        {
            if (child.gameObject.name == "character")
            {
                character = child.gameObject;
                //Debug.Log("curPlayer : " + curPlayer.name);
                break;
            }
        }
        if (character.name != "character") Debug.LogError("Missing gameobject character in Player");
        animator = character.GetComponent<Animator>();
        animationPlayerState = 0;
    }

    private void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = playerInput.Player.Move.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
            animationPlayerState = 1;
        }
        else
            animationPlayerState = 0;

        // bool jumpPress = playerInput.Player.Jump.IsPressed();
        bool jumpPress = playerInput.Player.Jump.triggered;
        if (jumpPress && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        animator.SetInteger("PlayerState", animationPlayerState);
        Debug.Log("PlayerState:" + animator.GetInteger("PlayerState"));
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

}
