using System.Collections.Generic;
using UnityEngine;

public abstract class ActivationTriggerBase : MonoBehaviour
{
    private readonly HashSet<Collider> occupiers = new();
    private bool isLatched;

    protected bool IsActive => occupiers.Count > 0 || isLatched;

    protected abstract bool LatchAfterFirstActivation { get; }

    protected virtual void Start()
    {
        RefreshState();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!ActivationUtility.IsSupportedActivator(other))
        {
            return;
        }

        occupiers.Add(other);
        if (LatchAfterFirstActivation)
        {
            isLatched = true;
        }

        RefreshState();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (occupiers.Remove(other) && !isLatched)
        {
            RefreshState();
        }
    }

    private void RefreshState()
    {
        ApplyVisualState(IsActive);
        ApplyLinkedState(IsActive);
    }

    protected abstract void ApplyVisualState(bool isActive);

    protected abstract void ApplyLinkedState(bool isActive);
}
