using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions inputSystem;
    private CharacterController controller;
    private Animator animator;
    [SerializeField] private float moveSpeed;
    [SerializeField] Vector2 moveInput;
    [SerializeField] private float rotationSpeed;

    private void Awake()
    {
        inputSystem = new InputSystem_Actions();
        inputSystem.Enable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
    }


    void Update()
    {
        moveInput = inputSystem.Player.Move.ReadValue<Vector2>();

        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);

        if (direction != Vector3.zero)
        {
            animator.SetBool("Running", true);
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            
            

            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            controller.Move(direction.normalized * moveSpeed * Time.deltaTime);
            
        }
        else
        {
            animator.SetBool("Running", false);
        }
    }
    
    
}
