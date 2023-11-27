using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Icon转换器
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SavetoIcon(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            img1.Source = new BitmapImage(new Uri(fileName, UriKind.Absolute));
            var img = Image.FromFile(fileName);
            var width = 0;
            var height = 0;
            if (rb0.IsChecked == true)
            {
                width = 0;
                height = 0;
            }
            else if (rb1.IsChecked == true)
            {
                width = 32;
                height = 32;
            }
            else if (rb2.IsChecked == true)
            {
                width = 64;
                height = 64;
            }
            else if (rb3.IsChecked == true)
            {
                width = 128;
                height = 128;
            }
            else if (rb4.IsChecked == true)
            {
                width = 256;
                height = 256;
            }
            else if (rb5.IsChecked == true)
            {
                width = img.Width / 2;
                height = img.Height / 2;
            }
            else if (rb6.IsChecked == true)
            {
                width = img.Width / 4;
                height = img.Height / 4;
            }
            else if (rb7.IsChecked == true)
            {
                width = img.Width / 10;
                height = img.Height / 10;
            }
            else if (rb8.IsChecked == true)
            {
                width = 256;
                height = img.Height * 256 / img.Width;
            }
            else if (rb9.IsChecked == true)
            {
                width = 128;
                height = img.Height * 128 / img.Width;
            }
            else if (rb10.IsChecked == true)
            {
                width = 64;
                height = img.Height * 64 / img.Width;
            }
            else if (rb11.IsChecked == true)
            {
                width = 32;
                height = img.Height * 32 / img.Width;
            }
            else if (rb12.IsChecked == true)
            {
                width = img.Width * 256 / img.Height;
                height = 256;
            }
            else if (rb13.IsChecked == true)
            {
                width = img.Width * 128 / img.Height;
                height = 128;
            }
            else if (rb14.IsChecked == true)
            {
                width = img.Width * 64 / img.Height;
                height = 64;
            }
            else if (rb15.IsChecked == true)
            {
                width = img.Width * 32 / img.Height;
                height = 32;
            }
            IconTool.SaveToIcon(img, new System.Drawing.Size(width, height), fileName.Replace(ext, ".ico"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "图片|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dlg.ShowDialog() == true)
            {
                SavetoIcon(dlg.FileName);
            }
            else
            {
                img1.Source = null;
            }
        }

        private void Button_Drop(object sender, DragEventArgs e)
        {
            try
            {
                var fileName = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                var ext = Path.GetExtension(fileName);
                if(!ext.Equals(".jpg") && !ext.Equals(".jpeg") && !ext.Equals(".png") && !ext.Equals(".bmp"))
                {
                    img1.Source = null;
                    MessageBox.Show("请拖入如下格式文件*.jpg;*.jpeg;*.png;*.bmp", "提示");
                    return;
                }

                SavetoIcon(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void rb0_Checked(object sender, RoutedEventArgs e)
        {
            if (img1.Source == null) return;
            var fileName = ((BitmapImage)img1.Source).UriSource.LocalPath;
            SavetoIcon(fileName);
        }
    }

    /// <summary>
    /// Icon操作类，实现了将Image图像向Icon的转化
    /// 
    /// 调用示例：
    /// Image pic = Image.FromFile(@"D:/tmp/2.png");
    /// IconTool.SaveToIcon(pic, @"D:/tmp/test2.ico");
    /// </summary>
    public class IconTool
    {
        /// <summary>
        /// Icon图像信息类
        /// </summary>
        public class IconInfo
        {
            public ushort Width = 16;         // 图像宽度
            public ushort Height = 16;        // 图像高度
            public ushort ColorNum = 0;       // 图像中的颜色数
            public ushort Reserved = 0;       // 保留字
            public ushort Planes = 1;       // 为目标设备说明位面数
            public ushort PixelBit = 32;    // 每个像素素所占位数

            public uint ImageSize = 0;      // 图像字节大小
            public uint ImageOffset = 0;    // 图形数据起点偏移位置

            public byte[] ImageData;        // 图形数据

            /// <summary>
            /// 创建默认的Icon图像数据结构
            /// </summary>
            public IconInfo() { }
        }

        /// <summary>
        /// 从pic创建Icon信息, 生成Icon的尺寸为rect
        /// </summary>
        public static IconInfo CreatIconInfo(Image pic, Rectangle rect)
        {
            if (rect.Width == 0)
                rect = new Rectangle(0, 0, pic.Width, pic.Height);
            // 创建最适尺寸的图像
            Bitmap IconBitmap = new Bitmap(rect.Width, rect.Height);
            Graphics g = Graphics.FromImage(IconBitmap);
            g.DrawImage(pic, rect, new Rectangle(0, 0, pic.Width, pic.Height), GraphicsUnit.Pixel);
            g.Dispose();

            // 以位图的形式保存到内存流中
            MemoryStream memoryStream = new MemoryStream();
            IconBitmap.Save(memoryStream, ImageFormat.Bmp);

            // 从位图创建Icon属性
            IconInfo iconInfo1 = new IconInfo();
            iconInfo1.Width = (ushort)rect.Width;
            iconInfo1.Height = (ushort)rect.Height;

            // 获取图形数据
            memoryStream.Position = 14;
            iconInfo1.ImageData = new byte[memoryStream.Length - memoryStream.Position];
            memoryStream.Read(iconInfo1.ImageData, 0, iconInfo1.ImageData.Length);

            // Icon图像的高是BMP的2倍
            byte[] Height = BitConverter.GetBytes((uint)iconInfo1.Height * 2);
            iconInfo1.ImageData[8] = Height[0];
            iconInfo1.ImageData[9] = Height[1];
            iconInfo1.ImageData[10] = Height[2];
            iconInfo1.ImageData[11] = Height[3];

            iconInfo1.ImageSize = (uint)iconInfo1.ImageData.Length;
            iconInfo1.ImageOffset = 6 + (uint)(1 * 16);

            return iconInfo1;
        }

        /// <summary>
        /// 保存pic为Icon图像,保存文件路径名称FileName
        /// </summary>
        public static void SaveToIcon(Image pic, string FileName)
        {
            SaveToIcon(pic, Rectangle.Empty, FileName);
        }

        /// <summary>
        /// 保存pic为Icon图像,尺寸Size，保存文件路径名称PathName
        /// </summary>
        public static void SaveToIcon(Image pic, System.Drawing.Size size, string PathName)
        {
            SaveToIcon(pic, new Rectangle(0, 0, size.Width, size.Height), PathName);
        }

        /// <summary>
        /// 保存pic为Icon图像,尺寸rect，保存文件路径名称PathName
        /// </summary>
        public static void SaveToIcon(Image pic, Rectangle rect, string PathName)
        {
            // 获取Icon信息
            IconInfo iconInfo = CreatIconInfo(pic, rect);
            // 创建文件输出流，写入文件，生成Icon图像
            using (FileStream stream = new FileStream(PathName, FileMode.Create))
            {
                // 写入Icon固定部分
                ushort Reserved = 0;
                ushort Type = 1;
                ushort Count = 1;
                byte[] Temp = BitConverter.GetBytes(Reserved);
                stream.Write(Temp, 0, Temp.Length);
                Temp = BitConverter.GetBytes(Type);
                stream.Write(Temp, 0, Temp.Length);
                Temp = BitConverter.GetBytes(Count);
                stream.Write(Temp, 0, Temp.Length);

                //stream.WriteByte((byte)(iconInfo.Width < 256 ? iconInfo.Width : 0));
                //stream.WriteByte((byte)(iconInfo.Height < 256 ? iconInfo.Height : 0));
                stream.WriteByte((byte)iconInfo.Width);
                stream.WriteByte((byte)iconInfo.Height);
                stream.WriteByte((byte)iconInfo.ColorNum);
                stream.WriteByte((byte)iconInfo.Reserved);

                Temp = BitConverter.GetBytes(iconInfo.Planes);
                stream.Write(Temp, 0, Temp.Length);
                Temp = BitConverter.GetBytes(iconInfo.PixelBit);
                stream.Write(Temp, 0, Temp.Length);
                Temp = BitConverter.GetBytes(iconInfo.ImageSize);
                stream.Write(Temp, 0, Temp.Length);
                Temp = BitConverter.GetBytes(iconInfo.ImageOffset);
                stream.Write(Temp, 0, Temp.Length);
                // 写入图形数据
                stream.Write(iconInfo.ImageData, 0, iconInfo.ImageData.Length);
                stream.Close();
            }
        }
    }
}
