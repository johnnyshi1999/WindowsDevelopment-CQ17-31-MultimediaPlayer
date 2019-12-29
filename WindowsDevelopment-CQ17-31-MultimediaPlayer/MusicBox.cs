using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace WindowsDevelopment_CQ17_31_MultimediaPlayer
{
    class MusicBox
    {
        private static MusicBox instance = null;
        private MediaPlayer myMusicPlayer;
        public bool isPlaying { get; private set; }

        //Spaghetti coding variable here
        private class timingObject
        {
            public DispatcherTimer myTimer;
            public TimeSpan? pos;
        };
        private timingObject myTimingObj;

        public delegate void InvokeUIUpdateMediaOpened();
        public event InvokeUIUpdateMediaOpened MediaOpenedUIUpdates;

        private MusicBox()
        {
            myMusicPlayer = new MediaPlayer();
            isPlaying = false;
            myTimingObj = new timingObject();
        }

        public void SetTrackEndedEvent(EventHandler TrackEndedEvent)
        {
            myMusicPlayer.MediaEnded += TrackEndedEvent;
        }

        public void SetMediaOpenedUIUpdate(EventHandler UIUpdate)
        {
            myMusicPlayer.MediaOpened += UIUpdate;
            myMusicPlayer.MediaOpened += MyMusicPlayer_MediaOpened;
        }

        private void MyMusicPlayer_MediaOpened(object sender, EventArgs e)
        {
            myTimingObj.myTimer?.Start();
            if (myTimingObj.pos != null)
            {
                myMusicPlayer.Position = myTimingObj.pos.Value;
            }
            
            myMusicPlayer.Play();
        }

        public static MusicBox getInstance()
        {
            if (instance == null)
                instance = new MusicBox();
            return instance;
        }

        public void playTrack(string trackPath, DispatcherTimer _timer, TimeSpan? position)
        {
            if (myMusicPlayer.Source != null)
            {
                if (trackPath.Equals(myMusicPlayer.Source.LocalPath))
                {
                    if (position != null) myMusicPlayer.Position = position.Value;
                    myMusicPlayer.Play();
                    _timer.Start();
                }
                else
                {
                    myTimingObj.pos = position;
                    myTimingObj.myTimer = _timer;
                    myMusicPlayer.Open(new Uri(trackPath, UriKind.Absolute));
                    
                }
            }
            else
            {
                myTimingObj.pos = position;
                myTimingObj.myTimer = _timer;
                myMusicPlayer.Open(new Uri(trackPath, UriKind.Absolute));
            }
            isPlaying = true;
        }

        public void pauseTrack()
        {
            isPlaying = false;
            myMusicPlayer.Pause();
        }

        public void stopTrack()
        {
            isPlaying = false;
            myMusicPlayer.Stop();
        }

        public TimeSpan? getCurrentPosition()
        {
            if (myMusicPlayer.Source != null)
            {
                return myMusicPlayer.Position;
            }
            return null;
        }

        public string getDuration()
        {
            if (myMusicPlayer.NaturalDuration.HasTimeSpan)
            {
                return myMusicPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            }
            return null;
        }

        public bool isSourceNull()
        {
            return (myMusicPlayer.Source == null);
        }

    }
}
