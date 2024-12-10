﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RhythmGameStarter
{
    [HelpURL("https://bennykok.gitbook.io/rhythm-game-starter/hierarchy-overview/song-manager")]
    [RequireComponent(typeof(TrackManager))]
    public class SongManager : MonoBehaviour
    {
        [Comment("Responsible for song control, handling song related events.")]
        public AudioSource audioSource;

        [Header(" Elements ")]
        public CharacterAction characterAction;
        public Transform endNotePos;

        [Header(" UI ")]
        public Image songProgressBar;

        [Title("Properties", 0)]
        [Space]
        public bool playAuto;
        public bool playOnAwake = true;
        public SongItem defaultSong;
        public SongItem changeSong;
        public float delay;
        public bool looping;

        [Tooltip("Automatically handle play/pause when timescale set to 0, or back to 1")]
        public bool autoTimeScalePause;

        [Title("Display", 0)]
        public bool progressAsPercentage = true;
        public bool inverseProgressFill = false;

        [HideInInspector] public float secPerBeat;
        [HideInInspector] public float songPosition;
        [HideInInspector] public IEnumerable<SongItem.MidiNote> currnetNotes;

        [Title("Events", 0)]
        [CollapsedEvent("Triggered every frame when a song progress")] public FloatEvent onSongProgress;
        [CollapsedEvent("Triggered every frame when a song progress with a float between [0,1] useful for Image fill")] public FloatEvent onSongProgressFill;
        [CollapsedEvent("Triggered every frame the song progress with a string variable of the progress display")] public StringEvent onSongProgressDisplay;
        [CollapsedEvent("Triggered when the song play, after the delay wait time")] public UnityEvent onSongStart;
        [CollapsedEvent("Triggered when the song play, before the delay wait time")] public UnityEvent onSongStartPlay;
        [CollapsedEvent("Triggered when the song finished")] public UnityEvent onSongFinished;

        #region RUNTIME_FIELD

        [NonSerialized] public bool songPaused;
        [NonSerialized] public SongItem currentSongItem;
        [NonSerialized] public ComboSystem comboSystem;
        [NonSerialized] public TrackManager trackManager;

        private bool songHasStarted;
        private bool songStartEventInvoked;
        private double dspStartTime;
        private double dspPausedTime;
        private double accumulatedPauseTime;

        #endregion

        private void Awake()
        {
            trackManager = GetComponent<TrackManager>();
            comboSystem = GetComponent<ComboSystem>();

            trackManager.Init(this);
        }

        private void Start()
        {
            playAuto = false;

            if (playOnAwake && defaultSong)
            {
                PlaySong(defaultSong);
            }
        }

        public void PlaySong()
        {
            if (defaultSong)
                PlaySong(defaultSong);
            else
                Debug.LogWarning("Default song is not set!");
        }

        public void PlaySongSelected(SongItem songItem)
        {
            PlaySong(songItem);
        }

        public void SetDefaultSong(SongItem songItem)
        {
            defaultSong = songItem;
        }

        public void PlaySong(SongItem songItem, double specificStartTime = 0)
        {
            currentSongItem = songItem;
            songPaused = false;
            songHasStarted = true;
            accumulatedPauseTime = 0;
            dspPausedTime = 0;
            songPosition = -1;

            if (audioSource) audioSource.clip = songItem.clip;
            if (songItem.customMidiMapping || songItem.customPrefabMapping)
                trackManager.OverrideMapping(songItem.customMidiMapping, songItem.customPrefabMapping);
            else
                trackManager.ResetMappingOverride();

            // songItem.ResetNotesState();
            currnetNotes = songItem.GetNotes();
            secPerBeat = 60f / songItem.bpm;

            //Starting the audio play back
            dspStartTime = AudioSettings.dspTime;
            if (audioSource)
            {
                audioSource.pitch = songItem.speedModifier;

                audioSource.PlayScheduled(AudioSettings.dspTime + delay);
                audioSource.time = (float)specificStartTime;
                dspStartTime -= specificStartTime;
            }

            trackManager.SetupForNewSong();

            onSongStartPlay.Invoke();
        }

        public void PauseSong()
        {
            if (songPaused) return;

            songPaused = true;
            if (audioSource) audioSource.Pause();

            dspPausedTime = AudioSettings.dspTime;
        }

        public void ResumeSong()
        {
            if (!songHasStarted)
            {
                PlaySong();
                return;
            }
            if (!songPaused) return;

            songPaused = false;
            if (audioSource) audioSource.Play();

            accumulatedPauseTime += AudioSettings.dspTime - dspPausedTime;
        }

        public void StopSong(bool dontInvokeEvent = false)
        {
            if (audioSource) audioSource.Stop();
            songHasStarted = false;
            songStartEventInvoked = false;

            if (!dontInvokeEvent)
                onSongFinished.Invoke();

            trackManager.ClearAllTracks();
        }

        void Update()
        {
            if (!songStartEventInvoked && songHasStarted && songPosition >= 0)
            {
                songStartEventInvoked = true;
                onSongStart.Invoke();
            }

            // If we need to automatically handle play/pause according to the timescale;
            if (songHasStarted && songStartEventInvoked && autoTimeScalePause)
            {
                if (!songPaused && Time.timeScale == 0)
                {
                    PauseSong();
                }
                else if (songPaused && Time.timeScale == 1)
                {
                    ResumeSong();
                }
            }

            //Sync the tracks position with the audio
            if (!songPaused && songHasStarted)
            {
                songPosition = (float)(AudioSettings.dspTime - dspStartTime - delay - accumulatedPauseTime) * audioSource.pitch;

                trackManager.UpdateTrack(songPosition, secPerBeat);

                onSongProgress.Invoke(songPosition);

                float progress = songPosition / currentSongItem.clip.length;

                // Update the progress bar fill amount
                if (songProgressBar != null)
                    songProgressBar.fillAmount = inverseProgressFill ? 1 - progress : progress;

                onSongProgressFill.Invoke(inverseProgressFill ? 1 - progress : progress);

                if (songPosition >= 0)
                {
                    if (progressAsPercentage)
                        onSongProgressDisplay.Invoke(Math.Truncate(songPosition / currentSongItem.clip.length * 100) + "%");
                    else
                    {
                        var now = new DateTime((long)songPosition * TimeSpan.TicksPerSecond);
                        onSongProgressDisplay.Invoke(now.ToString("mm:ss"));
                    }
                }
            }

            if (songHasStarted && currentSongItem.clip && songPosition >= currentSongItem.clip.length)
            {
                songHasStarted = false;
                songStartEventInvoked = false;
                onSongFinished.Invoke();

                trackManager.ClearAllTracks();

                //If its looping, we replay the current song
                if (looping)
                    PlaySong(currentSongItem);
            }
        }

        public void ChangeNoteSpeed()
        {
            if (changeSong == null)
            {
                Debug.LogWarning("No changeSong assigned!");
                return;
            }

            // Lấy vị trí hiện tại của bài hát (thời gian phát bài hát hiện tại)
            float currentSongPosition = audioSource.time;

            // Cập nhật thông tin bài hát mới
            currentSongItem = changeSong;
            secPerBeat = 60f / changeSong.bpm;
            currnetNotes = changeSong.GetNotes();

            // Tiếp tục phát bài hát mới từ vị trí hiện tại
            if (!audioSource.isPlaying)
            {
                audioSource.PlayScheduled(AudioSettings.dspTime);
                audioSource.time = currentSongPosition;
            }

            // Cập nhật trackManager để tạo note mới từ vị trí hiện tại
            trackManager.SetupForNewSong();

            // Đảm bảo các note tiếp tục sinh ra từ bên phải, theo hướng đi của bài hát
            Debug.Log("Changed to new song and resumed from current position.");
        }

    }
}