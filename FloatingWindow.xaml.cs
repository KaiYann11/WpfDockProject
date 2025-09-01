using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace WpfDockProject
{
    public partial class FloatingWindow : Window
    {
        public string PanelName { get; set; } = string.Empty;
        public MainWindow? ParentMainWindow { get; set; }
        private bool isDragging = false;
        
        public FloatingWindow()
        {
            InitializeComponent();
        }
        
        public void SetContent(FrameworkElement content)
        {
            ContentBorder.Child = content;
        }
        
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                isDragging = true;
                this.CaptureMouse();
                // Don't call DragMove() here - it blocks mouse events
            }
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                // Move the window manually
                Point currentScreenPos = this.PointToScreen(e.GetPosition(this));
                this.Left = currentScreenPos.X - this.Width / 2;
                this.Top = currentScreenPos.Y - 10;
                
                if (ParentMainWindow != null)
                {
                    ParentMainWindow.ShowDockIndicators(currentScreenPos, PanelName);
                }
            }
        }
        
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            
            if (isDragging)
            {
                isDragging = false;
                this.ReleaseMouseCapture();
                
                if (ParentMainWindow != null)
                {
                    Point screenPos = this.PointToScreen(e.GetPosition(this));
                    ParentMainWindow.HandleDrop(screenPos, PanelName);
                }
            }
        }
    }
}