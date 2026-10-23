
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Player Movement")]
    [SerializeField] private MonoBehaviour playerController;

    [Header("Top-Down Camera")]
    [SerializeField] private float height = 15f;
    [SerializeField] private Vector3 offset = Vector3.zero;

    [Header("Smooth Follow")]
    [SerializeField] private float playerSmoothTime = 0.15f;
    [SerializeField] private float ballSmoothTime = 0.08f;

    [Header("Ball Camera")]
    [SerializeField] private float ballHeight = 10f;
    [SerializeField] private Vector3 ballOffset = Vector3.zero;
    [SerializeField] private float returnDelay = 2f;


    private Transform target;
    private BallController currentBall;
    private Coroutine returnCoroutine;

    private Vector3 smoothVelocity;

    private void Start()
    {
        target = player;

        if (target != null)
            SnapToTarget();
    }

    private void OnEnable()
    {
        BallController.OnBallKicked += FollowBall;
        BallController.OnBallScored += OnBallScored;
    }

    private void OnDisable()
    {
        BallController.OnBallKicked -= FollowBall;
        BallController.OnBallScored -= OnBallScored;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        bool followingBall =
            currentBall != null &&
            target == currentBall.transform;

        float currentHeight = followingBall ? ballHeight : height;

        Vector3 currentOffset = followingBall
            ? ballOffset
            : offset;

        Vector3 desiredPosition = target.position +
            new Vector3(
                currentOffset.x,
                currentHeight + currentOffset.y,
                currentOffset.z
            );

        float smoothTime = followingBall
            ? ballSmoothTime
            : playerSmoothTime;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref smoothVelocity,
            smoothTime
        );

 
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void FollowBall(BallController ball)
    {
        if (ball == null || ball.HasScored)
            return;

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        if (playerController != null)
            playerController.enabled = false;

        currentBall = ball;
        target = ball.transform;

        smoothVelocity = Vector3.zero;
    }

    private void OnBallScored(BallController ball)
    {
        if (ball == null || ball != currentBall)
            return;

        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);

        returnCoroutine = StartCoroutine(ReturnToPlayer());
    }

    private IEnumerator ReturnToPlayer()
    {
        yield return new WaitForSeconds(returnDelay);

        target = player;
        currentBall = null;
        smoothVelocity = Vector3.zero;
        returnCoroutine = null;

        if (playerController != null)
            playerController.enabled = true;
    }

    private void SnapToTarget()
    {
        Vector3 desiredPosition = target.position +
            new Vector3(offset.x, height + offset.y, offset.z);

        transform.position = desiredPosition;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
