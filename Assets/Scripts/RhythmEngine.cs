using System;
using System.Collections.Generic;
using UnityEngine;
using MidiParser;

[Serializable]
public struct MusicNote
{
    public int note;
    public double time;
}

public enum NoteResult
{
    Pending,
    Hit,
    Miss
}

public class RhythmEngine : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bg;
    public AudioSource whistleSource;
    public AudioClip whistleClip;
    public AudioSource whistleLoop;

    [Header("MIDI")]
    public string midiFileSource = "flo-rida-recorder";
    public int fallbackBPM = 104;
    public List<int> selectedChannels = new List<int>() { 1, 2 };

    [Header("Pitch")]
    public int sampleRootNote = 89;

    public List<MusicNote> notes = new List<MusicNote>();
    public List<NoteResult> noteResults = new List<NoteResult>();

    private bool songStarted = false;

    public int upcoming = 0;

    [Header("Rhythm")]
    public float toleranceBeforeS = 0.1f;
    public float toleranceAfterS = 0.1f;

    void Start()
    {
        LoadMidi();
        StartSong();
    }

    void LoadMidi()
    {
        var midiFile = new MidiFile(
            Application.streamingAssetsPath + "/" + midiFileSource + ".mid"
        );

        int ppq = midiFile.TicksPerQuarterNote;
        double bpm = fallbackBPM;
        double secondsPerBeat = 60.0 / bpm;

        double lastNote = -0.1f;

        List<MusicNote> temp = new List<MusicNote>();
        foreach (var track in midiFile.Tracks)
        {
            foreach (var midiEvent in track.MidiEvents)
            {
                if (midiEvent.MidiEventType == MidiEventType.NoteOn)
                {
                    double noteTime =
                        (midiEvent.Time / (double)ppq) * secondsPerBeat;
                    temp.Add(new MusicNote
                    {
                        note = midiEvent.Note,
                        time = noteTime
                    });

                }
            }
        }

        temp.Sort((a, b) => a.time.CompareTo(b.time));

        for (int i = 0; i < temp.Count; i++)
        {
            print(temp[i].time + " - " + lastNote);
            if (temp[i].time - lastNote > 0.1f)
            {
                notes.Add(temp[i]);
                lastNote = temp[i].time;
            }
        }
        
        noteResults.Clear();
        for (int i = 0; i < notes.Count; i++)
            noteResults.Add(NoteResult.Pending);
    }

    void StartSong()
    {
        double dspStart = AudioSettings.dspTime + 0.2;
        bg.PlayScheduled(dspStart);

        songStarted = true;
        upcoming = 0;
    }

    void Update()
    {
        if (!songStarted || upcoming >= notes.Count)
            return;

        double songTime = bg.time;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            double noteTime = notes[upcoming].time;

            if (songTime < noteTime - toleranceBeforeS)
            {
                print("Too early!");
                PlayNote(notes[upcoming]);
                noteResults[upcoming] = NoteResult.Miss;

                return;
            }

            if (songTime > noteTime + toleranceAfterS)
            {
                print("Too late!");
                PlayNote(notes[upcoming]);

                noteResults[upcoming] = NoteResult.Miss;
                upcoming++;
                return;
            }

            print("PERFECT!");
            noteResults[upcoming] = NoteResult.Hit;
            PlayNote(notes[upcoming]);
            upcoming++;
        }

        if (upcoming < notes.Count &&
            songTime > notes[upcoming].time + toleranceAfterS)
        {
            print("Missed a note!!");
            noteResults[upcoming] = NoteResult.Miss;
            upcoming++;
        }
    }

    void PlayNote(MusicNote note)
    {
        float semitoneOffset = note.note - sampleRootNote;
        float pitch = Mathf.Pow(2f, semitoneOffset / 12f);

        whistleSource.pitch = pitch;
        whistleSource.PlayOneShot(whistleClip);
    }
}