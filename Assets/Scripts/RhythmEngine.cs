using System;
using System.Collections.Generic;
using UnityEngine;
using MidiParser;

[Serializable]
struct MusicNote
{
    public int note;
    public double time; // use double for precision
}

public class RhythmEngine : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bg;
    public AudioSource whistleSource;
    public AudioClip whistleClip;

    [Header("MIDI")]
    public string midiFileSource = "flo-rida-recorder";
    public int fallbackBPM = 104;
    public List<int> selectedChannels = new List<int>() { 1, 2 };

    [Header("Pitch")]
    public int sampleRootNote = 89; // F6 example

    private List<MusicNote> notes = new List<MusicNote>();
    private int currentNoteIndex = 0;
    private bool songStarted = false;

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

        foreach (var track in midiFile.Tracks)
        {
            foreach (var midiEvent in track.MidiEvents)
            {
                if (midiEvent.MidiEventType == MidiEventType.NoteOn)
                {
              

                    double noteTime =
                        (midiEvent.Time / (double)ppq) * secondsPerBeat;

                    notes.Add(new MusicNote
                    {
                        note = midiEvent.Note,
                        time = noteTime
                    });
                }
            }
        }

        notes.Sort((a, b) => a.time.CompareTo(b.time));
    }

    void StartSong()
    {
        double dspStart = AudioSettings.dspTime + 0.2;

        bg.PlayScheduled(dspStart);

        songStarted = true;
        currentNoteIndex = 0;
    }

    void Update()
    {
        if (!songStarted) return;

        double songTime = bg.time;

        while (currentNoteIndex < notes.Count &&
               songTime >= notes[currentNoteIndex].time)
        {
            PlayNote(notes[currentNoteIndex]);
            currentNoteIndex++;
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