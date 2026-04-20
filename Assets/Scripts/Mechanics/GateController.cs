using UnityEngine;

public class GateController : MovingMechanismBase
{
    public float raisedHeight = 3.8f;
    public MeshRenderer gateRenderer;
    public Color closedColor = new(0.33f, 0.18f, 0.12f);
    public Color openColor = new(0.45f, 1f, 0.7f);

    private Vector3 closedPosition;
    private Vector3 openPosition;

    protected override Vector3 InactiveLocalPosition => closedPosition;
    protected override Vector3 ActiveLocalPosition => openPosition;
    protected override MeshRenderer StateRenderer => gateRenderer;
    protected override Color InactiveColor => closedColor;
    protected override Color ActiveColor => openColor;

    protected override void Awake()
    {
        base.Awake();

        closedPosition = movingPart.localPosition;
        openPosition = closedPosition + Vector3.up * raisedHeight;
    }

    public void SetOpen(bool shouldOpen)
    {
        SetActiveState(shouldOpen);
    }
}
