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
using System.Windows.Shapes;

namespace travwpf
{
    /// <summary>
    /// Логика взаимодействия для Prox.xaml
    /// </summary>
    public partial class Prox : Window
    {
        public Prox()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Proxy = IP.Text;
            Properties.Settings.Default.Save();
            Proxy.Set(new System.Net.WebProxy(IP.Text.Split(':')[0], Convert.ToInt32(IP.Text.Split(':')[1])));
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(Properties.Settings.Default.Proxy != "")
            {
                IP.Text = Properties.Settings.Default.Proxy;
            }
        }
    }
}
