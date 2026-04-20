using UnityEngine;

public class GoalZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (WorldGameManager.Instance == null)
        {
            return;
        }

        if (other.GetComponentInParent<PlayerController>() != null)
        {
            WorldGameManager.Instance.CompleteLevel();
        }
    }
}
