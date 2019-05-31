using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content.PM;
using Android.Views;
using IOCLAndroidApp;
using Android.Content;
using System;
using System.IO;
using SatoScanningApp.ActivityClass;
using System.Collections.Generic;
using Android.Media;

namespace SatoScanningApp
{
    [Activity(Label = "Scan&SaveApp", MainLauncher = true, WindowSoftInputMode = SoftInput.StateHidden, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        clsGlobal clsGLB;
        int _iScanCount = 0;
        Dictionary<string, string> _DicCaseBarcode = new Dictionary<string, string>();

        EditText txtScanCase;
        TextView txtScanCount,txtMsg;
      

        public MainActivity()
        {
            try
            {
                clsGLB = new clsGlobal();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                // Set our view from the "main" layout resource
                SetContentView(Resource.Layout.activity_main);

                txtScanCase = FindViewById<EditText>(Resource.Id.txtScanCase);
                txtScanCase.KeyPress += TxtScanCase_KeyPress;

                txtScanCount = FindViewById<TextView>(Resource.Id.txtScanCount);
                txtMsg = FindViewById<TextView>(Resource.Id.txtMsg);

                //if (ReadSettingFile() == false)
                //    OpenActivity(typeof(SettingActivity));

                txtScanCase.RequestFocus();
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }

        private void TxtScanCase_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.Event.Action == KeyEventActions.Down)
                {
                    if (e.KeyCode == Keycode.Enter)
                    {
                        txtMsg.Text = "";
                        if (!string.IsNullOrEmpty(txtScanCase.Text))
                        {
                            if (!_DicCaseBarcode.ContainsKey(txtScanCase.Text.Trim().ToUpper()))
                            {
                                SaveCase(txtScanCase.Text.Trim());
                                txtMsg.Text = "Scanned successfully!!";
                                txtScanCase.Text = "";
                                txtScanCase.RequestFocus();
                            }
                            else
                            {
                                clsGLB.ShowMessage("Barcode already scanned", this, MessageTitle.INFORMATION);
                                txtScanCase.Text = "";
                                txtScanCase.RequestFocus();
                            }
                        }
                        else
                        {
                            clsGLB.ShowMessage("Scan Barcode", this, MessageTitle.INFORMATION);
                            txtScanCase.RequestFocus();
                        }
                    }
                    else
                        e.Handled = false;
                }
            }
            catch (Exception ex)
            {
                clsGLB.ShowMessage(ex.Message, this, MessageTitle.ERROR);
            }
        }

        #region Methods

        private void SaveCase(string CaseBarcode)
        {
            StreamWriter sw = null;
            try
            {
                string folderPath= Path.Combine(clsGlobal.FilePath, clsGlobal.FileFolder);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filename = Path.Combine(folderPath, clsGlobal.CaseFileName);
                sw = new StreamWriter(filename, true);
                string FileData = CaseBarcode + "," + DateTime.Now.ToString("dd-MMM-yyyy") + "," + DateTime.Now.ToString("HH:mm:ss");
                _DicCaseBarcode.Add(CaseBarcode.ToUpper(), FileData);
                sw.WriteLine(FileData);
                _iScanCount++;
                txtScanCount.Text = "Scan Count : " + _iScanCount;

                MediaScannerConnection.ScanFile(this, new String[] { filename }, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                    sw = null;
                }
            }
        }

        private bool ReadSettingFile()
        {
            StreamReader sr = null;
            try
            {
                string filename = Path.Combine(clsGlobal.FilePath, clsGlobal.ServerIpFileName);

                if (File.Exists(filename))
                {
                    sr = new StreamReader(filename);
                    clsGlobal.mSockIp = sr.ReadLine();
                    clsGlobal.mSockPort = Convert.ToInt32(sr.ReadLine());

                    sr.Close();
                    sr.Dispose();
                    sr = null;

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            { throw ex; }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                    sr = null;
                }
            }
        }

        public void ShowConfirmBox(string msg, Activity activity)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(activity);
            builder.SetTitle("Message");
            builder.SetMessage(msg);
            builder.SetCancelable(false);
            builder.SetPositiveButton("Yes", handllerOkButton);
            builder.SetNegativeButton("No", handllerCancelButton);
            builder.Show();
        }
        void handllerOkButton(object sender, DialogClickEventArgs e)
        {
            this.FinishAffinity();
        }
        void handllerCancelButton(object sender, DialogClickEventArgs e)
        {

        }
        public void OpenActivity(Type t)
        {
            try
            {
                Intent MenuIntent = new Intent(this, t);
                StartActivity(MenuIntent);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public override void OnBackPressed()
        {
            ShowConfirmBox("Do you want to exit", this);
        }
    }
}