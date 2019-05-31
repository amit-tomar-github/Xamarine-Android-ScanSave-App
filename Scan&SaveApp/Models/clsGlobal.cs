using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;
using System.Net.Http;
using System.Text;
using IOCLAndroidApp.Models;
using System.Collections.Generic;
using System.IO;
using Android.Content.PM;
using Android.Views;
using Android.Net;
using System.Net.Sockets;

namespace IOCLAndroidApp
{
    public enum MessageTitle
    {
        ERROR = 1,
        CONFIRM = 2,
        INFORMATION = 3,
    }

    public class clsGlobal : Activity
    {
        public static string FileFolder = "ScanFiles";
        public static string CaseFileName = "Barcode.csv";
        public static string ServerIpFileName = "ServerIP.txt";
        public static string FilePath = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
        public static TcpClient mTcpClient;
        internal static string mSockIp = "";
        internal static Int32 mSockPort = 0;

        //***************************************
        public void ShowMessage(string msg, Activity activity, MessageTitle MsgTitle)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(activity);
            builder.SetTitle(MsgTitle.ToString());
            builder.SetMessage(msg);
            builder.SetCancelable(false);
            builder.SetPositiveButton("OK", delegate { Finish(); });
            builder.Show();
        }
    }
}
