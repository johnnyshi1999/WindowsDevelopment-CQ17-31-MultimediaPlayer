using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsDevelopment_CQ17_31_MultimediaPlayer
{
    class Track
    {
        private FileInfo trackFile;
        public string FilePath
        {
            get
            {
                string path = trackFile.FullName;
                return path;
            }
        }
        public TimeSpan position { set; get; }
        public TimeSpan naturalDuration { set; get; }

        public Track(string trackPath)
        {
            if (File.Exists(trackPath))
                trackFile = new FileInfo(trackPath);
        }
    }

    //Chế độ chơi playlist
    enum PLAY_MODE
    {
        SEQUENTIAL,
        RANDOM
    }

    //Chế độ lặp playlist
    enum LOOP_MODE
    {
        ONE_TIME,
        INFINITE
    }

    class Playlist
    {
        public string playlistName { get; set; }
        public List<Track> trackList;
        public PLAY_MODE playMode { get; set; }
        public LOOP_MODE loopMode { get; set; }

        public Playlist(string name)
        {
            playlistName = name;
            trackList = new List<Track>();
            playMode = PLAY_MODE.SEQUENTIAL;
            loopMode = LOOP_MODE.ONE_TIME;
        }
    }
}
