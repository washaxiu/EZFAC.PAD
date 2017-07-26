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
                Content = contnet,
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消"
            };
            await dialog.ShowAsync();
        }

        
    }
}
