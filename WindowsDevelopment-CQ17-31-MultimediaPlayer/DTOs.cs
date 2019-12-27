using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindowsDevelopment_CQ17_31_MultimediaPlayer
{
    public class Track
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
        private TagLib.File fileForTrackingProperties;

        public string Name {
            get {
                if (trackFile == null) return "N/A";
                string trueName = fileForTrackingProperties.Tag.Title == null ? 
                       Path.GetFileNameWithoutExtension(trackFile.FullName)
                    : fileForTrackingProperties.Tag.Title;
                return trueName;
            }
        }
        public string Length
        {
            get
            {
                if (fileForTrackingProperties == null) return "N/A";
                return fileForTrackingProperties.Properties.Duration.ToString();
            }
        }
        public TimeSpan position { set; get; }
        public string FilePath
        {
            get
            {
                return trackFile.FullName;
            }
        }

        public Track(string trackPath)
        {
            if (File.Exists(trackPath)) {
                trackFile = new FileInfo(trackPath);
                fileForTrackingProperties = TagLib.File.Create(trackPath);
            }
        }
    }

    //Chế độ chơi playlist
    public enum PLAY_MODE
    {
        SEQUENTIAL,
        RANDOM
    }

    //Chế độ lặp playlist
    public enum LOOP_MODE
    {
        ONE_TIME,
        INFINITE
    }

    public class Playlist
    {
        public string playlistName { get; set; }
        public BindingList<Track> trackList;
        public PLAY_MODE playMode { get; set; }
        public LOOP_MODE loopMode { get; set; }
        public int currentTrackIdx = -1;

        public Playlist(string name)
        {
            playlistName = name;
            trackList = new BindingList<Track>();
            playMode = PLAY_MODE.SEQUENTIAL;
            loopMode = LOOP_MODE.ONE_TIME;
        }

        private void shufflePlaylist()
        {
            Random random = new Random();
            BindingList<Track> result = new BindingList<Track>();
            while (trackList.Count != 0)
            {
                int index = random.Next(trackList.Count);
                result.Add(trackList[index]);
                trackList.RemoveAt(index);
            }
            trackList = result;
        }
    }
}
