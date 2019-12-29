using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsDevelopment_CQ17_31_MultimediaPlayer
{
    class Model
    {
        private static Model model = null;
        public List<Playlist> collection;

        string getFileName(string PlayListName)
        {
            string result = BitConverter.ToString(Encoding.ASCII.GetBytes(PlayListName));
            return result.Replace("-", "");
        }

        private Model()
        {
            collection = new List<Playlist>();
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\data"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\data");
            }
        }

        public static Model GetInstance()
        {
            if (model == null)
            {
                model = new Model();
            }
            return model;
        }

        public void SavePlayList(Playlist playlist)
        {
            string fileName = getFileName(playlist.playlistName);

            string path = $"data\\{fileName}.awe";
            var writer = new StreamWriter(path);

            StringBuilder fileContentString = new StringBuilder();

            //playlist name
            fileContentString.Append(playlist.playlistName);
            fileContentString.Append("\n");

            //number of tracks;
            fileContentString.Append(playlist.TrackCount);
            fileContentString.Append("\n");

            //save file paths of the tracks
            for (int i = 0; i < playlist.TrackCount; i++)
            {
                fileContentString.Append(playlist.TrackList[i].FilePath);
                fileContentString.Append(" - ");
                double time = playlist.TrackList[i].position == null ? 0 : playlist.TrackList[i].position.Value.TotalMilliseconds;
                fileContentString.Append(time);
                fileContentString.Append("\n");
            }

            fileContentString.Append(playlist.currentTrackIdx);
            fileContentString.Append("\n");
           
            writer.Write(fileContentString.ToString());

            writer.Close();

            System.Windows.MessageBox.Show("playlist is saved");

        }

        public BindingList<Playlist> GetCollection()
        {
            LoadCollection();
            BindingList<Playlist> result = new BindingList<Playlist>(collection);
            return result;
        }

        private void LoadCollection()
        {
            collection.Clear();
            DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\data");

            var fileList = di.GetFiles();
            for (int i = 0; i < fileList.Length; i++)
            {
                Playlist playlist = LoadPlayList(fileList[i].FullName);
                collection.Add(playlist);
            }
        }

        private Playlist LoadPlayList(string path)
        {
            Playlist result;

            if (Path.GetExtension(path) != ".awe")
            {
                return null;
            }

            var reader = new StreamReader(path);

            //read playlist name
            var playlistName = reader.ReadLine();
            result = new Playlist(playlistName);

            //read number of tracks
            int trackCount = int.Parse(reader.ReadLine());

            for (int i = 0; i < trackCount; i++)
            {
                string line = reader.ReadLine();
                var tokens = line.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                Track track = new Track(tokens[0]);
                track.position = TimeSpan.FromMilliseconds(double.Parse(tokens[1]));
                result.addTrack(track);
            }
          
            int currentTrackIdx = -1;
            if (!Int32.TryParse(reader.ReadLine(), out currentTrackIdx))
            {
                currentTrackIdx = -1;
            }
            result.currentTrackIdx = currentTrackIdx;
            
            reader.Close();

            return result;

        }
    }
}
