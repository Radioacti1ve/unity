using UnityEngine;

public class PressurePlate : ActivationTriggerBase
{
    public GateController[] linkedGates;
    public BridgeController[] linkedBridges;
    public Transform plateVisual;
    public MeshRenderer plateRenderer;
    public Color idleColor = new(0.18f, 0.24f, 0.26f);
    public Color activeColor = new(0.43f, 0.95f, 1f);
    public bool staysPressedAfterFirstActivation;

    protected override bool LatchAfterFirstActivation => staysPressedAfterFirstActivation;

    protected override void ApplyVisualState(bool isActive)
    {
        if (plateVisual != null)
        {
            Vector3 localPosition = plateVisual.localPosition;
            localPosition.y = isActive ? 0.08f : 0.18f;
            plateVisual.localPosition = localPosition;
        }

        if (plateRenderer != null)
        {
            plateRenderer.material.color = isActive ? activeColor : idleColor;
        }
    }

    protected override void ApplyLinkedState(bool isActive)
    {
        foreach (GateController gate in linkedGates)
        {
            if (gate != null)
            {
                gate.SetOpen(isActive);
            }
        }

        foreach (BridgeController bridge in linkedBridges)
        {
            if (bridge != null)
            {
                bridge.SetExtended(isActive);
            }
        }
    }
}
