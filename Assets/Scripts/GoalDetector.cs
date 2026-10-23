
using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform targetPoint;

    [Header("Goal VFX")]
    [SerializeField] private GameObject confettiPrefab;
    [SerializeField] private Vector3 vfxOffset = new Vector3(0f, 1f, 0f);

    public Vector3 GetTargetPosition()
    {
        return targetPoint != null
            ? targetPoint.position
            : transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        BallController ball = other.GetComponent<BallController>();

        if (ball == null)
            ball = other.GetComponentInParent<BallController>();

        if (ball == null || ball.HasScored)
            return;

     
        ball.MarkScored();

     
        if (confettiPrefab != null)
        {
            GameObject effect = Instantiate(
                confettiPrefab,
                ball.transform.position + vfxOffset,
                Quaternion.identity
            );

          
            ParticleSystem[] particles =
                effect.GetComponentsInChildren<ParticleSystem>(true);

            foreach (ParticleSystem particle in particles)
            {
                particle.Play(true);
            }

       
            Destroy(effect, 5f);
        }
        else
        {
            Debug.LogWarning(
                "Chua gan Confetti Prefab trong GoalDetector!"
            );
        }

        Debug.Log("Goal! Confetti Explosion - Stars");
    }
}
