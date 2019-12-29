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

        private MusicBox()
        {
            myMusicPlayer = new MediaPlayer();
            myMusicPlayer.MediaOpened += MyMusicPlayer_MediaOpened;
            isPlaying = false;
        }

        public void SetTrackEndedEvent(EventHandler TrackEndedEvent)
        {
            myMusicPlayer.MediaEnded += TrackEndedEvent;
        }

        private void MyMusicPlayer_MediaOpened(object sender, EventArgs e)
        {
            myMusicPlayer.Play();
        }

        public static MusicBox getInstance()
        {
            if (instance == null)
                instance = new MusicBox();
            return instance;
        }

        public void playTrack(string trackPath, DispatcherTimer _timer)
        {
            if (myMusicPlayer.Source != null)
            {
                if(trackPath.Equals(myMusicPlayer.Source.LocalPath))
                {

                    myMusicPlayer.Play();
                }
                else
                    myMusicPlayer.Open(new Uri(trackPath, UriKind.Absolute));
            }
            else
                myMusicPlayer.Open(new Uri(trackPath, UriKind.Absolute));
            isPlaying = true;
            _timer.Start();
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

        public string getCurrentPosition()
        {
            if(myMusicPlayer.Source != null ) {
                return myMusicPlayer.Position.ToString(@"mm\:ss");
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

        public Boolean isSourceNull()
        {
            return (myMusicPlayer.Source == null);
        }

    }
}
