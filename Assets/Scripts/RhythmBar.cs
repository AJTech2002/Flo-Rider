using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class RhythmBar : MonoBehaviour
{
    public RhythmEngine engine;

    [Header("Layout")]
    public float barWidth = 12f;
    public float barThickness = 0.15f;
    public float verticalOffset = 2.5f;
    public float hitLineHeight = 1.5f;

    [Range(0f, 1f)]
    public float triggerPerc = 0.2f; // 0 = left edge, 1 = right edge

    [Header("Timing")]
    public float travelTime = 2f;

    [Header("Style")]
    public float noteRadius = 0.25f;
    public float bopScale = 1.4f;
    public Color barColor = Color.white;
    public Color idleColor = Color.gray;
    public Color hitColor = Color.green;
    public Color missColor = Color.red;
    public Color hitLineColor = Color.yellow;

    void OnRenderObject()
    {
        if (engine == null || engine.notes == null || engine.notes.Count == 0)
            return;

        Draw.ZTest = UnityEngine.Rendering.CompareFunction.Always;
        Draw.BlendMode = ShapesBlendMode.Transparent;
        Draw.LineGeometry = LineGeometry.Flat2D;

        float camHeight = Camera.main.orthographicSize;
        Vector3 origin = new Vector3(0, -camHeight + verticalOffset, 0);

        float leftEdge = -barWidth * 0.5f;
        float rightEdge = barWidth * 0.5f;

        Vector3 left = origin + Vector3.right * leftEdge;
        Vector3 right = origin + Vector3.right * rightEdge;

        Draw.Line(left, right, barThickness, barColor);

        // Compute trigger X from percentage
        float triggerX = Mathf.Lerp(leftEdge, rightEdge, triggerPerc);
        Vector3 hitLinePos = origin + Vector3.right * triggerX;

        Draw.Line(hitLinePos + Vector3.down * hitLineHeight / 2.0f, hitLinePos + Vector3.up * hitLineHeight / 2.0f , barThickness, hitLineColor);

        double songTime = engine.bg.time;

        for (int i = 0; i < engine.notes.Count; i++)
        {
            double noteTime = engine.notes[i].time;
            double timeUntilHit = noteTime - songTime;

            if (timeUntilHit < -engine.toleranceAfterS)
                continue;

            if (timeUntilHit > travelTime)
                continue;

            float t = (float)(1.0 - (timeUntilHit / travelTime));

            float startX = rightEdge;
            float endX = triggerX;

            float xPos = Mathf.Lerp(startX, endX, t);
            Vector3 notePos = origin + Vector3.right * xPos;

            NoteResult result = engine.noteResults[i];

            Color color = idleColor;
            float radius = noteRadius;

            if (result == NoteResult.Miss)
            {
                color = missColor;
            }
            else if (result == NoteResult.Hit)
            {
                color = hitColor;
            }

            Draw.Disc(notePos, radius, color);
        }
    }
}