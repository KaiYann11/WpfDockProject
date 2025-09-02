using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfDockProject
{
    public partial class FloatingWindow : Window
    {
        public string PanelName { get; set; } = string.Empty;
        public MainWindow? ParentMainWindow { get; set; }
        
        public FloatingWindow()
        {
            InitializeComponent();
        }
        
        public void SetContent(FrameworkElement content)
        {
            ContentBorder.Child = content;
            // Update title to show panel name
            if (!string.IsNullOrEmpty(PanelName))
            {
                this.Title = $"{PanelName} - Floating";
                TitleText.Text = PanelName;
            }
        }
        
        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the floating window, which will trigger the dock back functionality
            this.Close();
        }
        
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Enable dragging the window by clicking on the title bar
            if (e.ClickCount == 1)
            {
                this.DragMove();
            }
        }
        
        private void TitleBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Double-click to dock back
            this.Close();
        }
    }
}