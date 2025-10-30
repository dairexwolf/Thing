using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private CharacterController controller;

    [Header("Movement")]
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.5f;
    public LayerMask groundMask;

    [Header("Clamps")]
    public float topClamp = 10f;
    public float bottomClamp = -50f;

    [Header("Test Values, do not change")]
    public Vector3 velocity;



    bool isGrounded;
    bool isMoving;

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);

    // Input actions
    InputAction moveAction;
    InputAction jumpAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Reseting the default velocity
        if(isGrounded && velocity.y<0)
        {
            velocity.y = -2f;
        }

        // Get the input
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        // Creating the moving vector
        Vector3 move = transform.right * moveValue.x + transform.forward * moveValue.y;

        // Moving player
        controller.Move(move * speed * Time.deltaTime);

        if(isGrounded && jumpAction.IsPressed())
        {
            // Actually jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Falling down
        velocity.y += gravity * Time.deltaTime;
        // Clamp the upping and falling
        velocity.y = Mathf.Clamp(velocity.y, bottomClamp, topClamp);

        // Exectuting the jump
        controller.Move(velocity * Time.deltaTime);

        if(lastPosition != transform.position && isGrounded == true)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        lastPosition = transform.position;
    }
}
