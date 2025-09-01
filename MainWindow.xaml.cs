using System;
using System.Linq;
using System.Windows;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace WpfDockProject
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowToolbox_Click(object sender, RoutedEventArgs e)
        {
            ShowPane("toolbox");
        }

        private void ShowProperties_Click(object sender, RoutedEventArgs e)
        {
            ShowPane("properties");
        }

        private void ShowSolutionExplorer_Click(object sender, RoutedEventArgs e)
        {
            ShowPane("solution");
        }

        private void ShowOutput_Click(object sender, RoutedEventArgs e)
        {
            ShowPane("output");
        }

        private void ResetLayout_Click(object sender, RoutedEventArgs e)
        {
            // Reset all panes to their default positions
            var toolbox = FindLayoutAnchorable("toolbox");
            var properties = FindLayoutAnchorable("properties");
            var solution = FindLayoutAnchorable("solution");
            var output = FindLayoutAnchorable("output");

            if (toolbox != null) toolbox.IsVisible = true;
            if (properties != null) properties.IsVisible = true;
            if (solution != null) solution.IsVisible = true;
            if (output != null) output.IsVisible = true;

            MessageBox.Show("레이아웃이 초기화되었습니다.\n\n참고: AvalonDock은 자동으로 레이아웃을 관리합니다.\n패널을 드래그하여 원하는 위치에 도킹하세요.", 
                          "레이아웃 초기화", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowPane(string contentId)
        {
            var anchorable = FindLayoutAnchorable(contentId);
            if (anchorable != null)
            {
                anchorable.IsVisible = true;
                anchorable.IsActive = true;
            }
        }

        private LayoutAnchorable? FindLayoutAnchorable(string contentId)
        {
            return dockingManager.Layout.Descendents()
                .OfType<LayoutAnchorable>()
                .FirstOrDefault(a => a.ContentId == contentId);
        }
    }
}