
using UnityEngine;

public class GoalCelebrationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private MonoBehaviour playerController;

    [Header("Animation")]
    [SerializeField] private string celebrateTrigger = "Celebrate";

    private bool hasCelebrated;

    private void Update()
    {
        if (hasCelebrated)
            return;

        BallController[] balls =
            FindObjectsByType<BallController>(
                FindObjectsSortMode.None
            );

        if (balls.Length == 0)
            return;

        foreach (BallController ball in balls)
        {
            if (ball == null || !ball.HasScored)
                return;
        }

        Celebrate();
    }

    private void Celebrate()
    {
        hasCelebrated = true;

        if (playerController != null)
            playerController.enabled = false;

        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger(celebrateTrigger);
            playerAnimator.SetTrigger(celebrateTrigger);
        }

    }
}
