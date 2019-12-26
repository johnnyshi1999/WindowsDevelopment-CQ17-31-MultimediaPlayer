using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WindowsDevelopment_CQ17_31_MultimediaPlayer
{
    class MusicBox
    {
        private static MusicBox instance = null;
        private MediaPlayer myMusicPlayer;

        private MusicBox()
        {
            myMusicPlayer = new MediaPlayer();
            myMusicPlayer.MediaOpened += MyMusicPlayer_MediaOpened;
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

        public void playTrack(string trackPath)
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
        }

        public void pauseTrack()
        {
            myMusicPlayer.Pause();
        }

        public void stopTrack()
        {
            myMusicPlayer.Stop();
        }
    }
}
