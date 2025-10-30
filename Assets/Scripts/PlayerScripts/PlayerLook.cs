using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    InputAction move;

    [Header("Look")]
    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    float yRotation = 0f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;
    public Transform playerBody; 
    public Transform camera; 




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Locking corsour to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;

        if (playerBody == null) 
            playerBody = transform;
        
        if (camera == null) 
            camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 delta = Mouse.current.delta.ReadValue();

        // Rotation around the x axis (Up and down)
        xRotation -= delta.y * mouseSensitivity * Time.deltaTime;

        // Clamp the rotation (ограничить кароч)
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        // Rotation around the y axis (Left and right)
        yRotation += delta.x * mouseSensitivity * Time.deltaTime;

        //Apply rotation
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
