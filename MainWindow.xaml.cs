using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AvalonDock.Layout;

namespace WpfDockProject
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ToggleDockableControl_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            string controlName = menuItem.Name.Replace("MenuItem", "").ToLower();

            // Find the corresponding LayoutAnchorable by ContentId
            var anchorable = FindLayoutAnchorable(controlName);
            
            if (anchorable != null)
            {
                if (menuItem.IsChecked)
                {
                    // Show the anchorable
                    anchorable.Show();
                }
                else
                {
                    // Hide the anchorable
                    anchorable.Hide();
                }
            }
        }

        private LayoutAnchorable FindLayoutAnchorable(string contentId)
        {
            return dockingManager.Layout.Descendents().OfType<LayoutAnchorable>()
                .FirstOrDefault(a => a.ContentId == contentId);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}