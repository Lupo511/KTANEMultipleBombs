using DarkTonic.MasterAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MultipleBombsAssembly
{
    public class GameplayMusicControllerManager : MonoBehaviour
    {
        private static FieldInfo isPlayingField;
        private static FieldInfo stingerResultField;
        private GameplayMusicController gameplayMusicController;
        private PlaylistController playlistController;
        private bool stingerPlayed;
        private int currentSongIndex;

        static GameplayMusicControllerManager()
        {
            isPlayingField = typeof(GameplayMusicController).GetField("isPlaying", BindingFlags.Instance | BindingFlags.NonPublic);
            stingerResultField = typeof(GameplayMusicController).GetField("stingerResult", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public void Awake()
        {
            gameplayMusicController = GetComponent<GameplayMusicController>();
            playlistController = GetComponent<PlaylistController>();
        }

        public void Start()
        {
            gameplayMusicController.enabled = false;
        }

        public void OnDestroy()
        {
            gameplayMusicController.enabled = true;
        }

        //To-do: check transitions when going backwards
        public void Update()
        {
            //Sets the current song index and plays the stinger considering the timer of all of the bombs
            if (gameplayMusicController.Settings.PlaylistName != string.Empty && SceneManager.Instance != null && SceneManager.Instance.GameplayState != null && (bool)isPlayingField.GetValue(gameplayMusicController))
            {
                float timeRemaining = float.MaxValue;
                float totalTime = float.MaxValue;
                bool stingerTime = false;
                foreach (Bomb bomb in SceneManager.Instance.GameplayState.Bombs)
                {
                    if (!bomb.IsSolved())
                    {
                        if (bomb.GetTimer().TimeRemaining < timeRemaining)
                            timeRemaining = bomb.GetTimer().TimeRemaining;

                        if (!stingerTime)
                            stingerTime = bomb.GetTimer().TimeRemaining < 30f + bomb.GetTimer().GetRate() * gameplayMusicController.Settings.StingerTime;
                    }

                    if (bomb.TotalTime < totalTime)
                        totalTime = bomb.TotalTime;
                }

                if (!string.IsNullOrEmpty(gameplayMusicController.Settings.StingerName) && stingerTime && !stingerPlayed)
                {
                    stingerResultField.SetValue(gameplayMusicController, MasterAudio.PlaySound3DAtTransform(gameplayMusicController.Settings.StingerName, transform, 1f, null, 0f, null, false, false));
                    stingerPlayed = true;
                }

                if (!stingerTime && stingerPlayed)
                {
                    PlaySoundResult stingerResult = (PlaySoundResult)stingerResultField.GetValue(gameplayMusicController);
                    if (stingerResult != null && stingerResult.ActingVariation != null)
                        stingerResult.ActingVariation.Stop();
                    stingerPlayed = false;
                }

                if (timeRemaining < 30f)
                {
                    if (currentSongIndex != playlistController.CurrentPlaylist.MusicSettings.Count - 1)
                    {
                        currentSongIndex = playlistController.CurrentPlaylist.MusicSettings.Count - 1;
                        playlistController.ClearQueue();
                        playlistController.PlaySong(playlistController.CurrentPlaylist.MusicSettings[currentSongIndex], PlaylistController.AudioPlayType.PlayNow);
                    }
                }
                else
                {
                    int newSongIndex;
                    float timeRatio = timeRemaining / totalTime;
                    if (gameplayMusicController.Settings.useCrossfade)
                    {
                        newSongIndex = Mathf.Clamp(playlistController.CurrentPlaylist.MusicSettings.Count - (int)(timeRatio * playlistController.CurrentPlaylist.MusicSettings.Count), 0, playlistController.CurrentPlaylist.MusicSettings.Count);
                    }
                    else
                    {
                        newSongIndex = Mathf.Clamp(playlistController.CurrentPlaylist.MusicSettings.Count - (int)(timeRatio * (playlistController.CurrentPlaylist.MusicSettings.Count - 1)), 0, playlistController.CurrentPlaylist.MusicSettings.Count - 2);
                    }
                    if (newSongIndex != currentSongIndex)
                    {
                        currentSongIndex = newSongIndex;
                        playlistController.ClearQueue();
                        playlistController.QueuePlaylistClip(playlistController.CurrentPlaylist.MusicSettings[currentSongIndex].songName, true);
                    }
                }
            }
        }
    }
}
