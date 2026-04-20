using UnityEngine;

public class LeverHoldTrigger : ActivationTriggerBase
{
    public BridgeController[] linkedBridges;
    public GateController[] linkedGates;
    public Transform handleVisual;
    public MeshRenderer leverRenderer;
    public Color idleColor = new(0.55f, 0.33f, 0.16f);
    public Color activeColor = new(1f, 0.82f, 0.34f);
    public bool latchAfterFirstActivation = true;

    private Quaternion idleRotation;
    private Quaternion activeRotation;

    protected override bool LatchAfterFirstActivation => latchAfterFirstActivation;

    protected override void Start()
    {
        if (handleVisual != null)
        {
            idleRotation = handleVisual.localRotation;
            activeRotation = Quaternion.Euler(-35f, 0f, 0f);
        }

        base.Start();
    }

    protected override void ApplyVisualState(bool isActive)
    {
        if (handleVisual != null)
        {
            handleVisual.localRotation = isActive ? activeRotation : idleRotation;
        }

        if (leverRenderer != null)
        {
            leverRenderer.material.color = isActive ? activeColor : idleColor;
        }
    }

    protected override void ApplyLinkedState(bool isActive)
    {
        foreach (BridgeController bridge in linkedBridges)
        {
            if (bridge != null)
            {
                bridge.SetExtended(isActive);
            }
        }

        foreach (GateController gate in linkedGates)
        {
            if (gate != null)
            {
                gate.SetOpen(isActive);
            }
        }
    }
}
