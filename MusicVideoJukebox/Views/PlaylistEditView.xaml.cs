﻿using MusicVideoJukebox.Core.ViewModels;
using System;
using System.Collections.Generic;
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

namespace MusicVideoJukebox.Views
{
    /// <summary>
    /// Interaction logic for PlaylistsView.xaml
    /// </summary>
    public partial class PlaylistEditView : UserControl
    {
        public PlaylistEditView()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                // Deselect items that were removed
                foreach (AvailableTrackViewModel item in e.RemovedItems)
                {
                    item.IsSelected = false;
                }

                // Select items that were added
                foreach (AvailableTrackViewModel item in e.AddedItems)
                {
                    item.IsSelected = true;
                }
            }
        }
    }
}
