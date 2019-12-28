using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WindowsDevelopment_CQ17_31_MultimediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        //Playlist
        Playlist currentPlaylist;

        Model model;

        //Threads
        BackgroundWorker AddTracks;

        DispatcherTimer _timer;
      
        MusicBox MusicBox;

        //List for saving data of random play mode
        List<int> listIndexForRandomPlayMode;
        int currentIndexOfRandomPlayMode;
        List<int> listIndexOfCurrentPlaylist;

        public MainWindow()
        {
            InitializeComponent();

            // Set up timer 
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += timer_Tick;

        }

        //-------------------------Threads-------------------------

        private void AddTracks_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] trackPaths = (string[])e.Argument;
           
            int playlistCount = currentPlaylist.trackList.Count;
            for (int i = 0; i < trackPaths.Length; i++)
            {
                bool trackDuplicate = false;
                
                for (int j = 0; j < playlistCount; j++)
                {
                    if (trackPaths[i].Equals(currentPlaylist.trackList[j].FilePath))
                    {
                        trackDuplicate = true;
                        break;
                    }
                }
                if (!trackDuplicate) AddTracks.ReportProgress(0, new Track(trackPaths[i]));

            }
        }

        private void AddTracks_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Track track = (Track)e.UserState;
            currentPlaylist.trackList.Add(track);
        }

        private void AddTracks_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        //--------------------------Events--------------------------

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //Stuffz threadz
            AddTracks = new BackgroundWorker() {
               WorkerReportsProgress = true,
            };
            AddTracks.DoWork += AddTracks_DoWork;
            AddTracks.ProgressChanged += AddTracks_ProgressChanged;
            AddTracks.RunWorkerCompleted += AddTracks_RunWorkerCompleted;

            //Set stuffz
            TrackNameTextBlock.Measure(new Size(double.PositiveInfinity,
                                            double.PositiveInfinity));
            var Width = TrackNameTextBlock.DesiredSize.Width;
            Canvas.SetRight(TrackNameTextBlock, (TrackNameWrapper.ActualWidth - Width) / 2);
            model = Model.GetInstance();
            MusicBox = MusicBox.getInstance();
            MusicBox.SetTrackEndedEvent(LoopMode_trackEndEventHandler);
        }

        private void AddTrackButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
            var fileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "MP3 Files(*.mp3;*.MP3)|*.mp3;*.MP3"
            };

            if (fileDialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                AddTracks.RunWorkerAsync(fileDialog.FileNames);
            }
        }

        private void RemoveTrackButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
            var selected = PlayListListView.SelectedItems.Cast<Track>().ToList();
            int amount = selected.Count;
            Mouse.OverrideCursor = Cursors.Wait;
            for (int i = 0; i < amount; i++)
                currentPlaylist.trackList.Remove(selected[i]);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void PlaylistTrack_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (currentPlaylist == null) return;
            var item = ((FrameworkElement)e.OriginalSource).DataContext as Track;
            if (item != null)
            {
                MarqueeTrackName(item.Name);
                if (item.FilePath == null)
                {
                    MusicBox.getInstance().stopTrack();
                    TimeTextBlock.Text = "00:00 | 00:00";
                    return;
                }
                currentPlaylist.currentTrackIdx = PlayListListView.SelectedIndex;
                MusicBox.getInstance().playTrack(item.FilePath, _timer);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;

            //if more than 1 track is selected
            if (PlayListListView.SelectedItems.Count > 1)
            {
                MessageBox.Show("Please choose one and only one song");
                return;
            }

            //if no track is selected 
            if (PlayListListView.SelectedItems.Count == 0)
            {
                if (currentPlaylist.playMode == PLAY_MODE.RANDOM)
                {
                    PlayListListView.SelectedIndex = listIndexForRandomPlayMode[currentIndexOfRandomPlayMode];
                    currentIndexOfRandomPlayMode++;
                }
                else
                    //play the first track
                    PlayListListView.SelectedIndex = 0;
           
            }
            var item = PlayListListView.SelectedItem as Track;
            MarqueeTrackName(item.Name);
            if (item.FilePath == null)
            {
                MusicBox.getInstance().stopTrack();
                TimeTextBlock.Text = "00:00 | 00:00";
                return;
            }
            currentPlaylist.currentTrackIdx = PlayListListView.SelectedIndex;
            MusicBox.getInstance().playTrack(item.FilePath, _timer);

          

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
            MusicBox.getInstance().stopTrack();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
            MusicBox.getInstance().pauseTrack();
        }

        private void TrackUp_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
        }

        private void TrackDown_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
        }

        //New playlist dialog
        private void NewPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var newPlaylist = new NewPlaylistDialog();
            newPlaylist.NewPlaylistEvent += NewPlaylist_NewPlaylistEvent;
            newPlaylist.Show();
        }

        private void NewPlaylist_NewPlaylistEvent(Playlist playlist)
        {
            currentPlaylist = playlist;
            PlayListListView.ItemsSource = currentPlaylist.trackList;
            PlayListNameTextBlock.Text = currentPlaylist.playlistName;
            if (LoopButton.Tag.ToString() == "On")
            {
                currentPlaylist.loopMode = LOOP_MODE.INFINITE;
            }
        }

        //-----------------------Animations-----------------------
        private void MarqueeTrackName(string trackName)
        {
            TrackNameTextBlock.Text = trackName;
            TrackNameTextBlock.Measure(new Size(double.PositiveInfinity,
                                            double.PositiveInfinity));
            var Width = TrackNameTextBlock.DesiredSize.Width;
            double height = TrackNameWrapper.ActualHeight - TrackNameTextBlock.ActualHeight;
            TrackNameTextBlock.Margin = new Thickness(0, height / 2, 0, 0);
            
            Storyboard marquee = new Storyboard();
            //Marquee
            DoubleAnimation marqueeAnimation = new DoubleAnimation
            {
                From = -Width,
                To = TrackNameWrapper.ActualWidth,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.FromSeconds(10)),
            };
            Storyboard.SetTarget(marqueeAnimation, TrackNameTextBlock);
            Storyboard.SetTargetProperty(marqueeAnimation, new PropertyPath("(Canvas.Right)"));
            marquee.Children.Add(marqueeAnimation);

            //Apply
            marquee.Begin(this);

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!MusicBox.getInstance().isPlaying) return;
            var currentPos = MusicBox.getInstance().getCurrentPosition();
            var duration = MusicBox.getInstance().getDuration();
            TimeTextBlock.Text = $"{currentPos} | {duration}";    
        }

        private void SavePlayListButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null)
            {
                MessageBox.Show("There are no playlist");
                return;
            }
            model.SavePlayList(currentPlaylist);
        }

        private void LoadPlayListButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPlayListDialog dialog = new LoadPlayListDialog();
            dialog.NewPlaylistEvent += NewPlaylist_NewPlaylistEvent;
            dialog.ShowDialog();
        }

        private void LoopButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button.Tag.ToString() == "Off")
            {
                button.Tag = "On";
                if (currentPlaylist != null)
                {
                    currentPlaylist.loopMode = LOOP_MODE.INFINITE;
                }
            }
            else
            {
                button.Tag = "Off";
                if (currentPlaylist != null)
                {
                    currentPlaylist.loopMode = LOOP_MODE.ONE_TIME;
                }
            }
        }

        private void LoopMode_trackEndEventHandler(object sender, EventArgs e)
        {
            if (currentPlaylist.playMode == PLAY_MODE.RANDOM) // PLAYMODE RANDOM
            {
                if (currentIndexOfRandomPlayMode < listIndexForRandomPlayMode.Count)
                {
                    PlayListListView.SelectedIndex = listIndexForRandomPlayMode[currentIndexOfRandomPlayMode];
                    currentIndexOfRandomPlayMode++;
                }
                else
                {
                    if (currentPlaylist.loopMode == LOOP_MODE.INFINITE)
                    {
                        generateRanDomList();
                        PlayListListView.SelectedIndex = listIndexForRandomPlayMode[currentIndexOfRandomPlayMode];
                    }
                }
               
            }
            else
            {
                if (PlayListListView.SelectedIndex < PlayListListView.Items.Count - 1)
                {

                    PlayListListView.SelectedIndex++;
                }
                else
                {
                    //back to the start
                    if (currentPlaylist.loopMode == LOOP_MODE.INFINITE)
                    {
                        PlayListListView.SelectedIndex = 0;
                    }
                }
            }

            PlayButton_Click(PlayButton, null);

        
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null)
            {
                return;
            }

            if (PlayListListView.SelectedIndex < PlayListListView.Items.Count - 1)
            {
                PlayListListView.SelectedIndex++;
            }
            else
            {
                PlayListListView.SelectedIndex = 0;
            }

            PlayButton_Click(PlayButton, null);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null)
            {
                return;
            }

            if (PlayListListView.SelectedIndex > 0)
            {
                PlayListListView.SelectedIndex--;
            }
            else
            {
                //if this is already the first track
                StopButton_Click(StopButton, null);
            }
            PlayButton_Click(PlayButton, null);
        }

        private void ShuflleButton_Click(object sender, RoutedEventArgs e)
        {

            var button = sender as Button;
            if (button.Tag.ToString() == "Off")
            {
                if (currentPlaylist != null)
                {
                    button.Tag = "On";

                 
                    listIndexOfCurrentPlaylist = new List<int>(Enumerable.Range(0, currentPlaylist.TrackCount));
                   
                    if (PlayListListView.SelectedIndex != -1)
                    {
                        // Trong lúc chương trình đang chạy 1 bài hát thì random list sẽ dc tạo ra mà không có bài hát đang chạy
                        // Loại bỏ bài hát đang chạy
                        listIndexOfCurrentPlaylist.RemoveAt(PlayListListView.SelectedIndex);
                        
                    }
                    else 
                    {
                        // Nếu bấm shuffle button mà trong lúc chương trình đang rảnh sẽ tạo ra random list bao gồm tất cả bài hát trong play list
                    }
                    // Generate random list index
                    generateRanDomList();
                    currentPlaylist.playMode = PLAY_MODE.RANDOM;
                  
                }
               
            }
            else
            {
                button.Tag = "Off";
                if (currentPlaylist != null)
                {
                    currentPlaylist.playMode = PLAY_MODE.SEQUENTIAL;
                }
            }

      
        }
        private void generateRanDomList()
        {
            // Generate random list index
            listIndexForRandomPlayMode = new List<int>();
            currentIndexOfRandomPlayMode = 0;
            Random rnd = new Random();
            while (listIndexOfCurrentPlaylist.Count != 0)
            {
                int index = rnd.Next(listIndexOfCurrentPlaylist.Count);
                listIndexForRandomPlayMode.Add(listIndexOfCurrentPlaylist[index]);
                listIndexOfCurrentPlaylist.RemoveAt(index);
            }
            currentPlaylist.playMode = PLAY_MODE.RANDOM;
            for (int i = 0; i < listIndexForRandomPlayMode.Count; i++)
                Console.Write("" + listIndexForRandomPlayMode[i] + ' ');
        }
    }
}