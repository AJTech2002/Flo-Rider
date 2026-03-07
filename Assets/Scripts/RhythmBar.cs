using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmBar : MonoBehaviour
{
    public RhythmEngine engine;

    [Header("UI References")]
    public RectTransform barContainer;
    public Image barImage;
    public Image hitLineImage;
    public RectTransform notesParent;
    public Image notePrefab;

    [Header("Layout")]
    [Range(0f, 1f)]
    public float triggerPerc = 0.2f;

    [Header("Timing")]
    public float travelTime = 2f;
    public float postHitTravelTime = 0.25f;   // how long it continues after hit
    public float overshootDistance = 40f;     // how far past the bar it goes

    [Header("Style")]
    public float noteSize = 20f;
    public float bopScale = 1.4f;
    public Color idleColor = Color.gray;
    public Color hitColor = Color.green;
    public Color missColor = Color.red;

    private List<Image> spawnedNotes = new List<Image>();
    private bool[] noteConsumed; // prevents reprocessing after hit/miss

    void Start()
    {
        noteConsumed = new bool[engine.notes.Count];

        for (int i = 0; i < engine.notes.Count; i++)
        {
            Image img = Instantiate(notePrefab, notesParent);
            img.rectTransform.sizeDelta = Vector2.one * noteSize;
            spawnedNotes.Add(img);
        }
    }

    void Update()
    {
        if (engine == null || engine.notes == null || engine.notes.Count == 0)
            return;

        float barWidth = barContainer.rect.width;
        float leftEdge = -barWidth * 0.5f;
        float rightEdge = barWidth * 0.5f;

        float triggerX = Mathf.Lerp(leftEdge, rightEdge, triggerPerc);
        hitLineImage.rectTransform.anchoredPosition = new Vector2(triggerX, 0);

        double songTime = engine.bg.time;

        for (int i = 0; i < engine.notes.Count; i++)
        {
            if (noteConsumed[i])
                continue;

            double noteTime = engine.notes[i].time;
            double timeUntilHit = noteTime - songTime;

            Image noteImage = spawnedNotes[i];
            RectTransform rt = noteImage.rectTransform;
            NoteResult result = engine.noteResults[i];

            // Missed completely → delete
            if (result == NoteResult.Miss)
            {
                noteImage.color = missColor;
                noteConsumed[i] = true;
                Destroy(noteImage.gameObject, 0.5f);
                
            }

            // Hit → go green, travel past bar, then delete
            else if (result == NoteResult.Hit)
            {
                
                rt.localScale = Vector3.one * bopScale;
                noteImage.color = hitColor;
                noteConsumed[i] = true;
                Destroy(noteImage.gameObject, 0.1f);

                continue;
            }
            else
            {
                noteImage.color = idleColor;

            }

            // Normal movement before hit
            if (timeUntilHit > travelTime)
            {
                noteImage.gameObject.SetActive(false);
                continue;
            }

            noteImage.gameObject.SetActive(true);

            float t = (float)(1.0 - (timeUntilHit / travelTime));
            float xPosNormal = Mathf.Lerp(rightEdge, leftEdge, t);

            if (rt)
            {
                rt.anchoredPosition = new Vector2(xPosNormal, 0);
                rt.localScale = Vector3.one;
            }
        }
    }
}