using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiParser;

[Serializable]
public class MusicNote
{
    public int id;
    public int note;
    public double time;
    public NoteResult result = NoteResult.Pending;
}

public enum NoteResult
{
    Pending,
    Hit,
    Miss,
    Wrong
}

public class RhythmEngine : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bg;
    public AudioSource whistleSource;
    public AudioClip whistleClip;
    public AudioSource whistleLoop;
    public Animator whistle;

    [Header("MIDI")]
    public string midiFileSource = "flo-rida-recorder";
    public int fallbackBPM = 104;
    public List<int> selectedChannels = new List<int>() { 1, 2 };

    [Header("Pitch")]
    public int sampleRootNote = 89;

    public List<MusicNote> notes = new List<MusicNote>();

    private bool songStarted = false;

    public int upcoming = 0;

    [Header("Rhythm")]
    public float toleranceBeforeS = 0.1f;
    public RhythmBar bar;
    public float startingSongSpeed = 0.8f;
    public float maxSongSpeed = 1.2f;
    void Start()
    {
        LoadMidi();
        SetSongSpeed(startingSongSpeed);
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
        int noteID = 0;

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
                        id = -1,
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
            if (temp[i].time - lastNote > toleranceBeforeS)
            {
                temp[i].id = noteID;
                noteID++;   
                notes.Add(temp[i]);
                lastNote = temp[i].time;
            }
        }
        
    }

    void StartSong()
    {
        double dspStart = AudioSettings.dspTime + 0.2;
        bg.PlayScheduled(dspStart);
        
        if (bar)
        bar.StartSong();
        
        songStarted = true;
        upcoming = 0;
            StartCoroutine(SongSpeedController());
    }
    
    public void StopSong()
    {
        bg.Stop();
        songStarted = false;
    }

    IEnumerator SongSpeedController()
    {
        // Every 10 seconds, increase the song speed by 0.05, up to maxSongSpeed
        while (songStarted)
        {
            yield return new WaitForSeconds(15);
            float currentSpeed = bg.pitch;
            float newSpeed = Mathf.Min(currentSpeed + 0.02f, maxSongSpeed);
            SetSongSpeed(newSpeed);
        }
    }
    
    public  void SetSongSpeed(float speed)
    {
        bg.pitch = speed;
    }

    public float GetSongTime()
    {
        if (!songStarted)
            return 0;

        return bg.time;    }
    
    
    public List<MusicNote> GetNotesInRange(float start, float end)
    {
        var temp = new List<MusicNote>();
        for (int i = 0; i < notes.Count; i++)
        {
            if (notes[i].time > end)
            {
                break;
            }

            if (notes[i].time >= start)
            {
                temp.Add(notes[i]);
            }
        }

        return temp;
    }

    public bool spacePressed = false;
    void Update()
    {
        if (!songStarted || upcoming >= notes.Count)
            return;

        double songTime = bg.time;

        List<MusicNote> nearbyNotes = GetNotesInRange(
            (float)(songTime - toleranceBeforeS),
            (float)(songTime)
        );

        if (nearbyNotes.Count > 0)
        {
            bar.Prepare();
        }
        else
        {
            bar.Unprepare();
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            spacePressed = true;
            whistle.SetTrigger("blow");
            
            // double noteTime = notes[upcoming].time;
            //
            // bool alreadyPlayed = false;
            //
            // // Handle upcoming
            // if (songTime < noteTime - toleranceBeforeS && notes[upcoming].result == NoteResult.Pending)
            // {
            //     print("Too early!");
            //     bar.UpdateState(notes[upcoming].id, NoteResult.Wrong);
            // }
            //
            // for (int i = 0; i < nearbyNotes.Count; i++)
            // {
            //     if (nearbyNotes[i].result == NoteResult.Pending)
            //     {
            //
            //         print("PERFECT!");
            //         nearbyNotes[i].result = NoteResult.Hit;
            //         if (!alreadyPlayed)
            //         {
            //             PlayNote(nearbyNotes[i]);
            //             alreadyPlayed = true;
            //         }
            //
            //         bar.UpdateState(nearbyNotes[i].id, NoteResult.Hit);
            //         // upcoming++;
            //     }
            // }
            //
            // if (!alreadyPlayed)
            // {
            //     PlayNote(notes[upcoming]);
            // }
        }
        else
        {
            spacePressed = false;
        }
        // if (upcoming < notes.Count &&
        //     songTime > notes[upcoming].time + toleranceAfterS)
        // {
        //     print("Missed a note!!");
        //     noteResults[upcoming] = NoteResult.Miss;
        //     bar.UpdateState(notes[upcoming].id, NoteResult.Miss);
        //
        //     upcoming++;
        // }
        
        // Check for ALL missed notes
        // for (int i = 0; i < notes.Count; i++)
        // {
        //     if (notes[i].result == NoteResult.Pending &&
        //         songTime > notes[i].time + toleranceAfterS)
        //     {
        //         print("Missed a note!!");
        //         notes[i].result = NoteResult.Miss;
        //         bar.UpdateState(notes[i].id, NoteResult.Miss);
        //      
        //     }
        //
        //     if (notes[i].result == NoteResult.Pending)
        //     {
        //         upcoming = i;   
        //         break;
        //     }
        // }
        
        // while (upcoming < notes.Count &&
        //        notes[upcoming].result == NoteResult.Pending &&
        //        songTime > notes[upcoming].time + toleranceAfterS)
        // {
        //     print("Missed a note!!");
        //
        //     notes[upcoming].result = NoteResult.Miss;
        //     bar.UpdateState(notes[upcoming].id, NoteResult.Miss);
        //
        //     upcoming++;
        // }
    }

    public void PlayNote(MusicNote note)
    {
        float semitoneOffset = note.note - sampleRootNote;
        float pitch = Mathf.Pow(2f, semitoneOffset / 12f);

        whistleSource.pitch = pitch;
        whistleSource.PlayOneShot(whistleClip);
    }
}