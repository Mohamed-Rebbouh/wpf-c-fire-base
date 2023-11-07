using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System.Net;
using Microsoft.Win32;
using System.Windows.Markup;
using System.Security.Cryptography;

namespace education
{
    /// <summary>
    /// Logique d'interaction pour homewin.xaml
    /// </summary>
    public partial class homewin : Window
    {
        int state;
        
        string[] tab = { "student", "halper", "prof" };
        static data d = new data();
        private bool ismax = false;
        string prec;

        IFirebaseConfig con = new FirebaseConfig()
        {
            AuthSecret = "xse0qTokr0TpZ2dE8JaMJ9aFI1OCxPwnsmLVMDAO",
            BasePath= "https://itc-mnagr-default-rtdb.firebaseio.com/"

        };
        IFirebaseClient client;
        public homewin()
        {
              client = new FirebaseClient(con);
              InitializeComponent();
              star();
           
           
            
           
        }



        private void addbtn_Click(object sender, RoutedEventArgs e)
        {
            switch (titl_page.Text) 
            {
                case "home":
                addcours w = new addcours();
                w.ShowDialog();
                    break;
                    default: 
                    break;
                case "Coumunity":
                    sqtwin we=new sqtwin();
                    we.ShowDialog();
                    break;
                    
             }
        }

        private void gridload(string path1,string pathe2 )
        {
            Dictionary<string, fil> list = new Dictionary<string, fil>();
            ObservableCollection<fil> fils = new ObservableCollection<fil>();

            
            if(!string.IsNullOrEmpty(d.spec) && !string.IsNullOrEmpty(d.year))
            {
                FirebaseResponse re = client.Get(@""+ d.spec + "/" + d.year+"/"+path1+"/"+pathe2);
                list = JsonConvert.DeserializeObject<Dictionary<string, fil>>(re.Body.ToString());
                try
                {
                    foreach (var item in list)
                    {
                        if (item.Value is not null)
                        {
                            fils.Add(item.Value);

                        }
                    }
                    gridcours.ItemsSource = fils;
                }catch(Exception ex) { MessageBox.Show(ex.Message); }

            }
         
        }

   

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            dockcours.Visibility = Visibility.Hidden;
            dockqst.Visibility =    Visibility.Hidden;
            Homepnl.Visibility = Visibility.Visible;
            first();
            NameT.Text = d.name;


        }
        private void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                fil ri= (fil)gridcours.SelectedItem;
                
               
                if (ri != null)
                {
                    string url = ri.url.ToString();
                    SaveFileDialog svd = new SaveFileDialog();
                    svd.ShowDialog();
                    if (!string.IsNullOrEmpty(svd.FileName))
                    {
                        WebClient web = new WebClient();
                        web.DownloadFile(url, svd.FileName + ".pdf");
                        MessageBox.Show("downlod succec");
                    }
                    else
                    {
                        MessageBox.Show("file name can not be empty!!");
                    }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
            
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void max_btn_Click(object sender, RoutedEventArgs e)
        {
            
            if (!ismax)
            {
                this.WindowState= WindowState.Maximized;
                ismax= true;
            }
            else
            {
                this.WindowState= WindowState.Normal;
                 Width = 1080;
                 Height = 720;

                ismax= false;
            }
        }

        private void star()
        {
            
            switch (d.post)
            {
                case "student":
                    addbtn.Visibility = Visibility.Hidden;
                    break;
                default:
                    addbtn.Visibility = Visibility.Visible;
                    break;

            }
            dockqst.Visibility = Visibility.Hidden;
            dockcours.Visibility = Visibility.Hidden;

            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            dockcours.Visibility = Visibility.Hidden;
            dockqst.Visibility = Visibility.Hidden;
            Homepnl.Visibility = Visibility.Visible;
            Homepnl.Children.Clear();
            titl_page.Text = "home";
            state = 0;
            Homepnl.Children.Clear();
            first();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            gridqstload();
            dockqst.Visibility = Visibility.Visible;
            dockcours.Visibility = Visibility.Hidden;
            Homepnl.Visibility = Visibility.Hidden;

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow();
            this.Close();
            win.Show();
        }

        private void gridqstload()
        {
            dockqst.Visibility = Visibility.Visible;
            dockcours.Visibility = Visibility.Hidden;
            addbtn.Visibility = Visibility.Visible;
            titl_page.Text = "Coumunity";

            ObservableCollection<qst> fils = new ObservableCollection<qst>();
            FirebaseResponse re = client.Get(@"qst");   
            Dictionary<string, qst> list = JsonConvert.DeserializeObject<Dictionary<string, qst>>(re.Body.ToString());
            try
            {
                foreach (var item in list)
                {
                    if (item.Value is not null)
                    {
                        fils.Add(item.Value);

                    }



                }
                gridqst.ItemsSource = fils;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public void set(string n,string s,string p,string y)
        {
            d.name = n;
            d.spec= s;  
            d.post= p;
            d.year= y;
            d.password = "";
            


        }

        public void first()
        {
           
            FirebaseResponse re = client.Get(@""+d.spec+"/"+d.year);
            Dictionary<string, fil> list = JsonConvert.DeserializeObject<Dictionary<string,fil>>(re.Body.ToString());
            try
            {
                foreach (var item in list)
                {
                    Style s = (Style)FindResource("borderhome");
                    Button b = new Button();
                    b.Style = s;
                    b.Content = item.Key;
                    b.Click += (sender, e) => { enter(sender, e); };
                    b.FontSize = 40;
                    b.Foreground = NameT.Foreground;
                    Homepnl.Children.Add(b);



                }
            }catch(Exception ex) { MessageBox.Show(ex.Message); }





        }

      

        public void enter(object sender, RoutedEventArgs e)
        {
            if (state == 0)
            {
                Button be = (Button)sender;
                if(be is not null)
                prec = be.Content.ToString();
                FirebaseResponse re = client.Get(@"" + d.spec + "/" + d.year+"/"+be.Content.ToString());
                Dictionary<string, fil> list = JsonConvert.DeserializeObject<Dictionary<string, fil>>(re.Body.ToString());
                Homepnl.Children.Clear();
                foreach (var item in list)
                {
                    Style s = (Style)FindResource("borderhome");
                    Button b = new Button();
                    b.Style = s;
                    b.Content = item.Key;
                    b.FontSize = 40;
                    b.Click += (sender, e) => { enter(sender, e); };
                    b.Foreground = NameT.Foreground;
                    Homepnl.Children.Add(b);



                }


                state++;

            }

            else if(state == 1) 
            {
                Homepnl.Children.Clear();
                Button be = (Button)sender;
                gridload(prec,be.Content.ToString());
                Homepnl.Visibility = Visibility.Hidden;
                dockqst.Visibility=Visibility.Hidden;
                dockcours.Visibility= Visibility.Visible;
                Homepnl.Children.Clear();
            }

            



        }
    }
}
