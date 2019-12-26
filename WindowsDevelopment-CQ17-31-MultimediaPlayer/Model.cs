using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsDevelopment_CQ17_31_MultimediaPlayer
{
    class Model
    {
        public List<Playlist> collection;

        string getFileName(string PlayListName)
        {
            string result = BitConverter.ToString(Encoding.ASCII.GetBytes(PlayListName));
            return result.Replace("-", "");
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
            fileContentString.Append(playlist.trackList.Count);
            fileContentString.Append("\n");

            //save file paths of the tracks
            for (int i = 0; i < playlist.trackList.Count; i++)
            {
                fileContentString.Append(playlist.trackList[i].FilePath);
                fileContentString.Append("\n");
            }

            writer.Write(fileContentString.ToString());

            writer.Close();

            System.Windows.MessageBox.Show("playlist is saved");

        }

        public Playlist LoadPlayList(string path)
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
                Track track = new Track(reader.ReadLine());
                result.trackList.Add(track);
            }

            return result;

        }
    }
}
