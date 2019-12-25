using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsDevelopment_CQ17_31_MultimediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
     class Album
    {
        public string Name { get; set; }
        public string Length { get; set; }
    }
    public partial class MainWindow : Window
    {
        BindingList<Album> list;
        public MainWindow()
        {
            InitializeComponent();
            Album album = new Album()
            {
                Name = "hello",
                Length="3:45"
            };
            list = new BindingList<Album>();
            list.Add(album);
            PlayListListView.ItemsSource = list;

        }
    }
}