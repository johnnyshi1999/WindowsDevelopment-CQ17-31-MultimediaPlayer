﻿using System;
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

        public BindingList<Playlist> GetCollection()
        {
            LoadCollection();
            BindingList<Playlist> result = new BindingList<Playlist>(collection);
            return result;
        }

        private void LoadCollection()
        {
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
                Track track = new Track(reader.ReadLine());
                result.trackList.Add(track);
            }

            return result;

        }
    }
}