using UnityEngine;

public static class ActivationUtility
{
    public static bool IsSupportedActivator(Collider other)
    {
        return other.GetComponentInParent<PlayerController>() != null ||
               other.GetComponentInParent<ReplayClone>() != null;
    }
}
