using UnityEngine;

public static class WorldPhysicsUtility
{
    private const float GroundProbeHeight = 2f;
    private const float GroundProbeDistance = 6f;

    public static bool HasGroundBelow(Vector3 position)
    {
        return TryGetGroundHit(position, out _);
    }

    public static bool TryGetGroundHit(Vector3 position, out RaycastHit hit)
    {
        Vector3 rayStart = position + Vector3.up * GroundProbeHeight;
        return Physics.Raycast(rayStart, Vector3.down, out hit, GroundProbeDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
    }

    public static bool CanStandAtPositionInWorld(Vector3 position, WorldState targetWorld)
    {
        if (!TryGetGroundHit(position, out RaycastHit hit))
        {
            return false;
        }

        WorldBoundObject worldBoundObject = hit.collider.GetComponentInParent<WorldBoundObject>();
        return worldBoundObject == null || worldBoundObject.IsActiveInWorld(targetWorld);
    }
}
