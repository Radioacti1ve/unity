using UnityEngine;

public class WorldBoundObject : MonoBehaviour
{
    public bool activeInReality = true;
    public bool activeInSpirit = true;
    public Renderer[] renderersToToggle;
    public Collider[] collidersToToggle;
    
    private bool isSubscribed;

    private void Awake()
    {
        RefreshBindingsIfNeeded();
    }

    private void OnEnable()
    {
        BindToWorldManager();
    }

    private void Start()
    {
        BindToWorldManager();
    }

    private void OnDisable()
    {
        UnbindFromWorldManager();
    }

    private void OnDestroy()
    {
        UnbindFromWorldManager();
    }

    private void BindToWorldManager()
    {
        if (isSubscribed || WorldGameManager.Instance == null)
        {
            return;
        }

        WorldGameManager.Instance.WorldChanged += ApplyState;
        ApplyState(WorldGameManager.Instance.CurrentWorld);
        isSubscribed = true;
    }

    private void UnbindFromWorldManager()
    {
        if (!isSubscribed || WorldGameManager.Instance == null)
        {
            return;
        }

        WorldGameManager.Instance.WorldChanged -= ApplyState;
        isSubscribed = false;
    }

    public bool IsActiveInWorld(WorldState state)
    {
        return state == WorldState.Reality ? activeInReality : activeInSpirit;
    }

    private void RefreshBindingsIfNeeded()
    {
        if (renderersToToggle == null || renderersToToggle.Length == 0)
        {
            renderersToToggle = GetComponentsInChildren<Renderer>(true);
        }

        if (collidersToToggle == null || collidersToToggle.Length == 0)
        {
            collidersToToggle = GetComponentsInChildren<Collider>(true);
        }
    }

    private void ApplyState(WorldState state)
    {
        bool isActive = IsActiveInWorld(state);

        foreach (Renderer targetRenderer in renderersToToggle)
        {
            if (targetRenderer != null)
            {
                targetRenderer.enabled = isActive;
            }
        }

        foreach (Collider targetCollider in collidersToToggle)
        {
            if (targetCollider != null)
            {
                targetCollider.enabled = isActive;
            }
        }
    }
}
