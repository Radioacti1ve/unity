using UnityEngine;

public abstract class MovingMechanismBase : MonoBehaviour
{
    public Transform movingPart;
    public float movementSpeed = 3f;

    private bool isActive;

    protected abstract Vector3 InactiveLocalPosition { get; }
    protected abstract Vector3 ActiveLocalPosition { get; }
    protected abstract MeshRenderer StateRenderer { get; }
    protected abstract Color InactiveColor { get; }
    protected abstract Color ActiveColor { get; }

    protected virtual void Awake()
    {
        if (movingPart == null)
        {
            movingPart = transform;
        }
    }

    protected virtual void Start()
    {
        if (movingPart != null)
        {
            movingPart.localPosition = isActive ? ActiveLocalPosition : InactiveLocalPosition;
        }

        ApplyVisuals();
    }

    protected virtual void Update()
    {
        if (movingPart == null)
        {
            return;
        }

        movingPart.localPosition = Vector3.MoveTowards(
            movingPart.localPosition,
            isActive ? ActiveLocalPosition : InactiveLocalPosition,
            movementSpeed * Time.deltaTime);
    }

    protected void SetActiveState(bool shouldBeActive)
    {
        if (isActive == shouldBeActive)
        {
            return;
        }

        isActive = shouldBeActive;
        ApplyVisuals();
    }

    private void ApplyVisuals()
    {
        if (StateRenderer != null)
        {
            StateRenderer.material.color = isActive ? ActiveColor : InactiveColor;
        }
    }
}
