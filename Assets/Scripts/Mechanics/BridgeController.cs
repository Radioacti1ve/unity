using UnityEngine;

public class BridgeController : MovingMechanismBase
{
    public Vector3 retractedLocalPosition;
    public Vector3 extendedLocalPosition;
    public MeshRenderer bridgeRenderer;
    public Color inactiveColor = new(0.18f, 0.24f, 0.26f);
    public Color activeColor = new(0.45f, 1f, 0.7f);

    protected override Vector3 InactiveLocalPosition => retractedLocalPosition;
    protected override Vector3 ActiveLocalPosition => extendedLocalPosition;
    protected override MeshRenderer StateRenderer => bridgeRenderer;
    protected override Color InactiveColor => inactiveColor;
    protected override Color ActiveColor => activeColor;

    public void SetExtended(bool shouldExtend)
    {
        SetActiveState(shouldExtend);
    }
}
