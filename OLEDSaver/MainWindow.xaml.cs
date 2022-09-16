using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

using WinForms = System.Windows.Forms;
using WinDraw = System.Drawing;
using Dapplo.Windows.User32;
using Dapplo.Windows.User32.Enums;
using System.IO;
using System.Diagnostics;
using System.Windows.Interop;
using System.Drawing;

namespace OLEDSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool Hidden = false;
        List<Form> Overlays;
        int TitleHeight;
        bool TitleEnabled = false;
        const float DefaultOpacity = 0.5f;

        public MainWindow()
        {
            Overlays = new List<Form>();

            InitializeComponent();

            LoadOverlays();

            var bounds = Screen.PrimaryScreen.WorkingArea;
            Graphics graphics = Graphics.FromHwnd(new WindowInteropHelper(this).Handle);
            float scale = graphics.DpiX / 96.0f;
            Left = bounds.X + (bounds.Width / scale) - Width;
            Top = bounds.Y + (bounds.Height / scale) - Height;

            ShowInTaskbar = false;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void LoadOverlays()
        {
            foreach (var form in Overlays)
                form.Close();

            Overlays.Clear();

            OpacitySlider.Value = DefaultOpacity * 10.0f;

            var timer = new Timer()
            {
                Interval = 1000,
            };
            timer.Tick += (object sender, EventArgs e) =>
            {
                bool fullscreen = false;
                IntPtr? handle = WindowTools.IsForegroundFullScreen(Screen.PrimaryScreen);
                if (handle != null)
                {
                    fullscreen = true;

                    if (handle == new WindowInteropHelper(App.Current.MainWindow).Handle)
                        fullscreen = false;
                    else
                    {
                        foreach (var overlay in Overlays)
                        {
                            if (overlay.Handle == handle)
                            {
                                fullscreen = false;
                                break;
                            }
                        }
                    }
                }

                if (fullscreen && !Hidden)
                {
                    foreach (var overlay in Overlays)
                    {
                        overlay.Visible = false;
                    }

                    Hidden = true;
                }
                else if (!fullscreen && Hidden)
                {
                    foreach (var overlay in Overlays)
                    {
                        overlay.Visible = true;
                    }

                    Hidden = false;
                }

                foreach (var overlay in Overlays)
                {
                    overlay.BringToFront();
                }
            };
            timer.Start();


            CreateFullForm();
            //CreateTaskbarForm();
            //CreateMainForm();
            //CreateTitleForm();
        }

        void ApplyHiddenForm(Form form, WinDraw.Rectangle bounds)
        {
            form.BackColor = WinDraw.Color.Black;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Opacity = DefaultOpacity;
            form.TopMost = true;
            form.TopLevel = true;
            form.ShowInTaskbar = false;

            long initialStyle = User32Api.GetWindowLongWrapper(form.Handle, WindowLongIndex.GWL_EXSTYLE);
            User32Api.SetWindowLongWrapper(form.Handle, WindowLongIndex.GWL_EXSTYLE, (IntPtr)(initialStyle | 0x80000 | 0x20));

            form.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }

        void CreateFullForm()
        {
            var bounds = Screen.PrimaryScreen.Bounds;

            var newBounds = new WinDraw.Rectangle(0, 0, bounds.Width, bounds.Height);

            var newForm = new Form();

            newForm.Load += (object sender, EventArgs e) =>
            {
                ApplyHiddenForm(newForm, newBounds);
            };

            newForm.Show();

            Overlays.Add(newForm);
        }

        void CreateMainForm()
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            int height = Screen.PrimaryScreen.WorkingArea.Height;

            var newBounds = new WinDraw.Rectangle(0, 0, bounds.Width, height);

            var newForm = new Form();

            newForm.Load += (object sender, EventArgs e) =>
            {
                ApplyHiddenForm(newForm, newBounds);
            };

            newForm.Show();

            Overlays.Add(newForm);
        }

        void CreateTaskbarForm()
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            int height = bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;

            var newBounds = new WinDraw.Rectangle(0, bounds.Height - height, bounds.Width, height);

            var newForm = new Form();

            newForm.Load += (object sender, EventArgs e) =>
            {
                ApplyHiddenForm(newForm, newBounds);
            };

            newForm.Show();

            Overlays.Add(newForm);
        }

        void CreateTitleForm()
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            int height = bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;

            var newBounds = new WinDraw.Rectangle(0, bounds.Height - height, bounds.Width, height);

            var newForm = new Form();

            newForm.Load += (object sender, EventArgs e) =>
            {
                ApplyHiddenForm(newForm, newBounds);
            };

            newForm.Show();

            Overlays.Add(newForm);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        //private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        //{

        //}
        
        //private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var form in Overlays)
                form.Close();

            Close();

            Process.GetCurrentProcess().Kill();
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateOpacity();
        }
        private void UpdateOpacity()
        {
            foreach (var form in Overlays)
            {
                form.Opacity = Math.Min(1.0f - OpacitySlider.Value / 10.0f, 0.95f);
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadOverlays();
        }

        private void RootTitleBar_NotifyIconClick(object sender, RoutedEventArgs e)
        {
            Activate();
        }
    }
}
