using UnityEngine;

[System.Serializable]
public struct WorldPose
{
    public Vector3 position;
    public Quaternion rotation;

    public WorldPose(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
