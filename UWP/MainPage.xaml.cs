using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace MilkcocoaSampleApp
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Milkcocoa.Milkcocoa Milkcocoa;

        private bool _IsConnect;
        private bool IsConnect
        {
            get
            {
                return _IsConnect;
            }
            set
            {
                if (!value)
                {
                    txtAppId.IsEnabled = true;
                    btnConnect.IsEnabled = true;
                    btnDisconnect.IsEnabled = false;

                    txtPubPath.IsEnabled = false;
                    txtPubValue.IsEnabled = false;
                    btnPubPush.IsEnabled = false;
                    btnPubSend.IsEnabled = false;

                    txtSubPath.IsEnabled = false;
                    btnSubSubscribe.IsEnabled = false;
                    btnSubUnsubscribe.IsEnabled = false;
                }
                else
                {
                    txtAppId.IsEnabled = false;
                    btnConnect.IsEnabled = false;
                    btnDisconnect.IsEnabled = true;

                    txtPubPath.IsEnabled = true;
                    txtPubValue.IsEnabled = true;
                    btnPubPush.IsEnabled = true;
                    btnPubSend.IsEnabled = true;

                    IsSubscribe = false;
                }

                _IsConnect = value;
            }
        }

        private bool _IsSubscribe;
        private bool IsSubscribe
        {
            get
            {
                return _IsSubscribe;
            }
            set
            {
                if (!value)
                {
                    txtSubPath.IsEnabled = true;
                    btnSubSubscribe.IsEnabled = true;
                    btnSubUnsubscribe.IsEnabled = false;
                }
                else
                {
                    txtSubPath.IsEnabled = false;
                    btnSubSubscribe.IsEnabled = false;
                    btnSubUnsubscribe.IsEnabled = true;
                }

                _IsSubscribe = value;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IsConnect = false;
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            Milkcocoa = new Milkcocoa.Milkcocoa(txtAppId.Text + ".mlkcca.com");

            IsConnect = true;
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (Milkcocoa != null)
            {
                Milkcocoa.Dispose();
                Milkcocoa = null;
            }

            IsConnect = false;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Milkcocoa != null)
            {
                Milkcocoa.Dispose();
                Milkcocoa = null;
            }
        }

        private void btnPubPush_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("{0:HH:mm:ss.fff} Push {1}", DateTime.Now, txtPubValue.Text));

            Milkcocoa.dataStore(txtPubPath.Text).push(new { content = txtPubValue.Text });
        }

        private void btnPubSend_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("{0:HH:mm:ss.fff} Send {1}", DateTime.Now, txtPubValue.Text));

            Milkcocoa.dataStore(txtPubPath.Text).send(new { content = txtPubValue.Text });
        }

        private void btnSubSubscribe_Click(object sender, RoutedEventArgs e)
        {
            Milkcocoa.dataStore(txtSubPath.Text).on("push", MilkcocoaCallback);
            Milkcocoa.dataStore(txtSubPath.Text).on("send", MilkcocoaCallback);

            IsSubscribe = true;
        }

        private void btnSubUnsubscribe_Click(object sender, RoutedEventArgs e)
        {
            Milkcocoa.dataStore(txtSubPath.Text).off("push");
            Milkcocoa.dataStore(txtSubPath.Text).off("send");

            IsSubscribe = false;
        }

        private void MilkcocoaCallback(Milkcocoa.MilkcocoaDataStoreEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("{0:HH:mm:ss.fff} Sub  {1}", DateTime.Now, e.path));

            var dummy = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lstSubValues.Items.Insert(0, string.Format("{0:HH:mm:ss.fff} {1}", DateTime.Now, e.value.content));
                lstSubValues.SelectedIndex = 0;
            });
        }
    }
}
