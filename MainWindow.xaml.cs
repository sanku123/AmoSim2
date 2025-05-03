using AmoSim2.Player;
using AmoSim2.ViewModel;
using CommonServiceLocator;
using Microsoft.Win32;
using System;
using System.Windows;

namespace AmoSim2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        public PlayerViewModel PlayerViewModel => ServiceLocator.Current.GetInstance<PlayerViewModel>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(PlayerViewModel.Player.Attack.ToString());
        }
    }
}