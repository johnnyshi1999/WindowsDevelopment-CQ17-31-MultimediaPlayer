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
        public event EventHandler TrackEnded;
        private static MusicBox instance = null;
        private MediaPlayer myMusicPlayer;

        private MusicBox()
        {
            myMusicPlayer = new MediaPlayer();
            myMusicPlayer.MediaOpened += MyMusicPlayer_MediaOpened;
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
                    myMusicPlayer.Play();
                else
                    myMusicPlayer.Open(new Uri(trackPath, UriKind.Absolute));
            }
            else
                myMusicPlayer.Open(new Uri(trackPath, UriKind.Absolute));
            _timer.Start();
        }

        public void pauseTrack()
        {
            myMusicPlayer.Pause();
        }

        public void stopTrack()
        {
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
