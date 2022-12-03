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
                //�����¼��ť
                await Page.GetByRole(AriaRole.Link, new() { NameString = "��¼" })
                    .Filter(new() { HasTextString = "��¼" }).ClickAsync();

                //���΢�ŵ�¼��ť, ���ҵȴ�, ֱ����ά���ȡ���
                await Page.RunAndWaitForRequestAsync(async () =>
                {
                    await Page.GetByRole(AriaRole.Link, new() { NameString = "΢��" }).ClickAsync();
                }, new Regex(".*qrcode.*"));

                //ҳ���ͼ
                string screenShotPath = $"LoginQRCode{DateTime.Now.Ticks}.png";
                await Page.ScreenshotAsync(new PageScreenshotOptions() { Path = screenShotPath, FullPage = true });

                //�򿪽�ͼ�ȴ�ɨ��
                string qrCodeFilePath = $"{ Directory.GetCurrentDirectory()}\\{screenShotPath}";
                OpenQRCodeAsync(qrCodeFilePath);

                //�ȴ�ɨ��ɹ���,�ص�NET Conf 2022 �ҳ��
                await Page.WaitForURLAsync("https://www.huodongxing.com/event/4674917451700", 
                    new PageWaitForURLOptions() { Timeout = 0 });

                //ͨ���û���Ϣ������Ƿ��¼�ɹ�
                var currentUserInfo = Page.GetByRole(AriaRole.Link, new() { NameString = "Hi ����" });
                await Expect(currentUserInfo).ToBeVisibleAsync();
            }

            public override BrowserNewContextOptions ContextOptions()
            {
                //������Ƶ����Ŀ¼
                return new BrowserNewContextOptions() { RecordVideoDir = "video" };
            }

            /// <summary>
            /// �򿪶�ά��ͼƬ
            /// </summary>
            /// <param name="qrCodePath"></param>
            /// <returns></returns>
            private void OpenQRCodeAsync(string qrCodeFilePath)
            {
                //�����µ�ϵͳ���� 
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                //�����ļ������˴�ΪͼƬ����ʵ·��+�ļ��� 
                process.StartInfo.FileName = qrCodeFilePath;
                //��Ϊ�ؼ����֡����ý������в�������ʱΪ��󻯴�����ʾͼƬ�� 
                process.StartInfo.Arguments = "rundll32.exe %systemroot%\\system32\\shimgvw.dll,ImageView_Fullscreen";
                //����Ϊ�Ƿ�ʹ��Shellִ�г�����ϵͳĬ��Ϊtrue������Ҳ�ɲ��裬�������ñ���Ϊtrue 
                process.StartInfo.UseShellExecute = true;
                //�˴����Ը��Ľ������򿪴������ʾ��ʽ�����Բ��� 
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.Start();
                process.Close();
            }
        }
    }
}