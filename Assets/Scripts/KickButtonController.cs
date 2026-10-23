
using UnityEngine;
using UnityEngine.UI;

public class KickButtonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject kickButton;
    [SerializeField] private Animator animator;
    [SerializeField] private GoalDetector[] goals;

    [Header("Settings")]
    [SerializeField] private float kickDistance = 2.5f;
    [SerializeField] private float kickCooldown = 0.6f;
    [SerializeField] private float searchInterval = 0.15f;

    private BallController nearestBall;
    private float nextKickTime;
    private float nextSearchTime;

    private void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (kickButton != null)
        {
            kickButton.SetActive(false);

            Button button = kickButton.GetComponent<Button>();

            if (button != null)
            {
                button.onClick.RemoveListener(OnKickClicked);
                button.onClick.AddListener(OnKickClicked);
            }
        }
    }

    private void Update()
    {
        if (kickButton == null)
            return;

        if (Time.time >= nextSearchTime)
        {
            nextSearchTime = Time.time + searchInterval;
            FindNearestBall();
        }
    }

    private void FindNearestBall()
    {
        nearestBall = null;
        float shortestDistance = kickDistance;

        BallController[] balls =
            FindObjectsByType<BallController>(FindObjectsSortMode.None);

        Vector3 playerPosition = transform.position;
        playerPosition.y = 0f;

        foreach (BallController ball in balls)
        {
            if (ball == null || !ball.gameObject.activeInHierarchy)
                continue;

            Vector3 ballPosition = ball.transform.position;
            ballPosition.y = 0f;

            float distance = Vector3.Distance(playerPosition, ballPosition);

            if (distance <= shortestDistance)
            {
                shortestDistance = distance;
                nearestBall = ball;
            }
        }

        kickButton.SetActive(nearestBall != null);
    }

    public void OnKickClicked()
    {
        if (Time.time < nextKickTime)
            return;

        FindNearestBall();

        if (nearestBall == null)
            return;

        if (goals == null || goals.Length == 0)
        {
     
            return;
        }

        GoalDetector nearestGoal = null;
        float shortestGoalDistance = float.MaxValue;

        Vector3 ballPosition = nearestBall.transform.position;
        ballPosition.y = 0f;

        foreach (GoalDetector goal in goals)
        {
            if (goal == null)
                continue;

            Vector3 goalPosition = goal.GetTargetPosition();
            goalPosition.y = 0f;

            float distance = Vector3.Distance(ballPosition, goalPosition);

            if (distance < shortestGoalDistance)
            {
                shortestGoalDistance = distance;
                nearestGoal = goal;
            }
        }

        if (nearestGoal == null)
        {
            return;
        }

        nextKickTime = Time.time + kickCooldown;

        nearestBall.KickToTarget(nearestGoal.GetTargetPosition());

        FindNearestBall();
    }
}
