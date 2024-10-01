using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XmlFTS;
using XmlFTS.OutClass;
using XMLSigner;

namespace FTS_XML_UI_Netframework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cancellationToken;
        private IHost host;

        private string Header = "Ulitka:)";

        public MainWindow()
        {
            InitializeComponent();

            Config.BaseConfiguration("C:\\Test");
            Config.DeleteSourceFiles = true;
            Config.EnableBackup = true;
            Config.ReadAllSetting();

            var host = new HostBuilder()
                   .ConfigureHostConfiguration(hConfig => { })
                   .ConfigureServices((context, services) =>
                   {
                       services.AddHostedService<FileSystemWatcherService>();
                       //services.AddHostedService<ReplyProcess>();
                   })
                   .Build();
        }

        private async void btn_StartStopProcess_Click(object sender, RoutedEventArgs e)
        {
            await host.StartAsync();
        }
        private async void btn_StopProcess_Click(object sender, RoutedEventArgs e)
        {
            await host.StopAsync();
        }

        private async void menu_Operation_CreateArchive_Click(object sender, RoutedEventArgs e)
        {
            string MchdId = "719f90af-f777-4c70-9a33-053958eacc65";
            string MchdINN = "2536287574";
            X509Certificate2 cert = SignXmlGost.FindGostCurrentCertificate("01DAFCE9BC8E41B000087F5E381D0002");

            await Task.Run(() => TemplatingXml.CreateArchive(MchdId, MchdINN, cert, "NewArch_12"));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /// 
            host = new HostBuilder()
                   .ConfigureHostConfiguration(hConfig => { })
                   .ConfigureServices((context, services) =>
                   {
                       services.AddHostedService<FileSystemWatcherService>();
                       services.AddHostedService<ReplyProcess>();
                   })
                   .Build();
        }

        private void rtb_Logs_TextChanged(object sender, TextChangedEventArgs e)
        {
            rtb_Logs.ScrollToEnd();
        }

        private void btn_TEST_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                rtb_Logs.AppendText($@"{i}" + Environment.NewLine);
            }
        }

        #region RICHTEXTBOX DLL SCROLLABLE
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        //private const int WM_VSCROLL = 277;
        //private const int SB_PAGEBOTTOM = 7;

        //internal static void ScrollToBottom(RichTextBox richTextBox)
        //{
        //    SendMessage(richTextBox.Handle, WM_VSCROLL, (IntPtr)SB_PAGEBOTTOM, IntPtr.Zero);
        //    richTextBox.SelectionStart = richTextBox.Text.Length;
        //}
        #endregion
    }
}
