
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoKickController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Button autoKickButton;
    [SerializeField] private GoalDetector[] goals;

    [Header("Cooldown")]
    [SerializeField] private float cooldownTime = 1f;

    private bool isOnCooldown = false;
    private Coroutine cooldownCoroutine;

    private void Start()
    {
        if (autoKickButton == null)
            autoKickButton = GetComponent<Button>();

        if (autoKickButton != null)
            autoKickButton.onClick.AddListener(AutoKick);
    }

    public void AutoKick()
    {
        if (isOnCooldown)
            return;

        if (player == null || goals == null || goals.Length == 0)
            return;

        BallController[] balls =
            FindObjectsByType<BallController>(
                FindObjectsSortMode.None
            );

        BallController farthestBall = null;
        float maxDistance = -1f;

        foreach (BallController ball in balls)
        {
            if (ball == null ||
                !ball.gameObject.activeInHierarchy ||
                ball.HasScored)
            {
                continue;
            }

            float distance = Vector3.Distance(
                player.position,
                ball.transform.position
            );

            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestBall = ball;
            }
        }

        if (farthestBall == null)
        {

            return;
        }

        GoalDetector nearestGoal = null;
        float minDistance = float.MaxValue;

        foreach (GoalDetector goal in goals)
        {
            if (goal == null)
                continue;

            float distance = Vector3.Distance(
                farthestBall.transform.position,
                goal.GetTargetPosition()
            );

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestGoal = goal;
            }
        }

        if (nearestGoal == null)
            return;

        farthestBall.AutoKickToTarget(
            nearestGoal.GetTargetPosition()
        );

        StartCooldown();
    }

    private void StartCooldown()
    {
        isOnCooldown = true;

        if (autoKickButton != null)
            autoKickButton.interactable = false;

        cooldownCoroutine = StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldownTime);

        isOnCooldown = false;
        cooldownCoroutine = null;

        if (autoKickButton != null)
            autoKickButton.interactable = true;
    }

    private void OnDestroy()
    {
        if (autoKickButton != null)
            autoKickButton.onClick.RemoveListener(AutoKick);

        if (cooldownCoroutine != null)
            StopCoroutine(cooldownCoroutine);
    }
}
