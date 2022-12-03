using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace NUnitTestByNet
{
    public class Tests
    {
        [Parallelizable(ParallelScope.Self)]
        [TestFixture]
        public class Tests01 : PageTest
        {
            [Test]
            public async Task TestLoginInNETConfChina2022Page()
            {
                await Page.GotoAsync("https://www.huodongxing.com/event/4674917451700");
                //点击登录按钮
                await Page.GetByRole(AriaRole.Link, new() { NameString = "登录" })
                    .Filter(new() { HasTextString = "登录" }).ClickAsync();

                //点击微信登录按钮, 并且等待, 直到二维码获取完成
                await Page.RunAndWaitForRequestAsync(async () =>
                {
                    await Page.GetByRole(AriaRole.Link, new() { NameString = "微信" }).ClickAsync();
                }, new Regex(".*qrcode.*"));

                //页面截图
                string screenShotPath = $"LoginQRCode{DateTime.Now.Ticks}.png";
                await Page.ScreenshotAsync(new PageScreenshotOptions() { Path = screenShotPath, FullPage = true });

                //打开截图等待扫码
                string qrCodeFilePath = $"{ Directory.GetCurrentDirectory()}\\{screenShotPath}";
                OpenQRCodeAsync(qrCodeFilePath);

                //等待扫码成功后,回到NET Conf 2022 活动页面
                await Page.WaitForURLAsync("https://www.huodongxing.com/event/4674917451700", 
                    new PageWaitForURLOptions() { Timeout = 0 });

                //通过用户信息来诊断是否登录成功
                var currentUserInfo = Page.GetByRole(AriaRole.Link, new() { NameString = "Hi 阿白" });
                await Expect(currentUserInfo).ToBeVisibleAsync();
            }

            public override BrowserNewContextOptions ContextOptions()
            {
                //设置视频保存目录
                return new BrowserNewContextOptions() { RecordVideoDir = "video" };
            }

            /// <summary>
            /// 打开二维码图片
            /// </summary>
            /// <param name="qrCodePath"></param>
            /// <returns></returns>
            private void OpenQRCodeAsync(string qrCodeFilePath)
            {
                //建立新的系统进程 
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                //设置文件名，此处为图片的真实路径+文件名 
                process.StartInfo.FileName = qrCodeFilePath;
                //此为关键部分。设置进程运行参数，此时为最大化窗口显示图片。 
                process.StartInfo.Arguments = "rundll32.exe %systemroot%\\system32\\shimgvw.dll,ImageView_Fullscreen";
                //此项为是否使用Shell执行程序，因系统默认为true，此项也可不设，但若设置必须为true 
                process.StartInfo.UseShellExecute = true;
                //此处可以更改进程所打开窗体的显示样式，可以不设 
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.Start();
                process.Close();
            }
        }
    }
}