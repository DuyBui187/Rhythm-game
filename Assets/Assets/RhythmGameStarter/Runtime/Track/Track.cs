using System.Collections.Generic;
using UnityEngine;

namespace RhythmGameStarter
{
    public class Track : MonoBehaviour
    {
        public Transform lineArea;
        public NoteArea noteArea;

        [HideInInspector]
        public Transform notesParent;

        [HideInInspector]
        public Transform trapsParent;

        [HideInInspector]
        public IEnumerable<SongItem.MidiNote> allNotes;

        [HideInInspector]
        public AudioSource trackHitAudio;

        [HideInInspector]
        public List<Note> runtimeNote;
        public List<Trap> runtimeTrap;

        void Awake()
        {
            trackHitAudio = GetComponent<AudioSource>();
            notesParent = new GameObject("Notes").transform;
            notesParent.SetParent(transform);

            trapsParent = new GameObject("Traps").transform;
            trapsParent.SetParent(transform);

            notesParent.localScale = Vector3.one;
            trapsParent.localScale = Vector3.one;

            ResetTrackPosition();
        }

        private void ResetTrackPosition()
        {
            notesParent.transform.position = lineArea.position;
            notesParent.transform.localEulerAngles = Vector3.zero;

            trapsParent.transform.position = lineArea.position;
            trapsParent.transform.localEulerAngles = Vector3.zero;
        }

        // Note

        public GameObject CreateNote(GameObject prefab)
        {
            var ordinalScale = prefab.transform.localScale;

            var note = Instantiate(prefab);
            note.transform.SetParent(notesParent);
            note.transform.localEulerAngles = Vector3.zero;

            note.transform.localScale = ordinalScale;

            var noteScript = note.GetComponent<Note>();

            noteScript.inUse = true;
            runtimeNote.Add(noteScript);
            return note;
        }

        public void AttachNote(GameObject noteInstance)
        {
            var ordinalScale = noteInstance.transform.localScale;

            noteInstance.transform.SetParent(notesParent);
            noteInstance.transform.localEulerAngles = Vector3.zero;

            noteInstance.transform.localScale = ordinalScale;

            var note = noteInstance.GetComponent<Note>();
            note.parentTrack = this;
            runtimeNote.Add(note);
        }

        // ----------------------------------------

        // Trap

        public GameObject CreateTrap(GameObject prefab)
        {
            var ordinalScale = prefab.transform.localScale;

            var trap = Instantiate(prefab);
            trap.transform.SetParent(trapsParent);
            trap.transform.localEulerAngles = Vector3.zero;

            trap.transform.localScale = ordinalScale;

            var trapScript = trap.GetComponent<Trap>();

            trapScript.inUse = true;
            runtimeTrap.Add(trapScript);
            return trap;
        }

        public void AttachTrap(GameObject trapInstance)
        {
            var ordinalScale = trapInstance.transform.localScale;

            trapInstance.transform.SetParent(trapsParent);
            trapInstance.transform.localEulerAngles = Vector3.zero;

            trapInstance.transform.localScale = ordinalScale;

            var trap = trapInstance.GetComponent<Trap>();
            trap.parentTrack = this;
            runtimeTrap.Add(trap);
        }

        // ----------------------------------------

        public void DestoryAllNotes()
        {
            runtimeNote.Clear();

            foreach (Transform child in notesParent)
                GameObject.Destroy(child.gameObject);

            foreach (Transform child in trapsParent)
                GameObject.Destroy(child.gameObject);
        }

        public void RecycleAllNotes(TrackManager manager)
        {
            runtimeNote.Clear();
            runtimeTrap.Clear();

            var currentNotes = new List<Transform>();
            foreach (Transform child in notesParent)
            {
                currentNotes.Add(child);
            }
            currentNotes.ForEach(x =>
            {
                manager.ResetNoteToPool(x.gameObject);
            });

            var currentTraps = new List<Transform>();
            foreach (Transform child in trapsParent)
            {
                currentTraps.Add(child);
            }
            currentTraps.ForEach(x =>
            {
                manager.ResetNoteToPool(x.gameObject);
            });
        }

        public void ResetTrack()
        {
            ResetTrackPosition();

            runtimeNote.Clear();
            runtimeTrap.Clear();

            noteArea.ResetNoteArea();
        }
    }
}