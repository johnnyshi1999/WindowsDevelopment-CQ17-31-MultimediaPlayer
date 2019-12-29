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
        
        private TagLib.File fileForTrackingProperties;
        private readonly string _trackName;
        private readonly string _trackLength;

        public string Name {
            get {
                return _trackName;
            }
        }
        public string Length
        {
            get
            {
                return _trackLength;
            }
        }
        public TimeSpan position { set; get; }
        public string FilePath
        {
            get
            {
                if (trackFile == null || !File.Exists(trackFile.FullName)) return null;
                return trackFile.FullName;
            }
        }

        public Track(string trackPath)
        {
            if (File.Exists(trackPath)) {
                trackFile = new FileInfo(trackPath);
                fileForTrackingProperties = TagLib.File.Create(trackPath);
                _trackName = fileForTrackingProperties.Tag.Title == null ?
                      Path.GetFileNameWithoutExtension(trackFile.FullName)
                   : fileForTrackingProperties.Tag.Title;
                _trackLength = fileForTrackingProperties.Properties.Duration.ToString();
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
        public int TrackCount
        {
            get
            {
                if (trackList != null)
                {
                    return trackList.Count();
                }
                return 0;
            }
        }
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

        //private void shufflePlaylist()
        //{
        //    Random random = new Random();
        //    BindingList<Track> result = new BindingList<Track>();
        //    while (trackList.Count != 0)
        //    {
        //        int index = random.Next(trackList.Count);
        //        result.Add(trackList[index]);
        //        trackList.RemoveAt(index);
        //    }
        //    trackList = result;
        //}
    }

}
