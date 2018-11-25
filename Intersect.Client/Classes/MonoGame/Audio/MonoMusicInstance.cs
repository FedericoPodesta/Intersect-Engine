﻿using System;
using Intersect.Client.Framework.Audio;
using Intersect.Client.General;
using Microsoft.Xna.Framework.Media;

namespace Intersect.Client.MonoGame.Audio
{
    public class MonoMusicInstance : GameAudioInstance
    {
        private bool mDisposed;
        private Song mSong;
        private int mVolume;

        public MonoMusicInstance(GameAudioSource music) : base(music)
        {
            mSong = ((MonoMusicSource) music).GetSource();
        }

        public override void Play()
        {
            MediaPlayer.Play(mSong);
        }

        public override void Pause()
        {
            MediaPlayer.Pause();
        }

        public override void Stop()
        {
            MediaPlayer.Stop();
        }

        public override void SetVolume(int volume, bool isMusic = false)
        {
            mVolume = volume;
            try
            {
                MediaPlayer.Volume = (mVolume * (float) (Globals.Database.MusicVolume / 100f) / 100f);
            }
            catch (NullReferenceException)
            {
                // song changed while changing volume
            }
            catch (Exception)
            {
                // device not ready
            }
        }

        public override int GetVolume()
        {
            return mVolume;
        }

        public override void SetLoop(bool val)
        {
            MediaPlayer.IsRepeating = val;
        }

        public override AudioInstanceState GetState()
        {
            if (mDisposed) return AudioInstanceState.Disposed;
            if (MediaPlayer.State == MediaState.Playing) return AudioInstanceState.Playing;
            if (MediaPlayer.State == MediaState.Stopped) return AudioInstanceState.Stopped;
            if (MediaPlayer.State == MediaState.Paused) return AudioInstanceState.Paused;
            return AudioInstanceState.Disposed;
        }

        public override void Dispose()
        {
            mDisposed = true;
            MediaPlayer.Stop();
        }
    }
}