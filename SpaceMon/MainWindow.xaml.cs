using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;

namespace SpaceMon
{
    public struct DriveFreeSpace
    {
        private double signedPow(double n, double e)
        {
            return Math.Sign(n) * Math.Pow(Math.Abs(n), e);
        }
        public string driveLetter { get; set; }
        public Int64 freeBytes { get; set; }
        public double freeBytesLoggy
        {
            get
            {
                double megs = (double)freeBytes / 1000000.0;
                return megs < 232.1 ?
                    Math.Pow(megs,1.1)/101.1
                   : (
                   megs > 1000 ?
                   signedPow(2.0 * Math.Log10(megs) - 6.0, 2.3)*0.5 + 6
                   : Math.Pow(2.0*Math.Log10(megs) -6.0,3.0)+6
                   ); // this is not mathematically perfectly continuous or anything, it just looks ok that's all
            }
        }
        public double freeBytesHeight
        {
            get
            {
                return 25.0/6.0*freeBytesLoggy;
            }
        }
        public int freeBytesXXX
        {
            get
            {
                Int64 byteCount = freeBytes;
                while(byteCount >= 1000)
                {
                    byteCount /= 1000;
                }
                return (int)byteCount;
            }
        }
        public SolidColorBrush color
        {
            get
            {
                if (freeBytes < 100_000_000)
                {
                    return new SolidColorBrush(Color.FromRgb(255,0,0));
                }
                else if (freeBytes < 1000_000_000)
                {
                    return new SolidColorBrush(Color.FromRgb(255, 32, 0));
                }
                else if (freeBytes < 3000_000_000)
                {
                    return new SolidColorBrush(Color.FromRgb(255, 64, 0));
                }
                else if (freeBytes < 4000_000_000)
                {
                    return new SolidColorBrush(Color.FromRgb(255, 128, 0));
                }
                else if (freeBytes < 5000_000_000)
                {
                    return new SolidColorBrush(Color.FromRgb(255, 255, 0));
                }
                else if (freeBytes < 10_000_000_000)
                {
                    return new SolidColorBrush(Color.FromRgb(128, 255, 0));
                }
                else if (freeBytes < 25_000_000_000)
                {
                    return new SolidColorBrush(Color.FromRgb(64, 255, 0));
                }
                else
                {
                    return new SolidColorBrush(Color.FromRgb(0, 255, 0));
                }
            }
        }
        public string freeBytesXXXUnit
        {
            get
            {
                if (freeBytes < 1000)
                {
                    return "B";
                } else if (freeBytes < 1_000_000)
                {
                    return "K";
                }else if (freeBytes < 1_000_000_000)
                {
                    return "M";
                }else if (freeBytes < 1_000_000_000_000)
                {
                    return "G";
                }else if (freeBytes < 1_000_000_000_000_000)
                {
                    return "P";
                } else
                {
                    return "?";//idk. nobody has that much free space wtf.
                }
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        private const int WWS_EX_TRANSPARENT = 0x00000020;
        private const int GWL_EXSTYLE = (-20);



        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            //MakeWindowUnclickable();
            this.MouseDown += MainWindow_MouseDown;
            SetFreeSpaceInfo();
            startLoop();
        }

        private void startLoop()
        {

            Task.Factory.StartNew(()=> { Loop(); },new CancellationToken(), TaskCreationOptions.LongRunning, TaskScheduler.Default).ContinueWith((a)=> {
                MessageBox.Show(a.Exception.ToString());
                System.Threading.Thread.Sleep(2000);
                startLoop();
            },TaskContinuationOptions.OnlyOnFaulted);
        }

        private void Loop()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(5000);
                SetFreeSpaceInfo();
            }
        }

        private void SetFreeSpaceInfo()
        {
            var drives = DriveInfo.GetDrives();
            List<DriveFreeSpace> driveInfos = new List<DriveFreeSpace>();
            foreach(var drive in drives)
            {
                if(drive.DriveType != DriveType.CDRom && drive.IsReady)
                {
                    try
                    {
                        driveInfos.Add(new DriveFreeSpace() { driveLetter = drive.Name.Substring(0, 1), freeBytes = drive.TotalFreeSpace });
                    }
                    catch(Exception e)
                    {
                        // can happen i guess if it goes from ready to not ready quickly?
                        Debug.WriteLine(e.ToString());
                    }
                }
            }
            Dispatcher.Invoke(()=> {
                listDrives.ItemsSource = driveInfos.ToArray();
            });
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            MakeWindowUnclickable();
        }

        private ICommand unlockCommand = null;
        public ICommand UnlockCommand
        {
            get
            {
                return unlockCommand ?? (unlockCommand = new DoSomethingCommand(() => { MakeWindowClickable(); }));
            }
        }

        private void MakeWindowUnclickable()
        {
            try
            {

                IntPtr hwhandle = new WindowInteropHelper(this).Handle;
                int currentValue = GetWindowLong(hwhandle, GWL_EXSTYLE);
                SetWindowLong(hwhandle, GWL_EXSTYLE,currentValue | WWS_EX_TRANSPARENT);
            } catch(Exception e)
            {

            }

        }
        private void MakeWindowClickable()
        {
            try
            {

                IntPtr hwhandle = new WindowInteropHelper(this).Handle;
                int currentValue = GetWindowLong(hwhandle, GWL_EXSTYLE);
                SetWindowLong(hwhandle, GWL_EXSTYLE,currentValue & ~WWS_EX_TRANSPARENT);
            } catch(Exception e)
            {

            }

        }
    }




    public class DoSomethingCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action stuffToDo = null;

        public DoSomethingCommand(Action stuffToDoA)
        {
            if (stuffToDoA == null)
            {
                throw new InvalidOperationException("DoSomethingCommand must be passed a non-null Action");
            }
            stuffToDo = stuffToDoA;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            stuffToDo();
        }
    }
}
