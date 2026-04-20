using System;
using UnityEngine;

public static class WorldGameGui
{
    private static readonly Color RealityHudColor = new(1f, 0.78f, 0.38f);
    private static readonly Color SpiritHudColor = new(0.48f, 0.9f, 1f);
    private static readonly Color ModalBackdropColor = new(0f, 0f, 0f, 0.65f);
    private static readonly Color ModalWindowColor = new(0.92f, 0.97f, 1f, 1f);
    private static readonly Color ModalTextColor = new(0.1f, 0.14f, 0.18f, 1f);
    private static readonly Color ModalButtonColor = new(0.34f, 0.84f, 1f, 1f);

    public static void Draw(WorldState currentWorld, bool isLevelComplete, Color overlayColor, Action restartAction)
    {
        Color previousColor = GUI.color;
        GUI.color = overlayColor;
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);

        DrawHud(currentWorld, previousColor);

        if (isLevelComplete)
        {
            DrawCompletionModal(restartAction);
        }

        GUI.color = previousColor;
    }

    private static void DrawHud(WorldState currentWorld, Color previousColor)
    {
        GUI.color = currentWorld == WorldState.Reality ? RealityHudColor : SpiritHudColor;

        string worldLabel = currentWorld == WorldState.Reality ? "Реальность" : "Мир духов";
        string helpText = $"Мир: {worldLabel}\nWASD - движение\nМышь - обзор\nQ - переключить мир\nR - начать заново\nEsc - курсор";
        GUI.Box(new Rect(12f, 12f, 250f, 118f), helpText);

        GUI.color = previousColor;
    }

    private static void DrawCompletionModal(Action restartAction)
    {
        const float width = 320f;
        const float height = 160f;
        Rect modalRect = new(
            (Screen.width - width) * 0.5f,
            (Screen.height - height) * 0.5f,
            width,
            height);

        GUI.color = ModalBackdropColor;
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);

        GUI.color = ModalWindowColor;
        GUI.Box(modalRect, "Уровень пройден");

        GUI.color = ModalTextColor;
        GUI.Label(new Rect(modalRect.x + 32f, modalRect.y + 42f, modalRect.width - 64f, 32f), "Путь завершен. Можно пройти его снова.");

        GUI.color = ModalButtonColor;
        if (GUI.Button(new Rect(modalRect.x + 70f, modalRect.y + 94f, modalRect.width - 140f, 34f), "Начать сначала"))
        {
            restartAction?.Invoke();
        }
    }
}
