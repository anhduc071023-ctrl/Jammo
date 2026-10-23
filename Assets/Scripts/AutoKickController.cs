
using UnityEngine;
using UnityEngine.UI;

public class AutoKickController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Button autoKickButton;
    [SerializeField] private GoalDetector[] goals;

    private void Start()
    {
        if (autoKickButton == null)
            autoKickButton = GetComponent<Button>();

        if (autoKickButton != null)
            autoKickButton.onClick.AddListener(AutoKick);
        else
            Debug.LogError("AutoKickController: Chua gan Auto Kick Button!");

        if (player == null)
            Debug.LogError("AutoKickController: Chua gan Player!");

        if (goals == null || goals.Length == 0)
            Debug.LogError("AutoKickController: Chua gan cac GoalDetector!");
    }

    public void AutoKick()
    {
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
            Debug.Log("Auto Kick: Khong con bong nao de sut!");
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
        {
            return;
        }

        farthestBall.AutoKickToTarget(
     nearestGoal.GetTargetPosition()
        );
      

    }

    private void OnDestroy()
    {
        if (autoKickButton != null)
            autoKickButton.onClick.RemoveListener(AutoKick);
    }
}
