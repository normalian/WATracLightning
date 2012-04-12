using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.IO;

namespace WorkerRole1
{
    // Trac Lightning は InnoSetup を利用しているので、以下を参照する
    // https://twitter.com/#!/LightningX/status/177054298348326912

    public class WorkerRole : RoleEntryPoint
    {
        private CloudDrive azureDrive;

        public override void Run()
        {
            Trace.WriteLine("$projectname$ entry point called", "Information");


            // Trac Lightning のセットアップ／実行スクリプトをキック
            // Path.Combine() メソッドを利用するとエラーが出る。。。
            string exe = Environment.GetEnvironmentVariable("RoleRoot") + @"\approot\setuptrac.cmd";
            var proc = new Process();
            var procStartInfo = new ProcessStartInfo();

            procStartInfo.FileName = exe;
            procStartInfo.UseShellExecute = false;
            proc.StartInfo = procStartInfo;
            proc.Start();

            while (true)
            {
                Thread.Sleep(10000);
                Trace.WriteLine("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            // 構成設定パブリッシャの初期設定
            CloudStorageAccount.SetConfigurationSettingPublisher(
                (configName, configSettingPublisher) =>
                {
                    var connectionString = RoleEnvironment.GetConfigurationSettingValue(configName);
                    configSettingPublisher(connectionString);
                }
            );
            CloudStorageAccount storageAccount = CloudStorageAccount.FromConfigurationSetting("StorageConnectionString");


            // ローカルキャッシュの設定
            LocalResource localCache = RoleEnvironment.GetLocalResource("LocalStorage1");


            // 【#1】Windows Azure ドライブのマウント
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("trac");
            azureDrive = storageAccount.CreateCloudDrive(blobContainer.GetPageBlobReference("disk.vhd").Uri.ToString());
            string driveLetter = azureDrive.Mount(0, DriveMountOptions.Force);


            // 固定用ドライブレターを割り当てる
            // 今回は Z: ドライブに再接続
            string diskpartFile = Path.Combine(localCache.RootPath, "diskpart.txt");
            string diskpartCommand = string.Format("select volume = {0} {1} assign letter = {2}", driveLetter, Environment.NewLine, "Z:");
            File.WriteAllText(diskpartFile, diskpartCommand);
            string exe = Path.Combine(System.Environment.GetEnvironmentVariable("WINDIR"), @"System32\diskpart.exe");

            //Diskpartの実行
            Process p = new Process();

            p.StartInfo.FileName = exe;
            p.StartInfo.Arguments = "/S " + diskpartFile;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.Start();

            string results = p.StandardError.ReadToEnd();

            p.WaitForExit(60000);
            p.Close();

            return base.OnStart();
        }


        public override void OnStop()
        {
            // Windows Azure ドライブのアンマウント
            azureDrive.Unmount();

            base.OnStop();
        }
    }
}
