using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldGameManager : MonoBehaviour
{
    public static WorldGameManager Instance { get; private set; }

    [Header("Scene Links")]
    public PlayerController player;
    public ReplayClone clonePrefab;
    public Transform playerSpawnPoint;
    public Transform spiritSpawnPoint;

    [Header("Fallback Spawn")]
    public Vector3 emergencySpawnPosition = new(0f, 0.25f, -6f);
    public Vector3 emergencySpiritSpawnPosition = new(5f, 1.1f, 0f);

    [Header("Failure")]
    public float fallRestartHeight = -6f;

    [Header("Replay")]
    public float recordInterval = 0.08f;

    [Header("Input")]
    public Key keyForWorldSwap = Key.Q;
    public Key keyForRestart = Key.R;

    [Header("Visual Filter")]
    public Color spiritOverlayColor = new(0.18f, 0.45f, 0.62f, 0.18f);
    public Color realityOverlayColor = new(0.16f, 0.1f, 0.04f, 0.04f);

    public WorldState CurrentWorld { get; private set; } = WorldState.Reality;
    public bool IsLevelComplete { get; private set; }

    private readonly List<ReplayFrame> replayFrames = new();
    private float recordTimer;
    private ReplayClone activeClone;
    private WorldPose rememberedRealityPose;
    private WorldPose rememberedSpiritPose;

    public event Action<WorldState> WorldChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ResetRememberedWorldPoses();
        SnapPlayerToWorldPose(WorldState.Reality);
        BeginRealityRecording();
        BroadcastWorldState();
    }

    private void Update()
    {
        if (Keyboard.current == null || IsLevelComplete)
        {
            return;
        }

        if (Keyboard.current[keyForWorldSwap].wasPressedThisFrame)
        {
            ToggleWorld();
        }

        if (Keyboard.current[keyForRestart].wasPressedThisFrame)
        {
            RestartLevel();
        }

        if (player != null && player.transform.position.y < fallRestartHeight)
        {
            RestartLevel();
            return;
        }

        RememberCurrentWorldPoseIfSafe();

        if (CurrentWorld == WorldState.Reality)
        {
            RecordPlayerFrame();
        }
    }

    public void ToggleWorld()
    {
        Vector3 currentPosition = player != null ? player.transform.position : Vector3.zero;
        Quaternion currentRotation = player != null ? player.transform.rotation : Quaternion.identity;
        bool canKeepCurrentPositionInSpirit = WorldPhysicsUtility.CanStandAtPositionInWorld(currentPosition, WorldState.Spirit);
        bool canKeepCurrentPositionInReality = WorldPhysicsUtility.CanStandAtPositionInWorld(currentPosition, WorldState.Reality);

        if (CurrentWorld == WorldState.Reality)
        {
            if (replayFrames.Count == 0)
            {
                CaptureCurrentFrame();
            }

            RememberCurrentWorldPoseIfSafe();
            CurrentWorld = WorldState.Spirit;
            SpawnClone();
            BroadcastWorldState();
            SnapPlayerToWorldPose(
                WorldState.Spirit,
                canKeepCurrentPositionInSpirit ? currentPosition : null,
                canKeepCurrentPositionInSpirit ? currentRotation : null);
        }
        else
        {
            RememberCurrentWorldPoseIfSafe();
            CurrentWorld = WorldState.Reality;
            DestroyClone();
            BeginRealityRecording();
            BroadcastWorldState();
            SnapPlayerToWorldPose(
                WorldState.Reality,
                canKeepCurrentPositionInReality ? currentPosition : null,
                canKeepCurrentPositionInReality ? currentRotation : null);
        }
    }

    public void RestartLevel()
    {
        DestroyClone();
        CurrentWorld = WorldState.Reality;
        IsLevelComplete = false;

        if (player != null)
        {
            ResetRememberedWorldPoses();
            SnapPlayerToWorldPose(WorldState.Reality);
        }

        BeginRealityRecording();
        BroadcastWorldState();
    }

    public void CompleteLevel()
    {
        IsLevelComplete = true;
        DestroyClone();
    }

    private void BroadcastWorldState()
    {
        WorldChanged?.Invoke(CurrentWorld);
    }

    private void ResetRememberedWorldPoses()
    {
        rememberedRealityPose = ResolveSpawnPose(WorldState.Reality);
        rememberedSpiritPose = ResolveSpawnPose(WorldState.Spirit);
    }

    private void RememberCurrentWorldPoseIfSafe()
    {
        if (player == null || !WorldPhysicsUtility.HasGroundBelow(player.transform.position))
        {
            return;
        }

        if (CurrentWorld == WorldState.Reality)
        {
            rememberedRealityPose = new WorldPose(player.transform.position, player.transform.rotation);
        }
        else
        {
            rememberedSpiritPose = new WorldPose(player.transform.position, player.transform.rotation);
        }
    }

    private void SnapPlayerToWorldPose(WorldState targetWorld)
    {
        SnapPlayerToWorldPose(targetWorld, null, null);
    }

    private void SnapPlayerToWorldPose(WorldState targetWorld, Vector3? preferredPosition, Quaternion? preferredRotation)
    {
        if (player == null)
        {
            return;
        }

        WorldPose rememberedPose = GetRememberedPose(targetWorld);
        Vector3 targetPosition = preferredPosition ?? rememberedPose.position;
        Quaternion targetRotation = preferredRotation ?? rememberedPose.rotation;

        if (!WorldPhysicsUtility.HasGroundBelow(targetPosition))
        {
            targetPosition = rememberedPose.position;
            targetRotation = rememberedPose.rotation;
        }

        if (!WorldPhysicsUtility.HasGroundBelow(targetPosition))
        {
            WorldPose spawnPose = ResolveSpawnPose(targetWorld);
            targetPosition = spawnPose.position;
            targetRotation = spawnPose.rotation;
        }

        if (WorldPhysicsUtility.HasGroundBelow(targetPosition))
        {
            player.Teleport(targetPosition, targetRotation);
        }
    }

    private WorldPose GetRememberedPose(WorldState targetWorld)
    {
        return targetWorld == WorldState.Reality ? rememberedRealityPose : rememberedSpiritPose;
    }

    private WorldPose ResolveSpawnPose(WorldState targetWorld)
    {
        Transform targetSpawn = GetSpawnPoint(targetWorld);

        if (targetSpawn != null && WorldPhysicsUtility.HasGroundBelow(targetSpawn.position))
        {
            return new WorldPose(targetSpawn.position, targetSpawn.rotation);
        }

        return new WorldPose(
            targetWorld == WorldState.Reality ? emergencySpawnPosition : emergencySpiritSpawnPosition,
            targetSpawn != null ? targetSpawn.rotation : Quaternion.Euler(0f, 90f, 0f));
    }

    private Transform GetSpawnPoint(WorldState targetWorld)
    {
        return targetWorld == WorldState.Reality ? playerSpawnPoint : spiritSpawnPoint;
    }

    private void BeginRealityRecording()
    {
        replayFrames.Clear();
        recordTimer = 0f;
        CaptureCurrentFrame();
    }

    private void RecordPlayerFrame()
    {
        recordTimer += Time.deltaTime;

        if (recordTimer >= recordInterval)
        {
            recordTimer = 0f;
            CaptureCurrentFrame();
        }
    }

    private void CaptureCurrentFrame()
    {
        if (player == null)
        {
            return;
        }

        ReplayFrame frame = new ReplayFrame
        {
            position = player.transform.position,
            rotation = player.transform.rotation
        };

        replayFrames.Add(frame);
    }

    private void SpawnClone()
    {
        DestroyClone();

        if (clonePrefab == null || replayFrames.Count == 0)
        {
            return;
        }

        activeClone = Instantiate(clonePrefab);
        activeClone.gameObject.SetActive(true);
        activeClone.Initialize(replayFrames, recordInterval);
    }

    private void DestroyClone()
    {
        if (activeClone != null)
        {
            Destroy(activeClone.gameObject);
            activeClone = null;
        }
    }

    private void OnGUI()
    {
        Color overlayColor = CurrentWorld == WorldState.Reality ? realityOverlayColor : spiritOverlayColor;
        WorldGameGui.Draw(CurrentWorld, IsLevelComplete, overlayColor, HandleRestartFromModal);
    }

    private void HandleRestartFromModal()
    {
        RestartLevel();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
