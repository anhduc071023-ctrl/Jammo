
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Header("Pitch Boundaries")]
    [SerializeField] private Vector2 pitchMin = new Vector2(-10f, -15f);
    [SerializeField] private Vector2 pitchMax = new Vector2(10f, 15f);
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float gravity = -20f;

    [Header("References")]
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private Vector3 moveDirection;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        UpdateAnimation();
    }


    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(x, 0f, z);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity =
            moveDirection * moveSpeed +
            Vector3.up * verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

      
        Vector3 position = transform.position;

        position.x = Mathf.Clamp(
            position.x, pitchMin.x, pitchMax.x
        );

        position.z = Mathf.Clamp(
            position.z, pitchMin.y, pitchMax.y
        );

        transform.position = position;
    }


    private void HandleRotation()
    {
    
        if (moveDirection.sqrMagnitude < 0.001f)
            return;

       
        Quaternion targetRotation = Quaternion.LookRotation(
            moveDirection,
            Vector3.up
        );

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

     
        animator.SetFloat("Speed", moveDirection.magnitude);
    }
}
