using System;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;
using UnityEngine.UI;

public class NoteUI
{
    public MusicNote note;
    public Transform image;
    public NoteResult result;
}

public class RhythmBar : MonoBehaviour
{
    public RhythmEngine engine;

    [Header("UI References")] public Transform whistleIcon;
    public RectTransform barContainer;
    public Transform noteSpawnPoint;
    public GameObject notePrefab;
    public Image barLine;
    public Image whistleIconImage;
    
    
    public List<Sprite> noteTextures; // Assign in inspector, should correspond to note values

    private Dictionary<int, NoteUI> activeNotes = new Dictionary<int, NoteUI>();

    private float width
    {
        get { return barContainer.rect.width; }
    }

    private float barTotalDurationS = 2.0f; // How far ahead the bar should show notes, in seconds

    public void StartSong()
    {
        started = true;
    }

    public void Prepare()
    {
        barLine.color = Color.white;
        whistleIconImage.color = Color.white;   
    }
    
    public void Unprepare()
    {
        barLine.color = new Color(1f, 1f, 1f, 0.7f);
        whistleIconImage.color = new Color(1f, 1f, 1f, 0.7f);
    }

    private bool started = false;

    private void Update()
    {
        if (!started) return;
        
        List<MusicNote> notes = GetNotesInRange(engine.GetSongTime(), engine.GetSongTime() + barTotalDurationS);
        foreach (var note in notes)
        {
            if (!activeNotes.ContainsKey(note.id))
            {
                var noteUI = CreateNoteUI(note);
                activeNotes.Add(note.id, noteUI);
            }
        }
        
    }

    public void UpdateState(int noteId, NoteResult result)
    {
        
        if (activeNotes.ContainsKey(noteId))
        {
            if (result == NoteResult.Hit)
            {
                var noteUI = activeNotes[noteId];
                activeNotes[noteId].result = NoteResult.Hit;
                noteUI.image.GetComponent<Animator>().SetTrigger("Hit");
            }
            else if (result == NoteResult.Miss)
            {
                var noteUI = activeNotes[noteId];
                activeNotes[noteId].result = result;
                noteUI.image.GetComponent<Animator>().SetTrigger("Miss");
                
            }
            else if (result == NoteResult.Wrong)
            {
                var noteUI = activeNotes[noteId];
                activeNotes[noteId].result = result;
                noteUI.image.GetComponent<Animator>().SetTrigger("Wrong");
            }
        }
    }

    int nextNoteIndex = 0;
    IEnumerator UpdateNote(NoteUI ui)
    {
        float endX = whistleIcon.position.x;

        // Move toward hit line
        while (engine.GetSongTime() < ui.note.time)
        {
            if (!ui.image) yield break;

            // Early press = Wrong
            if (engine.spacePressed && nextNoteIndex == ui.note.id)
            {
                if (engine.GetSongTime() < ui.note.time - engine.toleranceBeforeS)
                {
                    UpdateState(ui.note.id, NoteResult.Wrong);
                    engine.PlayNote(ui.note);
                }
                else
                {
                    UpdateState(ui.note.id, NoteResult.Hit);
                    engine.PlayNote(ui.note);
                    
                    nextNoteIndex = Math.Max(nextNoteIndex, ui.note.id + 1);
                    yield break;
                }
            }

            float timeToNote = (float)(ui.note.time - engine.GetSongTime());
            float t = 1 - (timeToNote / barTotalDurationS);
            float xPos = Mathf.Lerp(noteSpawnPoint.position.x, endX, t);

            ui.image.position = new Vector3(
                xPos,
                ui.image.position.y,
                ui.image.position.z
            );

            yield return null;
        }

        float waitTime = engine.toleranceBeforeS;
        while (waitTime > 0)
        {
            if (!ui.image) yield break;
            
            if (engine.spacePressed)
            {
                UpdateState(ui.note.id, NoteResult.Hit);
                engine.PlayNote(ui.note);

                nextNoteIndex = Math.Max(nextNoteIndex, ui.note.id + 1);
                yield break;
            }
            waitTime -= Time.deltaTime; 
            yield return null;
        }

        // If we exit window without hit → Miss
        UpdateState(ui.note.id, NoteResult.Miss);
        if (nextNoteIndex == ui.note.id)
        {
            nextNoteIndex++;
        }

        // Small delay before cleanup
        float afterS = 0.5f;

        while (afterS > 0)
        {
            afterS -= Time.deltaTime;

            if (!ui.image) yield break;

            ui.image.position += Vector3.left *
                                 (width / barTotalDurationS) *
                                 Time.deltaTime;

            yield return null;
        }

        if (ui.image)
            Destroy(ui.image.gameObject);
    }

    public NoteUI CreateNoteUI(MusicNote note)
    {
        GameObject noteGO = Instantiate(notePrefab, barContainer);

        Image img = noteGO.GetComponentInChildren<Image>();
        img.color = Color.white; 
        img.sprite = noteTextures[note.note % noteTextures.Count];

        RectTransform rt = noteGO.GetComponent<RectTransform>();
        rt.position = noteSpawnPoint.position; 
        
        // add random rotation 
        rt.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-15f, 15f));

        NoteUI noteUI = new NoteUI { note = note, image = noteGO.transform, result = NoteResult.Pending};
        StartCoroutine(UpdateNote(noteUI));
        return noteUI;
    }


// #region Utilities
    public List<MusicNote> GetNotesInRange(float start, float end)
    {
        var temp = new List<MusicNote>();
        for (int i = engine.upcoming; i < engine.notes.Count; i++)
        {
            if (engine.notes[i].time > end)
            {
                break;
            }

            if (engine.notes[i].time >= start)
            {
                temp.Add(engine.notes[i]);
            }
        }

        return temp;
    }
    // #endregion
}