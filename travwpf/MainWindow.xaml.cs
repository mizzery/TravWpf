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
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
namespace travwpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<int> BusyList = new List<int>();
        List<UpgradeWar> UpgradeWarList = new List<UpgradeWar>();
        Timer BuildTaskTimer = new Timer();
        Timer UpgradeWarTimer = new Timer();
        Timer DemolishTaskTimer = new Timer();
        Timer AntiLogout = new Timer();
        bool DemolishTimerStop = false;
        bool BuildTimerStop = false;
        //string Server = "http://sx800.xtravianx.net/";
        string Server = "http://travian-hell.ru/";
        string NavigatedSite = "";
        bool logined = false;
        bool lasttasksuccess = true;
        bool successbuild = false;
        bool BuildTaskProcessed = false;
        bool DemolishTaskProcessed = false;
        bool UpgradeWarProcessed = false;
        bool WBUsed = false;
        List<VillClass> vills = new List<VillClass>();
        List<BuildTaskClass> LBT = new List<BuildTaskClass>();
        List<BuildTaskClass> LDT = new List<BuildTaskClass>();
        public MainWindow()
        {
            InitializeComponent();
        }
        #region FUNCTIONS FOR BROWSER
        bool loaded = false;
        bool initialized = false;
        void WB_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            WpfAnimatedGif.ImageBehavior.SetAnimatedSource(WBL, new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//loader")));
            if (!logined)
            {
                if (WB.Url == null) return; 
                if (WB.Url.ToString().Contains("http://sx800.xtravianx.net/login.php"))
                {
                    Properties.Settings.Default.Login = WB.Document.All["user"].GetAttribute("value");
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Password = WB.Document.All["pw"].GetAttribute("value");
                    Properties.Settings.Default.Save();
                }
                if (WB.Url.ToString().Contains("http://travian-hell.ru/login.php"))
                {
                    Properties.Settings.Default.Login = WB.Document.All["user"].GetAttribute("value");
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Password = WB.Document.All["pw"].GetAttribute("value");
                    Properties.Settings.Default.Save();
                }
            }
        }
        void WB_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            WBADDR.Text = WB.Url.ToString();
        }
        void WB_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            if(logined)
                AntiLogout.Stop();
            NavigatedSite = "";
            loaded = true;
            unblockTextBox();
            if(!logined)
            {
                if (WB.Url.ToString().Contains("http://sx800.xtravianx.net/login.php"))
                {
                    WB.Document.All["user"].InnerText = Properties.Settings.Default.Login;
                    WB.Document.All["pw"].InnerText = Properties.Settings.Default.Password;
                }
                if (WB.Url.ToString().Contains("http://travian-hell.ru/login.php"))
                {
                    WB.Document.All["user"].InnerText = Properties.Settings.Default.Login;
                    WB.Document.All["pw"].InnerText = Properties.Settings.Default.Password;
                }
                if(WB.Url.ToString().Contains("dorf1.php"))
                {
                    logined = true;
                    StartInitAcc.Visibility = System.Windows.Visibility.Visible;
                }
            }
            CheckAdress();
            WpfAnimatedGif.ImageBehavior.SetAnimatedSource(WBL, new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//loadok")));
            if(logined)
                AntiLogout.Start();
        }
        void WB_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if (e.CurrentProgress == 0 && e.MaximumProgress == 0)
                if (NavigatedSite != "")
                {
                    Navigate(NavigatedSite);
                }
            if (e.CurrentProgress == 0 && e.MaximumProgress == 0 && initialized) CheckAdress();
            
        }
        void wait()
        {
            loaded = false;
            while (!loaded) System.Windows.Forms.Application.DoEvents();
        }
        void unblockTextBox()
        {
            foreach (HtmlElement qwe in WB.Document.GetElementsByTagName("input"))
            {
                if (qwe.GetAttribute("type") == "text")
                {
                    qwe.SetAttribute("maxlength", "65535");
                }
            }
        }
        void CheckAdress()
        {
            if (!initialized) return;
            if (WB.Url.ToString().Contains(Server + "dorf1.php"))
            {
                if (WB.Document.GetElementById("vlist") != null)
                {
                    bool temp = false;
                    foreach (HtmlElement qwe in WB.Document.GetElementById("vlist").GetElementsByTagName("td"))
                    {
                        if (qwe.GetAttribute("className") == "dot hl" && !temp)
                        {
                            temp = true;
                            continue;
                        }
                        if (temp && qwe.GetAttribute("className") == "link")
                        {
                            if (qwe.GetElementsByTagName("a")[0].GetAttribute("href").Contains(vills[TIB_LBV.SelectedIndex].href))
                            {
                                VillInitDorf1();
                                return;
                            }
                            else
                            {
                                temp = false;
                            }
                        }

                    }
                }
                else
                    VillInitDorf1();
            }
            if (WB.Url.ToString().Contains(Server + "dorf2.php"))
            {
                if (WB.Document.GetElementById("vlist") != null)
                {
                    bool temp = false;
                    foreach (HtmlElement qwe in WB.Document.GetElementById("vlist").GetElementsByTagName("td"))
                    {
                        if (qwe.GetAttribute("className") == "dot hl" && !temp)
                        {
                            temp = true;
                            continue;
                        }
                        if (temp && qwe.GetAttribute("className") == "link")
                        {
                            if (qwe.GetElementsByTagName("a")[0].GetAttribute("href").Contains(vills[TIB_LBV.SelectedIndex].href.Replace("dorf1", "dorf2")))
                            {
                                VillInitDorf2();
                                return;
                            }
                            else
                            {
                                temp = false;
                            }
                        }
                    }
                }
                else
                    VillInitDorf2();
            }
        }
        private void Navigate(String address)
        {
            if (String.IsNullOrEmpty(address)) return;
            if (address.Equals("about:blank")) return;
            if (!address.StartsWith("http://") && !address.StartsWith("https://"))
            {
                address = "http://" + address;
            }
            try
            {
                NavigatedSite = address;
                WB.Navigate(new Uri(address));
                wait();
            }
            catch (System.UriFormatException)
            {
                return;
            }
        }
        #endregion
        #region APPLICATION INITIALIZE
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region BROWSER EMULATION
            RegistryKey reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            reg.SetValue(Process.GetCurrentProcess().ProcessName + ".exe", 10001, RegistryValueKind.DWord);
            #endregion
            #region SET PROXY
            Prox f = new Prox();
            f.Owner = this;
            f.ShowDialog();
            #endregion
            #region BROWSER INITIALIZE
            WB.ScriptErrorsSuppressed = true;
            WB.DocumentCompleted += WB_DocumentCompleted;
            WB.Navigating += WB_Navigating;
            WB.Navigated += WB_Navigated;
            WB.ProgressChanged += WB_ProgressChanged;
            #endregion
            BuildTimerStartButton.IsEnabled = true;
            BuildTaskDeleteButton.IsEnabled = true;
            BuildTaskDeleteAllButton.IsEnabled = true;
            BuildTimerStopButton.IsEnabled = false;
            DemolishTimerStartButton.IsEnabled = true;
            DemolishTaskDeleteButton.IsEnabled = true;
            DemolishTaskDeleteAllButton.IsEnabled = true;
            DemolishTimerStopButton.IsEnabled = false;
            TIB_D1CI();
            TIB_D2CI();
            GraphInit();
            BusyListInit();
            BuildTaskListInit();
            DemolishTaskListInit();
            BuildTimerInit();
            DemolishTimerInit();
            AntiLogout.Tick += AntiLogout_Tick;
            AntiLogout.Interval = 300000;
            UpgradeWarTimer.Interval = 5000;
            UpgradeWarTimer.Tick += UpgradeWarTimer_Tick;
            TIB_LBV.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            Navigate(Server + "login.php");
        }
        void BusyListInit()//-1 - не запускалось, 0 - заявка на запуск, 1.. - количество выполнений
        {
            BusyList.Add(-1);//build
            BusyList.Add(-1);//demolish
            BusyList.Add(-1);//upgrade units
            BusyList.Add(-1);//celebrations
        }
        void AntiLogout_Tick(object sender, EventArgs e)
        {
            if(!WBUsed)
                Navigate(Server + "dorf1.php");
            System.Windows.MessageBox.Show("asdfasdfasdf");
        }
        void BuildTimerInit()//переписать
        {
            BuildTaskTimer.Interval = 1000;
            BuildTaskTimer.Tick += BuildTaskTimer_Tick;
        }
        void DemolishTimerInit()//переписать
        {
            DemolishTaskTimer.Interval = 1000;
            DemolishTaskTimer.Tick += DemolishTaskTimer_Tick;
        }
        void BuildTaskListInit()
        {
            GridView g = new GridView();
            BuildTaskList.View = g;
            g.Columns.Add(new GridViewColumn { Header = "Задача", DisplayMemberBinding = new System.Windows.Data.Binding("task") });
            g.Columns.Add(new GridViewColumn { Header = "gid", DisplayMemberBinding = new System.Windows.Data.Binding("gid") });
            g.Columns.Add(new GridViewColumn { Header = "link", DisplayMemberBinding = new System.Windows.Data.Binding("link") });
            g.Columns.Add(new GridViewColumn { Header = "level", DisplayMemberBinding = new System.Windows.Data.Binding("level") });
        }
        void DemolishTaskListInit()
        {
            GridView g = new GridView();
            DemolishTaskList.View = g;
            g.Columns.Add(new GridViewColumn { Header = "Задача", DisplayMemberBinding = new System.Windows.Data.Binding("task") });
            g.Columns.Add(new GridViewColumn { Header = "gid", DisplayMemberBinding = new System.Windows.Data.Binding("gid") });
            g.Columns.Add(new GridViewColumn { Header = "link", DisplayMemberBinding = new System.Windows.Data.Binding("link") });
            g.Columns.Add(new GridViewColumn { Header = "level", DisplayMemberBinding = new System.Windows.Data.Binding("level") });
        }
        void GraphInit()
        {
            ID1_R.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//ref"));
            ID2_R.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//ref"));
            WBB.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//arrf"));
            WBF.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//arrb"));
            WBB.PreviewMouseDown += WBB_PreviewMouseDown;
            WBF.PreviewMouseDown += WBF_PreviewMouseDown;
            WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ITL, new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//loader")));
            ITL.Visibility = System.Windows.Visibility.Hidden;
            LabelTB.Visibility = System.Windows.Visibility.Hidden;
        }
        void WBF_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WB.GoForward();
        }
        void WBB_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WB.GoBack();
        }
        void TIB_D1CI()
        {
            for(int i = 0;i<18;i++)
            {
                System.Windows.Controls.Label l = FindName("ID1_L" + (i + 1).ToString()) as System.Windows.Controls.Label;
                l.PreviewMouseDown += LD1_MouseDown;
                l.MouseEnter += LD1_MouseEnter;
                l.MouseLeave += LD1_MouseLeave;
            }
        }
        void TIB_D2CI()
        {
            for (int i = 0; i < 21; i++)
            {
                Grid mimg = (Grid)FindName("ID2_L" + (i + 1).ToString());
                mimg.Background = new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//x")));
                mimg.MouseEnter += mimg_MouseEnter;
                mimg.MouseLeave += mimg_MouseLeave;
                mimg.PreviewMouseDown += mimg_MouseDown;
            }
        }
        void mimg_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }
        void mimg_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Hand;
        }
        void LD1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }
        void LD1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Hand;
        }
        #endregion
        #region ACCOUNT INITIALIZE
        private void StartInitAcc_Click(object sender, RoutedEventArgs e)
        {
            AccInit();
        }
        void AccInit()
        {
            TIOUWALL.Items.Clear();
            TIOUWS.Items.Clear();
            BuildTaskList.Items.Clear();
            DemolishTaskList.Items.Clear();
            TIB_LBV.Items.Clear();
            vills.Clear();
            foreach( HtmlElement qwe in WB.Document.GetElementById("side_navi").GetElementsByTagName("a"))
            {
                if (qwe.GetAttribute("href") == null) continue;
                if(qwe.GetAttribute("href").Contains("spieler.php?uid="))
                {
                    Navigate(qwe.GetAttribute("href"));
                    break;
                }
            }
            if (WB.Document.GetElementById("side_info").Children.Count != 0)
            {
                foreach (HtmlElement qwe in WB.Document.GetElementById("vlist").GetElementsByTagName("tr"))
                {
                    if (qwe.InnerHtml.ToLower().IndexOf("dorf3.php") != -1) continue;
                    string href = "";
                    string name = "";
                    string cox = "";
                    string coy = "";
                    foreach (HtmlElement wer in qwe.GetElementsByTagName("td"))
                    {
                        if (wer.GetAttribute("className") == "link")
                        {
                            href = wer.GetElementsByTagName("a")[0].GetAttribute("href");
                            name = wer.GetElementsByTagName("a")[0].InnerText;
                        }
                        if (wer.GetAttribute("className") == "aligned_coords")
                        {
                            foreach (HtmlElement qqq in wer.GetElementsByTagName("div"))
                            {
                                if (qqq.GetAttribute("className") == "cox")
                                {
                                    cox = qqq.InnerText.Remove(0, 1);
                                }
                                if (qqq.GetAttribute("className") == "coy")
                                {
                                    coy = qqq.InnerText.Remove(qqq.InnerText.Length - 1, 1);
                                }
                            }
                        }
                    }
                    bool cap = false;
                    foreach (HtmlElement asd in WB.Document.GetElementById("bgwhite").GetElementsByTagName("tbody")[0].GetElementsByTagName("tr"))
                    {
                        if (asd.Children[0].Children.Count == 2)
                        {
                            if (asd.Children[2].Children[0].InnerText.Replace("(", "") == cox && asd.Children[2].Children[2].InnerText.Replace(")", "") == coy)
                            {
                                cap = true;
                            }
                        }
                    }
                    href = href.Remove(href.IndexOf("&uid="), href.Length - (href.IndexOf("&uid="))).Replace("spieler", "dorf1");
                    vills.Add(new VillClass { cap = cap, name = name, newdid = href.Remove(0, (Server + "dorf1.php?newdid=").Length), href = href, X = cox, Y = coy, dorf1 = new List<FieldClass>(), dorf2 = new List<CityClass>() });
                }
            }
            else
            {
                string name = "";
                string href = "";
                string cox = "";
                string coy = "";
                foreach (HtmlElement qwe in WB.Document.GetElementById("bgwhite").GetElementsByTagName("td"))
                {
                    if(qwe.GetAttribute("className") == "aligned_coords")
                    {
                        cox = qwe.Children[0].InnerText.Replace("(", "");
                        coy = qwe.Children[2].InnerText.Replace(")", "");
                    }
                }
                Navigate(Server + "dorf3.php");
                foreach(HtmlElement qwe in WB.Document.GetElementById("overview").GetElementsByTagName("td"))
                {
                    if(qwe.GetAttribute("className") == "vil fc")
                    {
                        name = qwe.Children[0].InnerText;
                        href = qwe.Children[0].GetAttribute("href");
                    }
                }
                vills.Add(new VillClass { cap = true, name = name, href = href, newdid = href.Remove(0, (Server + "dorf1.php?newdid=").Length),X = cox,Y = coy, dorf1 = new List<FieldClass>(), dorf2 = new List<CityClass>() });
            }
            if (vills.Count != 0)
            {
                for (int i = 0; i < vills.Count; i++)
                {
                    TIB_LBV.Items.Add(vills[i].name);
                    TIOUWALL.Items.Add(vills[i].name);
                }
            }
            GlobalInit();
            LastBuildTaskInit();
            LastDemolishTaskInit();
            initialized = true;
        }
        void LastBuildTaskInit()
        {
            string str = Properties.Settings.Default.BuildTask;
            string[] str1 = str.Split(new string[] { "<=>" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < str1.Length; i++)
            {
                string gid = str1[i].Split(new string[] { "<:>" }, StringSplitOptions.RemoveEmptyEntries)[0];
                string link = str1[i].Split(new string[] { "<:>" }, StringSplitOptions.RemoveEmptyEntries)[1];
                string newdid = str1[i].Split(new string[] { "<:>" }, StringSplitOptions.RemoveEmptyEntries)[2];
                string tolevel = str1[i].Split(new string[] { "<:>" }, StringSplitOptions.RemoveEmptyEntries)[3];
                bool newdidexist = false;
                for (int j = 0; j < vills.Count; j++)
                {
                    if (newdid == vills[j].newdid)
                        newdidexist = true;
                }
                if (!newdidexist) continue;
                LBT.Add(new BuildTaskClass { gid = gid, link = link, newdid = newdid, tolevel = tolevel });
                BuildTaskList.Items.Add(new BuildTaskList { gid = gid, level = tolevel, link = link, task = "Улучшить " + GetGidName(gid) + " в деревне \"" + vills[TIB_LBV.SelectedIndex].name + "\" до уровня " + tolevel });
            }
        }
        void LastDemolishTaskInit()
        {
            string str = Properties.Settings.Default.DemolishTask;
            string[] str1 = str.Split(new string[] { "<=>" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < str1.Length; i++)
            {
                string gid = str1[i].Split(new string[] { "<:>" }, StringSplitOptions.RemoveEmptyEntries)[0];
                string link = str1[i].Split(new string[] { "<:>" }, StringSplitOptions.RemoveEmptyEntries)[1];
                string newdid = str1[i].Split(new string[] { "<:>" }, StringSplitOptions.RemoveEmptyEntries)[2];
                string tolevel = str1[i].Split(new string[] { "<:>" }, StringSplitOptions.RemoveEmptyEntries)[3];
                bool newdidexist = false;
                for (int j = 0; j < vills.Count; j++)
                {
                    if (newdid == vills[j].newdid)
                        newdidexist = true;
                }
                if (!newdidexist) continue;
                LDT.Add(new BuildTaskClass { gid = gid, link = link, newdid = newdid, tolevel = tolevel});
                DemolishTaskList.Items.Add(new BuildTaskList { gid = gid, level = tolevel, link = link, task = "Снести  " + GetGidName(gid) + " в деревне \"" + vills[TIB_LBV.SelectedIndex].name + "\" до уровня " + tolevel });
            }
        }
        #endregion
        #region BUILD INITIALIZE
        void GlobalInit()
        {
            for (int i = vills.Count - 1; i >= 0; i--)
            {
                TIB_LBV.SelectedIndex = i;
            }
        }
        private void TIB_LBV_SelectedIndexChanged(object sender, EventArgs e)
        {
            Navigate(vills[TIB_LBV.SelectedIndex].href);
            VillInitDorf1();
            ImageInitDorf1();
            Navigate(vills[TIB_LBV.SelectedIndex].href.Replace("dorf1", "dorf2"));
            VillInitDorf2();
            ImageInitDorf2();
        }
        void ImageInitDorf2()
        {
            TIB_ID2.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//bg0"));
            ID2_I1.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[0].igid));
            ID2_I2.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[1].igid));
            ID2_I3.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[2].igid));
            ID2_I4.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[3].igid));
            ID2_I5.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[4].igid));
            ID2_I6.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[5].igid));
            ID2_I7.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[6].igid));
            ID2_I8.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[7].igid));
            ID2_I9.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[8].igid));
            ID2_I10.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[9].igid));
            ID2_I11.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[10].igid));
            ID2_I12.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[11].igid));
            ID2_I13.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[12].igid));
            ID2_I14.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[13].igid));
            ID2_I15.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[14].igid));
            ID2_I16.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[15].igid));
            ID2_I17.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[16].igid));
            ID2_I18.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[17].igid));
            ID2_I19.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[18].igid));
            ID2_I20.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[19].igid));
            ID2_I21.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].dorf2[20].igid));
            ID2SPLIT.Background = new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//x")));
            ID2SPLIT.Children.Clear();
            for (int i = 0; i < 21; i++)
            {
                if (vills[TIB_LBV.SelectedIndex].dorf2[i].gid == "iso") continue;
                Image img = new Image();
                img.Width = 24;
                img.Height = 24;
                img.Stretch = Stretch.Fill;
                img.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                img.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                img.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//ilvl"));
                ID2SPLIT.Children.Add(img);
                Grid mimg = (Grid)FindName("ID2_L" + (i + 1).ToString());
                img.Margin = new Thickness(mimg.Margin.Left - ID2SPLIT.Margin.Left + 20, mimg.Margin.Top - ID2SPLIT.Margin.Top + 4, 0, 0);
                System.Windows.Controls.Label l = new System.Windows.Controls.Label();
                l.Height = 24;
                l.Width = 24;
                l.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                l.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                l.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                l.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                l.Margin = img.Margin;
                l.FontSize = 13;
                l.FontWeight = FontWeights.Bold;
                l.Content = vills[TIB_LBV.SelectedIndex].dorf2[i].level;
                l.Padding = new Thickness(0, 0, 0, 0);
                ID2SPLIT.Children.Add(l);
            }
        }
        void ImageInitDorf1()
        {
            TIB_ID1.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "//images//" + vills[TIB_LBV.SelectedIndex].type));
            for(int i = 0;i<18;i++)
            {
                System.Windows.Controls.Label l = FindName("ID1_L" + (i + 1).ToString()) as System.Windows.Controls.Label;
                l.Content = vills[TIB_LBV.SelectedIndex].dorf1[i].level;
            }
        }
        void VillInitDorf1()
        {
            vills[TIB_LBV.SelectedIndex].dorf1 = new List<FieldClass>();
            int counter = 1;
            if (WB.Document.GetElementById("village_map") == null) return;
            vills[TIB_LBV.SelectedIndex].type = WB.Document.GetElementById("village_map").GetAttribute("className");
            foreach (HtmlElement qwe in WB.Document.GetElementById("village_map").GetElementsByTagName("img"))
            {
                if (qwe.GetAttribute("className") == null || qwe.GetAttribute("className").IndexOf("reslevel") == -1) continue;
                if (Convert.ToInt32(qwe.GetAttribute("className").Split(' ')[1].Remove(0, 2)) == counter && qwe.GetAttribute("className").ToLower().IndexOf("reslevel") != -1)
                {
                    vills[TIB_LBV.SelectedIndex].dorf1.Add(new FieldClass { link = Server + "build.php?newdid=" + vills[TIB_LBV.SelectedIndex].newdid + "&id=" + counter, level = qwe.GetAttribute("className").Split(' ')[2].Remove(0, 5), gid = GetGidFields(vills[TIB_LBV.SelectedIndex].type, counter) });
                    counter++;
                }
            }
            ImageInitDorf1();
        }
        void VillInitDorf2()
        {
            vills[TIB_LBV.SelectedIndex].dorf2 = new List<CityClass>();
            foreach (HtmlElement qwe in WB.Document.GetElementById("village_map").GetElementsByTagName("img"))
            {
                if (qwe.GetAttribute("className") == null) continue;
                if (qwe.GetAttribute("className").ToLower().IndexOf("building") != -1)
                {
                    if (qwe.GetAttribute("className").Split(' ')[2] == "iso")
                    {
                        vills[TIB_LBV.SelectedIndex].dorf2.Add(new CityClass { gid = "iso", link = Server + "build.php?newdid=" + vills[TIB_LBV.SelectedIndex].newdid + "&id=" + (Convert.ToInt32(qwe.GetAttribute("className").Split(' ')[1].Remove(0, 1)) + 18).ToString(), level = "", igid = qwe.GetAttribute("className").Split(' ')[2] });
                    }
                    else
                    {
                        foreach (HtmlElement wer in WB.Document.GetElementById("levels").GetElementsByTagName("div"))
                        {
                            if (wer.GetAttribute("className")[0] == 'd' && wer.GetAttribute("className").Remove(0, 1) == qwe.GetAttribute("className").Split(' ')[1].Remove(0, 1))
                            {
                                if (qwe.GetAttribute("className").Split(' ')[2].Remove(0, 1).IndexOf('b') == -1)
                                    vills[TIB_LBV.SelectedIndex].dorf2.Add(new CityClass { gid = qwe.GetAttribute("className").Split(' ')[2].Remove(0, 1), link = Server + "build.php?newdid=" + vills[TIB_LBV.SelectedIndex].newdid + "&id=" + (Convert.ToInt32(qwe.GetAttribute("className").Split(' ')[1].Remove(0, 1)) + 18).ToString(), level = wer.InnerText, igid = qwe.GetAttribute("className").Split(' ')[2] });
                                else
                                    vills[TIB_LBV.SelectedIndex].dorf2.Add(new CityClass { gid = qwe.GetAttribute("className").Split(' ')[2].Remove(0, 1).Replace("b", ""), link = Server + "build.php?newdid=" + vills[TIB_LBV.SelectedIndex].newdid + "&id=" + (Convert.ToInt32(qwe.GetAttribute("className").Split(' ')[1].Remove(0, 1)) + 18).ToString(), level = wer.InnerText, igid = qwe.GetAttribute("className").Split(' ')[2] });
                            }
                        }
                    }
                }
                else
                {
                    if( qwe.GetAttribute("className").ToLower().IndexOf("dx1") != -1)
                    {
                        foreach (HtmlElement wer in WB.Document.GetElementById("levels").GetElementsByTagName("div"))
                        {
                            if (wer.GetAttribute("className") == "l39")
                            {
                                vills[TIB_LBV.SelectedIndex].dorf2.Add(new CityClass { gid = "16", link = Server + "build.php?newdid=" + vills[TIB_LBV.SelectedIndex].newdid + "&id=39", level = wer.InnerText, igid = qwe.GetAttribute("className").Split(' ')[1] });
                            }
                        }
                    }
                }
            }
            if(vills[TIB_LBV.SelectedIndex].dorf2.Count == 20)
            {
                vills[TIB_LBV.SelectedIndex].dorf2.Add(new CityClass { igid = "g16e", gid = "16", link = Server + "build.php?newdid=" + vills[TIB_LBV.SelectedIndex].newdid + "&id=39", level = "0" });
            }
            bool added = false;
            foreach (HtmlElement wer in WB.Document.GetElementById("levels").GetElementsByTagName("div"))
            {
                if(wer.GetAttribute("className") == "l40")
                {
                    vills[TIB_LBV.SelectedIndex].dorf2.Add(new CityClass { gid = "31", link = Server + "build.php?newdid=" + vills[TIB_LBV.SelectedIndex].newdid + "&id=40", level = wer.InnerText });
                    added = true;
                    break;
                }
            }
            if(!added)
                vills[TIB_LBV.SelectedIndex].dorf2.Add(new CityClass { gid = "31", link = Server + "build.php?newdid=" + vills[TIB_LBV.SelectedIndex].newdid + "&id=40", level = "0" });
            ImageInitDorf2();
        }
        private void ID1_R_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(initialized)
                Navigate(vills[TIB_LBV.SelectedIndex].href);
        }
        private void ID2_R_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(initialized)
                Navigate(vills[TIB_LBV.SelectedIndex].href.Replace("dorf1","dorf2"));
        }
        #endregion
        #region BUILDING AND DEMOLISH
        void mimg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                successbuild = false;
                Grid g = sender as Grid;
                int num = Convert.ToInt32(g.Name.Remove(0, 5)) - 1;
                Navigate(vills[TIB_LBV.SelectedIndex].dorf2[num].link);
                if (WB.Document.GetElementById("contract") == null)
                {
                    //if (vills[TIB_LBV.SelectedIndex].dorf2[num].gid == "iso")
                    //{
                    //    TC_main.SelectedIndex = 0;
                    //}
                    //else
                    //{
                        Navigate(vills[TIB_LBV.SelectedIndex].href.Replace("dorf1", "dorf2"));
                    //}
                    return;
                }
                foreach (HtmlElement qwe in WB.Document.GetElementById("contract").GetElementsByTagName("a"))
                {
                    if (qwe.GetAttribute("className") == null) continue;
                    if (qwe.GetAttribute("className") == "build")
                    {
                        Navigate(qwe.GetAttribute("href"));
                        successbuild = true;
                    }
                }
                if (!successbuild)
                {
                    foreach (HtmlElement qwe in WB.Document.GetElementById("contract").GetElementsByTagName("span"))
                    {
                        if (qwe.GetAttribute("className") == null) continue;
                        if (qwe.GetAttribute("className") == "none")
                        {
                            Navigate(vills[TIB_LBV.SelectedIndex].href.Replace("dorf1", "dorf2"));
                        }
                    }
                }
            }
            #region RIGHT BUTTON
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Grid g = sender as Grid;
                int num = Convert.ToInt32(g.Name.Remove(0, 5)) - 1;
                if (vills[TIB_LBV.SelectedIndex].dorf2[num].gid != "iso")
                {
                    int level = Convert.ToInt32(vills[TIB_LBV.SelectedIndex].dorf2[num].level);
                    int maxlevel = GetMaxLevel(vills[TIB_LBV.SelectedIndex].dorf2[num].gid, vills[TIB_LBV.SelectedIndex].cap);
                    Grid gr = new Grid();
                    gr.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    gr.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    gr.Width = 138;
                    gr.Background = new SolidColorBrush(Color.FromArgb(64, 0, 0, 0));
                    gr.Margin = new Thickness((sender as Grid).Margin.Left - 36, (sender as Grid).Margin.Top - 16, 0, 0);//Mouse.GetPosition(TIB_MG).X, Mouse.GetPosition(TIB_MG).Y, 0, 0);
                    TIB_MG.Children.Add(gr);
                    gr.MouseLeave += TIBGB_MouseLeave;
                    this.RegisterName("GCM", gr);
                    int rembuttons = 0;
                    int buildbuttons = 0;
                    for(int i = 0;i<level;i++)
                    {
                        System.Windows.Controls.Button b = new System.Windows.Controls.Button();
                        b.Width = 32;
                        b.Height = 32;
                        b.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        b.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                        b.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        b.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                        b.Background = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
                        b.Margin = new Thickness(16, 16 + ((level - 1) - i) * 32, 0 ,0);
                        b.Padding = new Thickness(0, 0, 0, 0);
                        b.FontSize = 18;
                        b.FontWeight = FontWeights.Bold;
                        b.Name = "ID2BB_" + num + "_" +vills[TIB_LBV.SelectedIndex].dorf2[num].gid + "_" + i;
                        b.PreviewMouseDown += ID2GC_MouseDown;
                        b.Content = i.ToString();
                        gr.Children.Add(b);
                        rembuttons++;
                    }
                    for (int i = level; i == level; i++)
                    {
                        System.Windows.Controls.Button b = new System.Windows.Controls.Button();
                        b.Width = 32;
                        b.Height = 32;
                        b.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        b.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                        b.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        b.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                        b.Background = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));
                        b.Margin = new Thickness(53, 16, 0, 0);
                        b.Padding = new Thickness(0, 0, 0, 0);
                        b.FontSize = 18;
                        b.FontWeight = FontWeights.Bold;
                        b.Name = "ID2BB_" + num + "_" + vills[TIB_LBV.SelectedIndex].dorf2[num].gid + "_" + i;
                        //b.PreviewMouseDown += ID2GC_MouseDown;
                        b.Content = i.ToString();
                        gr.Children.Add(b);
                    }
                    for(int i = level + 1, y = 0;i < maxlevel + 1;i++,y++)
                    {
                        System.Windows.Controls.Button b = new System.Windows.Controls.Button();
                        b.Width = 32;
                        b.Height = 32;
                        b.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        b.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                        b.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        b.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                        b.Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                        b.Margin = new Thickness(90, 16 + y * 32, 0, 0);
                        b.Padding = new Thickness(0, 0, 0, 0);
                        b.FontSize = 18;
                        b.FontWeight = FontWeights.Bold;
                        b.Name = "ID2BB_" + num + "_" + vills[TIB_LBV.SelectedIndex].dorf2[num].gid + "_" + i;
                        b.PreviewMouseDown += ID2GC_MouseDown;
                        b.Content = i.ToString();
                        gr.Children.Add(b);
                        buildbuttons++;
                    }
                    if(rembuttons > buildbuttons)
                        gr.Height = 32 + rembuttons * 32;
                    else
                        gr.Height = 32 + buildbuttons * 32;
                }
                #endregion
            }
        }
        void ID2GC_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Button b = sender as System.Windows.Controls.Button;
            int num = Convert.ToInt32(b.Name.Split('_')[1]);
            string gid = b.Name.Split('_')[2];
            string currlevel = vills[TIB_LBV.SelectedIndex].dorf2[num].level;
            string tolevel = b.Name.Split('_')[3];
            string link = vills[TIB_LBV.SelectedIndex].dorf2[num].link;
            AddTaskToList(gid, link, currlevel, tolevel);
            TIB_MG.Children.Remove(FindName("GCM") as Grid);

        }
        void ID1GC_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Button b = sender as System.Windows.Controls.Button;
            string dorf = b.Name[2].ToString();
            int num = Convert.ToInt32(b.Name.Split('_')[1]);
            string gid = b.Name.Split('_')[2];
            string currlevel = vills[TIB_LBV.SelectedIndex].dorf1[num].level;
            string tolevel = b.Name.Split('_')[3];
            string link = vills[TIB_LBV.SelectedIndex].dorf1[num].link;
            AddTaskToList(gid, link, currlevel, tolevel);
            TIB_MG.Children.Remove(FindName("GCM") as Grid);
        }
        private void LD1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                successbuild = false;
                System.Windows.Controls.Label g = sender as System.Windows.Controls.Label;
                int num = Convert.ToInt32(g.Name.Remove(0, 5)) - 1;
                Navigate(vills[TIB_LBV.SelectedIndex].dorf1[num].link);
                if (WB.Document.GetElementById("contract") == null)
                {
                    Navigate(vills[TIB_LBV.SelectedIndex].href);
                    return;
                }
                foreach (HtmlElement qwe in WB.Document.GetElementById("contract").GetElementsByTagName("a"))
                {
                    if (qwe.GetAttribute("className") == null) continue;
                    if (qwe.GetAttribute("className") == "build")
                    {
                        Navigate(qwe.GetAttribute("href"));
                        successbuild = true;
                    }
                }
                if (!successbuild)
                {
                    foreach (HtmlElement qwe in WB.Document.GetElementById("contract").GetElementsByTagName("span"))
                    {
                        if (qwe.GetAttribute("className") == null) continue;
                        if (qwe.GetAttribute("className") == "none")
                        {
                            Navigate(vills[TIB_LBV.SelectedIndex].href);
                        }
                    }
                }
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                System.Windows.Controls.Label g = sender as System.Windows.Controls.Label;
                int num = Convert.ToInt32(g.Name.Remove(0, 5)) - 1;
                if (vills[TIB_LBV.SelectedIndex].dorf1[num].gid != "iso")
                {
                    int level = Convert.ToInt32(vills[TIB_LBV.SelectedIndex].dorf1[num].level);
                    int maxlevel = GetMaxLevel(vills[TIB_LBV.SelectedIndex].dorf1[num].gid, vills[TIB_LBV.SelectedIndex].cap);
                    Grid gr = new Grid();
                    gr.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    gr.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    gr.Width = 138;
                    gr.Background = new SolidColorBrush(Color.FromArgb(64, 0, 0, 0));
                    gr.Margin = new Thickness((sender as System.Windows.Controls.Label).Margin.Left - 36, (sender as System.Windows.Controls.Label).Margin.Top - 16, 0, 0);//Mouse.GetPosition(TIB_MG).X, Mouse.GetPosition(TIB_MG).Y, 0, 0);
                    TIB_MG.Children.Add(gr);
                    gr.MouseLeave += TIBGB_MouseLeave;
                    this.RegisterName("GCM", gr);
                    int rembuttons = 0;
                    int buildbuttons = 0;
                    for (int i = level; i == level; i++)
                    {
                        System.Windows.Controls.Button b = new System.Windows.Controls.Button();
                        b.Width = 32;
                        b.Height = 32;
                        b.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        b.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                        b.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        b.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                        b.Background = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));
                        b.Margin = new Thickness(53, 16, 0, 0);
                        b.Padding = new Thickness(0, 0, 0, 0);
                        b.FontSize = 18;
                        b.FontWeight = FontWeights.Bold;
                        b.Name = "ID1BB_" + num + "_" + vills[TIB_LBV.SelectedIndex].dorf1[num].gid + "_" + i;
                        //b.PreviewMouseDown += ID1GC_MouseDown;
                        b.Content = i.ToString();
                        gr.Children.Add(b);
                    }
                    for (int i = level + 1, y = 0; i < maxlevel + 1; i++, y++)
                    {
                        System.Windows.Controls.Button b = new System.Windows.Controls.Button();
                        b.Width = 32;
                        b.Height = 32;
                        b.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        b.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                        b.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        b.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                        b.Background = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));
                        b.Margin = new Thickness(90, 16 + y * 32, 0, 0);
                        b.Padding = new Thickness(0, 0, 0, 0);
                        b.FontSize = 18;
                        b.FontWeight = FontWeights.Bold;
                        b.Name = "ID1BB_" + num + "_" + vills[TIB_LBV.SelectedIndex].dorf1[num].gid + "_" + i;
                        b.PreviewMouseDown += ID1GC_MouseDown;
                        b.Content = i.ToString();
                        gr.Children.Add(b);
                        buildbuttons++;
                    }
                    if (rembuttons >= buildbuttons)
                        gr.Height = 32 + rembuttons * 32;
                    if (rembuttons <= buildbuttons)
                        gr.Height = 32 + buildbuttons * 32;
                    if (rembuttons == 0 && buildbuttons == 0)
                        gr.Height = 64;
                }
            }
        }
        void AddTaskToList(string gid, string link, string currlevel, string tolevel)
        {
            if (Convert.ToInt32(currlevel) < Convert.ToInt32(tolevel))
            {
                BuildTaskList.Items.Add(new BuildTaskList { gid = gid, level = tolevel, link = link, task = "Улучшить " + GetGidName(gid) + " в деревне \"" + vills[TIB_LBV.SelectedIndex].name +  "\" до уровня " + tolevel });
                LBT.Add(new BuildTaskClass { gid = gid, tolevel = tolevel, link = link, newdid = vills[TIB_LBV.SelectedIndex].newdid }); string TaskList = "";
                for (int i = 0; i < LBT.Count; i++)
                {
                    TaskList += LBT[i].gid + "<:>" + LBT[i].link + "<:>" + LBT[i].newdid + "<:>" + LBT[i].tolevel + "<=>";
                }
                Properties.Settings.Default.BuildTask = TaskList;
                Properties.Settings.Default.Save();
            }
            else
            {
                DemolishTaskList.Items.Add(new BuildTaskList { gid = gid, level = tolevel, link = link, task = "Снести  " + GetGidName(gid) + " в деревне \"" + vills[TIB_LBV.SelectedIndex].name + "\" до уровня " + tolevel });
                LDT.Add(new BuildTaskClass { gid = gid, tolevel = tolevel, link = link, newdid = vills[TIB_LBV.SelectedIndex].newdid }); string TaskList = "";
                for (int i = 0; i < LDT.Count; i++)
                {
                    TaskList += LDT[i].gid + "<:>" + LDT[i].link + "<:>" + LDT[i].newdid + "<:>" + LDT[i].tolevel + "<=>";
                }
                Properties.Settings.Default.DemolishTask = TaskList;
                Properties.Settings.Default.Save();
            }
            
        }
        void TIBGB_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Grid g = sender as Grid;
            TIB_MG.Children.Remove(g);
            UnregisterName("GCM");
        }
        private void BuildDeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (BuildTaskList.SelectedIndex == -1)
            {
                System.Windows.MessageBox.Show("Вебырите Задание для удаления");
            }
            else
            {
                LBT.RemoveAt(BuildTaskList.SelectedIndex);
                BuildTaskList.Items.RemoveAt(BuildTaskList.SelectedIndex);
                string TaskList = "";
                for (int i = 0; i < LBT.Count; i++)
                {
                    TaskList += LBT[i].gid + "<:>" + LBT[i].link + "<:>" + LBT[i].newdid + "<:>" + LBT[i].tolevel + "<=>";
                }
                Properties.Settings.Default.BuildTask = TaskList;
                Properties.Settings.Default.Save();
            }
        }
        private void BuildDeleteAllTaskButton_Click(object sender, RoutedEventArgs e)
        {
            LBT.Clear();
            BuildTaskList.Items.Clear();
            Properties.Settings.Default.BuildTask = "";
            Properties.Settings.Default.Save();
        }
        private void BuildStartTaskButton_Click(object sender, RoutedEventArgs e)
        {
            BuildTaskProcessed = true;
            ITL.Visibility = System.Windows.Visibility.Visible;
            LabelTB.Visibility = System.Windows.Visibility.Visible;
            BuildTaskTimer.Interval = 500;
            BuildTimerStop = false;
            BuildTaskTimer.Start();
            BuildTimerStartButton.IsEnabled = false;
            BuildTaskDeleteButton.IsEnabled = false;
            BuildTaskDeleteAllButton.IsEnabled = false;
            BuildTimerStopButton.IsEnabled = true;
        }
        private void BuildStopTaskButton_Click(object sender, RoutedEventArgs e)
        {
            BuildTaskProcessed = false;
            if (!DemolishTaskProcessed)
            {
                ITL.Visibility = System.Windows.Visibility.Hidden;
                LabelTB.Visibility = System.Windows.Visibility.Hidden;
            }
            BuildTaskTimer.Stop();
            BuildTimerStop = true;
            BuildTimerStartButton.IsEnabled = true;
            BuildTaskDeleteButton.IsEnabled = true;
            BuildTaskDeleteAllButton.IsEnabled = true;
            BuildTimerStopButton.IsEnabled = false;
        }
        private void DemolishDeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (DemolishTaskList.SelectedIndex == -1)
            {
                System.Windows.MessageBox.Show("Вебырите Задание для удаления");
            }
            else
            {
                LDT.RemoveAt(DemolishTaskList.SelectedIndex);
                DemolishTaskList.Items.RemoveAt(DemolishTaskList.SelectedIndex);
                string TaskList = "";
                for (int i = 0; i < LDT.Count; i++)
                {
                    TaskList += LDT[i].gid + "<:>" + LDT[i].link + "<:>" + LDT[i].newdid + "<:>" + LDT[i].tolevel + "<=>";
                }
                Properties.Settings.Default.DemolishTask = TaskList;
                Properties.Settings.Default.Save();
            }
        }
        private void DemolishDeleteAllTaskButton_Click(object sender, RoutedEventArgs e)
        {
            LDT.Clear();
            DemolishTaskList.Items.Clear();
            Properties.Settings.Default.DemolishTask = "";
            Properties.Settings.Default.Save();
        }
        private void DemolishStartTaskButton_Click(object sender, RoutedEventArgs e)
        {
            DemolishTaskProcessed = true;
            ITL.Visibility = System.Windows.Visibility.Visible;
            LabelTB.Visibility = System.Windows.Visibility.Visible;
            DemolishTaskTimer.Interval = 500;
            DemolishTimerStop = false;
            DemolishTaskTimer.Start();
            DemolishTimerStartButton.IsEnabled = false;
            DemolishTaskDeleteButton.IsEnabled = false;
            DemolishTaskDeleteAllButton.IsEnabled = false;
            DemolishTimerStopButton.IsEnabled = true;
        }
        private void DemolishStopTaskButton_Click(object sender, RoutedEventArgs e)
        {
            DemolishTaskProcessed = false;
            if (!BuildTaskProcessed)
            {
                ITL.Visibility = System.Windows.Visibility.Hidden;
                LabelTB.Visibility = System.Windows.Visibility.Hidden;
            }
            DemolishTaskTimer.Stop();
            DemolishTimerStop = true;
            DemolishTimerStartButton.IsEnabled = true;
            DemolishTaskDeleteButton.IsEnabled = true;
            DemolishTaskDeleteAllButton.IsEnabled = true;
            DemolishTimerStopButton.IsEnabled = false;
        }
        private void DemolishTaskTimer_Tick(object sender, EventArgs e)
        {
            if(!DemolishTimerStop)
                DemolishFromTasklist();
        }
        private void BuildTaskTimer_Tick(object sender, EventArgs e)
        {
            if(!BuildTimerStop)
                BuildFromTaskList();
        }
        public static void PerformClick(System.Windows.Controls.Button btn)
        {
            btn.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
        }
        void BuildFromTaskList()
        {
            BuildTaskTimer.Stop();
            if (LBT.Count == 0)
            {
                PerformClick(BuildTimerStopButton);
                System.Windows.MessageBox.Show("Все готово мой Господин!");
                return;
            }
            if (WBUsed)
            {
                BusyList[0] = 0;
                BuildTaskTimer.Interval = 1000;
                BuildTaskTimer.Start();
                return;
            }
            else
            {
                if(BusyList[0] > 5)
                {
                    for(int i = 0;i<BusyList.Count;i++)
                    {
                        if(BusyList[i] == 0)
                        {
                            BusyList[0] = -1;
                            BuildTaskTimer.Interval = 3000;
                            BuildTaskTimer.Start();
                            return;
                        }
                    }
                }
            }
            if (BusyList[0] == -1)
                BusyList[0] = 1;
            else
                BusyList[0]++;
            WBUsed = true;
            MWind.Title = "WebBrowser busy(Build)" + BusyList[0];
            if(lasttasksuccess)
            {
                lasttasksuccess = false;
                successbuild = false;
                bool taskcompleted = false;
                Navigate(LBT.First().link);
                if(WB.Document.GetElementById("content") == null)
                {
                    WBUsed = false;
                    BuildTaskTimer.Interval = 1000;
                    BuildTaskTimer.Start(); 
                    MWind.Title = "";
                    return;
                }
                if (WB.Document.GetElementById("contract") == null)
                {
                    if (WB.Url.ToString().Contains(Server + "login.php"))
                    {
                        foreach(HtmlElement qwe in WB.Document.GetElementsByTagName("input"))
                        {
                            if (qwe.GetAttribute("type") == "image")
                                if (qwe.GetAttribute("value") == "login")
                                {
                                    qwe.InvokeMember("click");
                                    wait();
                                    successbuild = true;
                                    BuildTaskTimer.Interval = 500;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        taskcompleted = true;
                        successbuild = true;
                    }
                }
                if (!successbuild)
                {
                    string cstr = WB.Document.GetElementById("contract").InnerText.Split(':')[0];
                    string nextlevel = "";
                    foreach(char ch in cstr)
                    {
                        if (char.IsDigit(ch))
                            nextlevel += ch.ToString();
                    }
                    if (Convert.ToInt32(nextlevel) > Convert.ToInt32(LBT.First().tolevel))
                    {
                        taskcompleted = true;
                        successbuild = true;
                    }
                    else
                    {
                        foreach (HtmlElement qwe in WB.Document.GetElementById("contract").GetElementsByTagName("a"))
                        {
                            if (qwe.GetAttribute("className") == null) continue;
                            if (qwe.GetAttribute("className") == "build")
                            {
                                int hours = Convert.ToDateTime(WB.Document.GetElementById("contract").InnerText.Split('|')[5].Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim()).Hour;
                                int minutes = Convert.ToDateTime(WB.Document.GetElementById("contract").InnerText.Split('|')[5].Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim()).Minute;
                                int seconds = Convert.ToDateTime(WB.Document.GetElementById("contract").InnerText.Split('|')[5].Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim()).Second;
                                int time = (hours * 3600 + minutes * 60 + seconds)*1000;
                                if ((time - 1000) <= 0)
                                    BuildTaskTimer.Interval = 1;
                                else
                                    BuildTaskTimer.Interval = time - 1000;
                                Navigate(qwe.GetAttribute("href"));
                                successbuild = true;
                            }
                        }
                    }
                }
                if (!successbuild)
                {
                    foreach (HtmlElement qwe in WB.Document.GetElementById("contract").GetElementsByTagName("span"))
                    {
                        if (qwe.GetAttribute("className") == null) continue;
                        if (qwe.GetAttribute("className") == "none" && (qwe.InnerText.ToLower().Contains("все рабочие заняты") || qwe.InnerText.ToLower().Contains("достаточно ресурсов")))
                        {
                            BuildTaskTimer.Interval = 3000;
                            //Navigate(vills[TIB_LBV.SelectedIndex].href.Replace("dorf1", "dorf2"));
                        }
                    }
                }
                if (taskcompleted)
                {
                    BuildTaskTimer.Interval = 1000;
                    BuildTaskList.Items.RemoveAt(0);
                    LBT.RemoveAt(0);
                    string TaskList = "";
                    for (int i = 0; i < LBT.Count; i++)
                    {
                        TaskList += LBT[i].gid + "<:>" + LBT[i].link + "<:>" + LBT[i].newdid + "<:>" + LBT[i].tolevel + "<=>";
                    }
                    Properties.Settings.Default.BuildTask = TaskList;
                    Properties.Settings.Default.Save();
                }
                lasttasksuccess = true;
                if (LBT.Count == 0)
                {
                    PerformClick(BuildTimerStopButton);
                    WBUsed = false;
                    MWind.Title = "";
                    System.Windows.MessageBox.Show("Все готово мой Господин!");
                    return;
                }
            }
            WBUsed = false;
            MWind.Title = "";
            BuildTaskTimer.Start();
        }
        void DemolishFromTasklist()
        {
            DemolishTaskTimer.Stop();
            if (LDT.Count == 0)
            {
                PerformClick(DemolishTimerStopButton);
                System.Windows.MessageBox.Show("Все готово мой Господин!");
                return;
            }
            if (WBUsed)
            {
                BusyList[1] = 0;
                DemolishTaskTimer.Interval = 1000;
                DemolishTaskTimer.Start();
                return;
            } 
            else
            {
                if (BusyList[1] > 5)
                {
                    for (int i = 0; i < BusyList.Count; i++)
                    {
                        if (BusyList[i] == 0)
                        {
                            BusyList[1] = -1;
                            BuildTaskTimer.Interval = 3000;
                            BuildTaskTimer.Start();
                            return;
                        }
                    }
                }
            }
            if (BusyList[1] == -1)
                BusyList[1] = 1;
            else
                BusyList[1]++;
            WBUsed = true;
            bool taskcomplete = false;
            MWind.Title = "WebBrowser busy(Demolish)" + BusyList[1];
            bool MBFind = false;
            for(int i = 0;i<vills.Count;i++)
            {
                if(vills[i].newdid == LDT.Last().newdid)
                {
                    for(int j = 0;j<vills[i].dorf2.Count;j++)
                    {
                        if(vills[i].dorf2[j].gid == "15")
                        {
                            if (Convert.ToInt32(vills[i].dorf2[j].level) >= 10)
                            {
                                Navigate(vills[i].dorf2[j].link);
                                MBFind = true;
                                break;
                            }
                            else
                            {
                                LDT.Clear();
                                DemolishTaskList.Items.Clear();
                                Properties.Settings.Default.DemolishTask = "";
                                Properties.Settings.Default.Save();
                                WBUsed = false;
                                MWind.Title = "";
                                System.Windows.MessageBox.Show("Главное здание меньше 10 лвла");
                                PerformClick(DemolishTimerStopButton);
                                return;
                            }
                        }
                    }
                    if (MBFind) break;
                }
            }
            if (!MBFind)
            {
                WBUsed = false;
                LDT.Clear();
                DemolishTaskList.Items.Clear();
                Properties.Settings.Default.DemolishTask = "";
                Properties.Settings.Default.Save();
                MWind.Title = "";
                System.Windows.MessageBox.Show("Главное здание не найдено");
                return;
            }
            else
            {
                if(WB.Document.GetElementsByTagName("form").Count == 0)
                {
                    WBUsed = false;
                    DemolishTaskTimer.Interval = 3000;
                    MWind.Title = "";
                    DemolishTaskTimer.Start();
                    return;
                }
                string id = LDT.First().link.Remove(0, LDT.First().link.IndexOf("&id=") + 4);
                string currlevel = "";
                foreach(HtmlElement qwe in WB.Document.GetElementsByTagName("form")[0].Children[0].Children)
                {
                    if(qwe.GetAttribute("value") == id)
                    {
                        currlevel = qwe.InnerText.Split('.')[1].Remove(0, qwe.InnerText.Split('.')[1].IndexOf("(lvl")).Replace("(", "").Replace(")", "").Replace("l", "").Replace("v", "").Replace(" ", "");
                        break;
                    }
                }
                if (Convert.ToInt32(LDT.First().tolevel) < Convert.ToInt32(currlevel))
                {
                    WB.Document.GetElementsByTagName("form")[0].Children[0].SetAttribute("value",id);
                    WB.Document.All["btn_demolish"].InvokeMember("click"); 
                }
                if (Convert.ToInt32(LDT.First().tolevel) >= Convert.ToInt32(currlevel))
                {
                    taskcomplete = true;
                } 
                if (taskcomplete)
                {
                    DemolishTaskTimer.Interval = 3000;
                    DemolishTaskList.Items.RemoveAt(0);
                    LDT.RemoveAt(0);
                    string TaskList = "";
                    for (int i = 0; i < LDT.Count; i++)
                    {
                        TaskList += LDT[i].gid + "<:>" + LDT[i].link + "<:>" + LDT[i].newdid + "<:>" + LDT[i].tolevel + "<=>";
                    }
                    Properties.Settings.Default.DemolishTask = TaskList;
                    Properties.Settings.Default.Save();
                } 
                if (LDT.Count == 0)
                {
                    PerformClick(DemolishTimerStopButton);
                    WBUsed = false;
                    MWind.Title = "";
                    System.Windows.MessageBox.Show("Все готово мой Господин!");
                    return;
                }
            }
            WBUsed = false;
            MWind.Title = "";
            DemolishTaskTimer.Start();
        }
        #endregion
        #region WARIOR UPGRADE
        void AddVill(int num)
        {
            bool g12 = false;
            bool g13 = false;
            for(int i = 0;i<vills[num].dorf2.Count;i++)
            {
                if (vills[num].dorf2[i].gid == "12")
                    g12 = true;
                if (vills[num].dorf2[i].gid == "13") 
                    g13 = true;
            }
            if (g12 || g13)
            {
                bool exist = false;
                for (int i = 0; i < UpgradeWarList.Count;i++)
                    if (UpgradeWarList[i].villnum == num)
                        exist = true;
                if (!exist)
                {
                    UpgradeWarList.Add(new UpgradeWar { villnum = num, att = g12, def = g13 });
                    TIOUWS.Items.Add(vills[num].name);
                }
            }
        }
        void UpgradeWarTimer_Tick(object sender, EventArgs e)
        {
            UpgradeWarTimer.Stop();
            if (UpgradeWarList.Count == 0)
            {
                PerformClick(DemolishTimerStopButton);
                System.Windows.MessageBox.Show("Все готово мой Господин!");
                return;
            }
            if (WBUsed)
            {
                BusyList[2] = 0;
                DemolishTaskTimer.Interval = 1000;
                DemolishTaskTimer.Start();
                return;
            }
            else
            {
                if (BusyList[2] > 5)
                {
                    for (int i = 0; i < BusyList.Count; i++)
                    {
                        if (BusyList[i] == 0)
                        {
                            BusyList[2] = -1;
                            BuildTaskTimer.Interval = 5000;
                            BuildTaskTimer.Start();
                            return;
                        }
                    }
                }
            }
            if (BusyList[2] == -1)
                BusyList[2] = 1;
            else
                BusyList[2]++;
            WBUsed = true;
        }
        void TIOUWIS_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UpgradeWarTimer.Stop();

        }
        void TIOUWIP_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UpgradeWarTimer.Start();
        }
        private void AddSelectedVill(object sender, RoutedEventArgs e)
        {
            if(TIOUWALL.SelectedIndex != -1)
                AddVill(TIOUWALL.SelectedIndex);
        }
        private void AddAllVill(object sender, RoutedEventArgs e)
        {
            if (TIOUWALL.SelectedIndex != -1)
                for(int i = 0;i<vills.Count;i++)
                    AddVill(i);
        }
        private void DeleteAllVill(object sender, RoutedEventArgs e)
        {
            UpgradeWarList.Clear();
            TIOUWS.Items.Clear();
        }
        private void DeleteSelectedVill(object sender, RoutedEventArgs e)
        {
            if (TIOUWS.SelectedIndex != -1)
            {
                UpgradeWarList.RemoveAt(TIOUWS.SelectedIndex);
                TIOUWS.Items.RemoveAt(TIOUWS.SelectedIndex);
            }
        }
        #endregion
        #region GET INFO FUNCTIONS
        string GetGidFields(string VillType, int num)
        {
            int[] f1 = new int[18] { 4, 4, 1, 4, 4, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 1 };
            int[] f2 = new int[18] { 3, 4, 1, 3, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2 };
            int[] f3 = new int[18] { 1, 4, 1, 3, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2 };
            int[] f4 = new int[18] { 1, 4, 1, 2, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2 };
            int[] f5 = new int[18] { 1, 4, 1, 3, 1, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2 };
            int[] f6 = new int[18] { 4, 4, 1, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2, 4, 4 };
            int[] f7 = new int[18] { 1, 4, 4, 1, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2 };
            int[] f8 = new int[18] { 3, 4, 4, 1, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2 };
            int[] f9 = new int[18] { 3, 4, 4, 1, 1, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2 };
            int[] f10 = new int[18] { 3, 4, 1, 2, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2 };
            int[] f11 = new int[18] { 3, 1, 1, 3, 1, 4, 4, 3, 3, 2, 2, 3, 1, 4, 4, 2, 4, 4 };
            int[] f12 = new int[18] { 1, 4, 1, 1, 2, 2, 3, 4, 4, 3, 3, 4, 4, 1, 4, 2, 1, 2 };
            switch (VillType)
            {
                case "f1": return f1[num - 1].ToString();
                case "f2": return f2[num - 1].ToString();
                case "f3": return f3[num - 1].ToString();
                case "f4": return f4[num - 1].ToString();
                case "f5": return f5[num - 1].ToString();
                case "f6": return f6[num - 1].ToString();
                case "f7": return f7[num - 1].ToString();
                case "f8": return f8[num - 1].ToString();
                case "f9": return f9[num - 1].ToString();
                case "f10": return f10[num - 1].ToString();
                case "f11": return f11[num - 1].ToString();
                case "f12": return f12[num - 1].ToString();
            }
            return "";
        }
        int GetMaxLevel(string gid, bool cap)
        {
            switch(gid)
            {
                case  "1": if (cap) return 20; else return 10;
                case  "2": if (cap) return 20; else return 10;
                case  "3": if (cap) return 20; else return 10;
                case  "4": if (cap) return 20; else return 10;
                case  "5": return 5;
                case  "6": return 5;
                case  "7": return 5;
                case  "8": return 5;
                case  "9": return 5;
                case "10": return 20;
                case "11": return 20;
                case "12": return 20;
                case "13": return 20;
                case "14": return 20;
                case "15": return 20;
                case "16": return 20;
                case "17": return 20;
                case "18": return 20;
                case "19": return 20;
                case "20": return 20;
                case "21": return 20;
                case "22": return 20;
                case "23": return 10;
                case "24": return 20;
                case "25": return 20;
                case "26": return 20;
                case "27": return 20;
                case "28": return 20;
                case "29": return 20;
                case "30": return 20;
                case "31": return 20;
                case "32": return 20;
                case "33": return 20;
                case "34": return 20;
                case "35": return 10;
                case "36": return 20;
                case "37": return 20;
                case "38": return 20;
                case "39": return 20;
                case "40": return 100;
                case "41": return 20;
                case "42": return 20;
                default: return 0;
            }
        }
        string GetGidName(string gid)
        {
            switch(gid)
            {
                case "1": return "Лесопилка";
                case "2": return "Глиняный карьер";
                case "3": return "Железный рудник";
                case "4": return "Ферма";
                case "5": return "Лесопильный завод";
                case "6": return "Кирпичный завод";
                case "7": return "Чугунолитейный завод";
                case "8": return "Мукомольная мельница";
                case "9": return "Пекарня";
                case "10": return "Склад";
                case "11": return "Амбар";
                case "12": return "Кузница оружия";
                case "13": return "Кузница доспехов";
                case "14": return "Арена";
                case "15": return "Главное здание";
                case "16": return "Пункт сбора";
                case "17": return "Рынок";
                case "18": return "Посольство";
                case "19": return "Казарма";
                case "20": return "Конюшня";
                case "21": return "Мастерская";
                case "22": return "Акадения";
                case "23": return "Тайник";
                case "24": return "Ратуша";
                case "25": return "Резиденция";
                case "26": return "Дворец";
                case "27": return "Сокровищница";
                case "28": return "Торговая палата";
                case "29": return "Большая казарма";
                case "30": return "Большая конюшня";
                case "31": return "Городская стена";
                case "32": return "Земляной вал";
                case "33": return "Изгородь";
                case "34": return "Камнетес";
                case "35": return "Пивоварня";
                case "36": return "Капканщик";
                case "37": return "Таверна";
                case "38": return "Большой склад";
                case "39": return "Большой амбар";
                case "40": return "Чудо Света";
                case "41": return "Водопой";
            }
            return "ERR";
        }
        #endregion
        #region OTHER
        private void TC_main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //switch(TC_main.SelectedIndex)
            //{
            //    case 0:
            //        {
            //            //currMainTab = 0;
            //        }
            //        break;
            //    case 1:
            //        {
            //           // if (currMainTab == 1) break;
            //            //currMainTab = 1;
            //            //int lastusedvill = TIB_LBV.SelectedIndex;
            //            //TIB_LBV.Items.Clear();
            //            //for (int i = 0; i < vills.Count;i++)
            //            //    TIB_LBV.Items.Add(vills[i].name);
            //            //if (TIB_LBV.Items.Count > lastusedvill)
            //            //    TIB_LBV.SelectedIndex = lastusedvill;
            //            //else
            //            //    TIB_LBV.SelectedIndex = 0;
            //        }
            //        break;
            //}
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
        private void CHBPWB_Checked(object sender, RoutedEventArgs e)
        {
            if (CHBPWB.IsChecked == true)
            {
                WB1.Visible = true;
                WBHost1.Visibility = System.Windows.Visibility.Visible;
                WB.Visible = false;
                WBHost.Visibility = System.Windows.Visibility.Hidden;
                WB1.Navigate(Server + "dorf1.php");
            }
            else
            {
                WB1.Visible = false;
                WBHost1.Visibility = System.Windows.Visibility.Hidden;
                WB.Visible = true;
                WBHost.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void CHBPWB_Click(object sender, RoutedEventArgs e)
        {
            if (CHBPWB.IsChecked == true)
            {
                WB1.Visible = true;
                WBHost1.Visibility = System.Windows.Visibility.Visible;
                WB.Visible = false;
                WBHost.Visibility = System.Windows.Visibility.Hidden;
                WB1.Navigate(Server + "dorf1.php");
            }
            else
            {
                WB1.Visible = false;
                WBHost1.Visibility = System.Windows.Visibility.Hidden;
                WB.Visible = true;
                WBHost.Visibility = System.Windows.Visibility.Visible;
            }
        }
        #endregion
    }
}