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
            
        }

        public static MusicBox getInstance()
        {
            if (instance == null)
                instance = new MusicBox();
            return instance;
        }
    }
}
