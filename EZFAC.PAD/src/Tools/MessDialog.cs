using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace EZFAC.PAD.src.Tools
{
    class MessDialog
    {
        /*
         *弹出框
         * @param 弹出内容 
         */
        public async void showDialog(String contnet)
        {
            ContentDialog dialog = new ContentDialog()
            {
                FontSize = 32d,
                Content = contnet,
                PrimaryButtonText = "确定",
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch
            };
            await dialog.ShowAsync();
        }
    }
}
