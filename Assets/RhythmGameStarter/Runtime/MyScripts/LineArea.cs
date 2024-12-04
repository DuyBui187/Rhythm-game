using RhythmGameStarter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineArea : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private NoteArea noteArea;

    private SongManager songManager;

    private void Start()
    {
        songManager = GetComponentInParent<SongManager>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Note")
        {
            var note = col.GetComponent<Note>();

            if (songManager.playAuto)
            {
                // Automatically trigger the long note press interaction
                if (note.action == Note.NoteAction.LongPress)
                {
                    noteArea.TriggerLongNote(note);
                    return;
                }

                // Simulate touch for auto-play functionality
                var fakeTouchAuto = new TouchWrapper
                {
                    phase = TouchPhase.Began // Simulate touch start
                };

                noteArea.TriggerNote(fakeTouchAuto);
            }
        }
    }
}