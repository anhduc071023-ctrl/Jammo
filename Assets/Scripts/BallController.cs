
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    public static event Action<BallController> OnBallKicked;
    public static event Action<BallController> OnBallScored;

    [Header("Normal Kick")]
    [SerializeField] private float normalKickForce = 12f;
    [SerializeField] private float normalLiftForce = 2f;

    [Header("Auto Kick")]
    [SerializeField] private float autoKickForce = 8f;
    [SerializeField] private float autoLiftForce = 0.1f;

    [Header("Speed Limit")]
    [SerializeField] private float maxSpeed = 20f;

    private Rigidbody rb;

    public bool HasScored { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void KickToPosition(
        Vector3 targetPosition,
        float kickForce,
        float liftForce)
    {
        if (HasScored)
            return;

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 force =
            direction * kickForce + Vector3.up * liftForce;

        rb.AddForce(force, ForceMode.Impulse);

        // Bao cho CameraController biet bong vua duoc sut.
        OnBallKicked?.Invoke(this);
    }

    // Kick thuong: giu luc sut manh.
    public void KickToTarget(Vector3 targetPosition)
    {
        KickToPosition(
            targetPosition,
            normalKickForce,
            normalLiftForce
        );
    }

    // Auto Kick: luc sut nhe hon, it bong.
    public void AutoKickToTarget(Vector3 targetPosition)
    {
        KickToPosition(
            targetPosition,
            autoKickForce,
            autoLiftForce
        );
    }

    private void FixedUpdate()
    {
        if (HasScored || rb.isKinematic)
            return;

        Vector3 horizontalVelocity = new Vector3(
            rb.velocity.x,
            0f,
            rb.velocity.z
        );

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity =
                horizontalVelocity.normalized * maxSpeed;

            rb.velocity = new Vector3(
                horizontalVelocity.x,
                rb.velocity.y,
                horizontalVelocity.z
            );
        }
    }

    public void MarkScored()
    {
        if (HasScored)
            return;

        HasScored = true;

        // Bao cho camera biet bong da vao khung thanh.
        OnBallScored?.Invoke(this);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }
}
