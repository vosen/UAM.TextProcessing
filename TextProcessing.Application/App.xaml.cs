using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Vosen.SQLFilter;
using System.IO;
using System.Collections.ObjectModel;
using System.Net;
using System.ComponentModel;
using System.Windows.Data;

namespace SQLFilter.FilterView.Test
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static char[] tabSeparator = new char[] { '\t' };

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string input = @"(Active IS TRUE OR Address = 127.0.0.1 OR OS = '' OR Users = 0)";
            var filter = new Filter<FakeServer>(input);
            var window = new MainWindow(filter);
            window.ServerList.ItemsSource = LoadRawData();
            window.Show();
        }

        private static IList<FakeServer> LoadRawData()
        {
            List<FakeServer> list = new List<FakeServer>();
            string rawdata = SQLFilter.FilterView.Test.Properties.Resources.rawdata;
            string[] lines = rawdata.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            return new ObservableCollection<FakeServer>(lines.Select(GetServer));
        }

        private static FakeServer GetServer(string line)
        {
            var fields = line.Split(tabSeparator);
            return new FakeServer { Owner = fields[0],
                                    City = fields[1],
                                    Address = new IPAddress(long.Parse(fields[2])),
                                    OS = (OperatingSystem)int.Parse(fields[3]),
                                    Users = int.Parse(fields[4]),
                                    Active = bool.Parse(fields[5])
            };
        }
    }
}
