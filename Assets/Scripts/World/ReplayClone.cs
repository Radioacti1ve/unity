using System.Collections.Generic;
using UnityEngine;

public class ReplayClone : MonoBehaviour
{
    public float playbackSpeed = 1f;

    private readonly List<ReplayFrame> frames = new();
    private float frameDuration = 0.08f;
    private float elapsedTime;

    public void Initialize(IReadOnlyList<ReplayFrame> sourceFrames, float sampleRate)
    {
        frames.Clear();
        frames.AddRange(sourceFrames);
        frameDuration = Mathf.Max(0.01f, sampleRate);
        elapsedTime = 0f;

        if (frames.Count > 0)
        {
            transform.SetPositionAndRotation(frames[0].position, frames[0].rotation);
        }
    }

    private void Update()
    {
        if (frames.Count == 0)
        {
            return;
        }

        elapsedTime += Time.deltaTime * playbackSpeed;

        int lastIndex = frames.Count - 1;
        float totalDuration = lastIndex * frameDuration;

        if (elapsedTime >= totalDuration)
        {
            transform.SetPositionAndRotation(frames[lastIndex].position, frames[lastIndex].rotation);
            return;
        }

        int currentIndex = Mathf.FloorToInt(elapsedTime / frameDuration);
        int nextIndex = Mathf.Min(currentIndex + 1, lastIndex);
        float t = (elapsedTime - currentIndex * frameDuration) / frameDuration;

        Vector3 position = Vector3.Lerp(frames[currentIndex].position, frames[nextIndex].position, t);
        Quaternion rotation = Quaternion.Slerp(frames[currentIndex].rotation, frames[nextIndex].rotation, t);
        transform.SetPositionAndRotation(position, rotation);
    }
}
