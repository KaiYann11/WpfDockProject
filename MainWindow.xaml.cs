using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDockProject
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, FloatingWindow> floatingWindows = new Dictionary<string, FloatingWindow>();
        private Dictionary<string, Border> originalPanels = new Dictionary<string, Border>();
        private Dictionary<string, GridLength> originalRowHeights = new Dictionary<string, GridLength>();
        private Dictionary<string, GridLength> originalColumnWidths = new Dictionary<string, GridLength>();
        private Canvas? dockIndicatorCanvas;
        private List<DockIndicator> dockIndicators = new List<DockIndicator>();

        public MainWindow()
        {
            InitializeComponent();
            InitializePanelDictionary();
            InitializeDockIndicators();
        }

        private void InitializePanelDictionary()
        {
            originalPanels["Toolbox"] = ToolboxPanel;
            originalPanels["Properties"] = PropertiesPanel;
            originalPanels["Solution"] = SolutionPanel;
            originalPanels["Output"] = OutputPanel;
            
            // Store original grid dimensions
            originalRowHeights["Solution"] = MainGrid.RowDefinitions[0].Height;
            originalRowHeights["Output"] = MainGrid.RowDefinitions[4].Height;
            
            Grid middleGrid = MainGrid.Children[2] as Grid;
            if (middleGrid != null)
            {
                originalColumnWidths["Toolbox"] = middleGrid.ColumnDefinitions[0].Width;
                originalColumnWidths["Properties"] = middleGrid.ColumnDefinitions[4].Width;
            }
        }

        private void ToggleDockableControl_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            string controlName = menuItem.Name.Replace("MenuItem", "");

            if (menuItem.IsChecked)
            {
                ShowDockableControl(controlName);
            }
            else
            {
                HideDockableControl(controlName);
            }
        }

        private void ShowDockableControl(string controlName)
        {
            if (floatingWindows.ContainsKey(controlName))
            {
                floatingWindows[controlName].Show();
            }
            else if (originalPanels.ContainsKey(controlName))
            {
                originalPanels[controlName].Visibility = Visibility.Visible;
                RestoreGridDimensions(controlName);
            }
        }

        private void HideDockableControl(string controlName)
        {
            if (floatingWindows.ContainsKey(controlName))
            {
                floatingWindows[controlName].Hide();
            }
            else if (originalPanels.ContainsKey(controlName))
            {
                originalPanels[controlName].Visibility = Visibility.Collapsed;
                CollapseGridDimensions(controlName);
            }
        }

        private void UndockControl_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string controlName = button.Tag.ToString();

            UndockControl(controlName);
        }

        private void UndockControl(string controlName)
        {
            if (!originalPanels.ContainsKey(controlName))
                return;

            Border originalPanel = originalPanels[controlName];
            
            if (floatingWindows.ContainsKey(controlName))
                return;

            Grid content = originalPanel.Child as Grid;
            originalPanel.Child = null;

            FloatingWindow floatingWindow = new FloatingWindow
            {
                Title = controlName,
                Width = controlName == "Solution" || controlName == "Output" ? 400 : originalPanel.Width,
                Height = controlName == "Solution" || controlName == "Output" ? originalPanel.Height + 50 : 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                PanelName = controlName,
                ParentMainWindow = this
            };

            floatingWindow.SetContent(content);
            floatingWindow.Closed += (s, e) => DockControl(controlName);

            floatingWindows[controlName] = floatingWindow;
            originalPanel.Visibility = Visibility.Collapsed;
            CollapseGridDimensions(controlName);

            floatingWindow.Show();

            UpdateMenuItem(controlName, true);
        }

        private void DockControl(string controlName)
        {
            if (!floatingWindows.ContainsKey(controlName))
                return;

            FloatingWindow floatingWindow = floatingWindows[controlName];
            Grid content = floatingWindow.ContentBorder.Child as Grid;
            
            floatingWindow.ContentBorder.Child = null;
            floatingWindows.Remove(controlName);

            Border originalPanel = originalPanels[controlName];
            originalPanel.Child = content;
            originalPanel.Visibility = Visibility.Visible;
            RestoreGridDimensions(controlName);

            UpdateMenuItem(controlName, true);
        }

        private void UpdateMenuItem(string controlName, bool isChecked)
        {
            MenuItem menuItem = null;
            switch (controlName)
            {
                case "Toolbox":
                    menuItem = ToolboxMenuItem;
                    break;
                case "Properties":
                    menuItem = PropertiesMenuItem;
                    break;
                case "Solution":
                    menuItem = SolutionMenuItem;
                    break;
                case "Output":
                    menuItem = OutputMenuItem;
                    break;
            }

            if (menuItem != null)
            {
                menuItem.IsChecked = isChecked;
            }
        }

        private void CollapseGridDimensions(string controlName)
        {
            Grid middleGrid = MainGrid.Children[2] as Grid;
            
            switch (controlName)
            {
                case "Solution":
                    MainGrid.RowDefinitions[0].Height = new GridLength(0);
                    MainGrid.RowDefinitions[1].Height = new GridLength(0); // Hide splitter
                    break;
                case "Output":
                    MainGrid.RowDefinitions[4].Height = new GridLength(0);
                    MainGrid.RowDefinitions[3].Height = new GridLength(0); // Hide splitter
                    break;
                case "Toolbox":
                    if (middleGrid != null)
                    {
                        middleGrid.ColumnDefinitions[0].Width = new GridLength(0);
                        middleGrid.ColumnDefinitions[1].Width = new GridLength(0); // Hide splitter
                    }
                    break;
                case "Properties":
                    if (middleGrid != null)
                    {
                        middleGrid.ColumnDefinitions[4].Width = new GridLength(0);
                        middleGrid.ColumnDefinitions[3].Width = new GridLength(0); // Hide splitter
                    }
                    break;
            }
        }
        
        private void RestoreGridDimensions(string controlName)
        {
            Grid middleGrid = MainGrid.Children[2] as Grid;
            
            switch (controlName)
            {
                case "Solution":
                    if (originalRowHeights.ContainsKey(controlName))
                    {
                        MainGrid.RowDefinitions[0].Height = originalRowHeights[controlName];
                        MainGrid.RowDefinitions[1].Height = new GridLength(5); // Restore splitter
                    }
                    break;
                case "Output":
                    if (originalRowHeights.ContainsKey(controlName))
                    {
                        MainGrid.RowDefinitions[4].Height = originalRowHeights[controlName];
                        MainGrid.RowDefinitions[3].Height = new GridLength(5); // Restore splitter
                    }
                    break;
                case "Toolbox":
                    if (middleGrid != null && originalColumnWidths.ContainsKey(controlName))
                    {
                        middleGrid.ColumnDefinitions[0].Width = originalColumnWidths[controlName];
                        middleGrid.ColumnDefinitions[1].Width = new GridLength(5); // Restore splitter
                    }
                    break;
                case "Properties":
                    if (middleGrid != null && originalColumnWidths.ContainsKey(controlName))
                    {
                        middleGrid.ColumnDefinitions[4].Width = originalColumnWidths[controlName];
                        middleGrid.ColumnDefinitions[3].Width = new GridLength(5); // Restore splitter
                    }
                    break;
            }
        }

        private void InitializeDockIndicators()
        {
            dockIndicatorCanvas = new Canvas
            {
                Background = Brushes.Transparent,
                IsHitTestVisible = false,
                Visibility = Visibility.Visible  // Start visible for testing
            };
            
            // Add dock indicators
            for (int i = 0; i < 5; i++)
            {
                DockIndicator indicator = new DockIndicator();
                dockIndicators.Add(indicator);
                dockIndicatorCanvas.Children.Add(indicator);
            }
            
            // Add canvas to main grid as overlay
            MainGrid.Children.Add(dockIndicatorCanvas);
            Grid.SetRowSpan(dockIndicatorCanvas, 5);
            Panel.SetZIndex(dockIndicatorCanvas, 1000);
            
            // Position indicators immediately for testing
            this.Loaded += (s, e) => PositionIndicatorsForTesting();
        }
        
        private void PositionIndicatorsForTesting()
        {
            if (dockIndicatorCanvas == null || dockIndicators.Count == 0) return;
            
            double centerX = this.ActualWidth / 2 - 30;
            double centerY = this.ActualHeight / 2 - 30;
            
            // Position all indicators for testing
            Canvas.SetLeft(dockIndicators[0], centerX);
            Canvas.SetTop(dockIndicators[0], centerY);
            
            Canvas.SetLeft(dockIndicators[1], 100);
            Canvas.SetTop(dockIndicators[1], centerY);
            
            Canvas.SetLeft(dockIndicators[2], this.ActualWidth - 160);
            Canvas.SetTop(dockIndicators[2], centerY);
            
            Canvas.SetLeft(dockIndicators[3], centerX);
            Canvas.SetTop(dockIndicators[3], 100);
            
            Canvas.SetLeft(dockIndicators[4], centerX);
            Canvas.SetTop(dockIndicators[4], this.ActualHeight - 160);
            
            System.Diagnostics.Debug.WriteLine("Indicators positioned for testing");
        }
        
        public void ShowDockIndicators(Point screenPosition, string panelName)
        {
            if (dockIndicatorCanvas == null) return;
            
            Point clientPos = this.PointFromScreen(screenPosition);
            Rect mainBounds = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
            
            // Expand bounds slightly to make it easier to trigger
            mainBounds.Inflate(50, 50);
            
            if (mainBounds.Contains(clientPos))
            {
                dockIndicatorCanvas.Visibility = Visibility.Visible;
                
                // Position indicators - Center, Left, Right, Top, Bottom
                double centerX = this.ActualWidth / 2 - 30;
                double centerY = this.ActualHeight / 2 - 30;
                
                // Center indicator
                Canvas.SetLeft(dockIndicators[0], centerX);
                Canvas.SetTop(dockIndicators[0], centerY);
                
                // Left indicator
                Canvas.SetLeft(dockIndicators[1], 100);
                Canvas.SetTop(dockIndicators[1], centerY);
                
                // Right indicator  
                Canvas.SetLeft(dockIndicators[2], this.ActualWidth - 160);
                Canvas.SetTop(dockIndicators[2], centerY);
                
                // Top indicator
                Canvas.SetLeft(dockIndicators[3], centerX);
                Canvas.SetTop(dockIndicators[3], 100);
                
                // Bottom indicator
                Canvas.SetLeft(dockIndicators[4], centerX);
                Canvas.SetTop(dockIndicators[4], this.ActualHeight - 160);
                
                // Debug output
                System.Diagnostics.Debug.WriteLine($"Showing indicators at screen: {screenPosition}, client: {clientPos}");
            }
            else
            {
                dockIndicatorCanvas.Visibility = Visibility.Collapsed;
                System.Diagnostics.Debug.WriteLine($"Hiding indicators - outside bounds. Screen: {screenPosition}, Client: {clientPos}");
            }
        }
        
        public void HandleDrop(Point screenPosition, string panelName)
        {
            if (dockIndicatorCanvas == null) 
            {
                return;
            }
            
            dockIndicatorCanvas.Visibility = Visibility.Collapsed;
            
            Point clientPos = this.PointFromScreen(screenPosition);
            
            // Check which dock zone the drop occurred in
            string dockPosition = GetDockPosition(clientPos);
            
            if (!string.IsNullOrEmpty(dockPosition))
            {
                DockControlAtPosition(panelName, dockPosition);
            }
        }
        
        private string GetDockPosition(Point position)
        {
            double centerX = this.ActualWidth / 2;
            double centerY = this.ActualHeight / 2;
            double threshold = 75;
            
            // Check center dock zone
            if (Math.Abs(position.X - centerX) < threshold && Math.Abs(position.Y - centerY) < threshold)
                return "Center";
                
            // Check edge dock zones
            if (position.X < 125 && Math.Abs(position.Y - centerY) < threshold)
                return "Left";
            if (position.X > this.ActualWidth - 125 && Math.Abs(position.Y - centerY) < threshold)
                return "Right";
            if (position.Y < 125 && Math.Abs(position.X - centerX) < threshold)
                return "Top";
            if (position.Y > this.ActualHeight - 125 && Math.Abs(position.X - centerX) < threshold)
                return "Bottom";
                
            return string.Empty;
        }
        
        private void DockControlAtPosition(string controlName, string position)
        {
            switch (position)
            {
                case "Left":
                    DockControlAtLeft(controlName);
                    break;
                case "Right":
                    DockControlAtRight(controlName);
                    break;
                case "Top":
                    DockControlAtTop(controlName);
                    break;
                case "Bottom":
                    DockControlAtBottom(controlName);
                    break;
                case "Center":
                    DockControl(controlName); // Original position
                    break;
            }
        }
        
        private void DockControlAtLeft(string controlName)
        {
            if (!floatingWindows.ContainsKey(controlName))
                return;
                
            FloatingWindow floatingWindow = floatingWindows[controlName];
            Grid content = floatingWindow.ContentBorder.Child as Grid;
            
            floatingWindow.ContentBorder.Child = null;
            floatingWindows.Remove(controlName);
            
            // Set content to left position (toolbox area)
            Grid middleGrid = MainGrid.Children[2] as Grid;
            if (middleGrid != null && content != null)
            {
                Border toolboxPanel = originalPanels["Toolbox"];
                toolboxPanel.Child = content;
                toolboxPanel.Visibility = Visibility.Visible;
                RestoreGridDimensions("Toolbox");
            }
            
            UpdateMenuItem(controlName, true);
        }
        
        private void DockControlAtRight(string controlName)
        {
            if (!floatingWindows.ContainsKey(controlName))
                return;
                
            FloatingWindow floatingWindow = floatingWindows[controlName];
            Grid content = floatingWindow.ContentBorder.Child as Grid;
            
            floatingWindow.ContentBorder.Child = null;
            floatingWindows.Remove(controlName);
            
            // Set content to right position (properties area)
            Grid middleGrid = MainGrid.Children[2] as Grid;
            if (middleGrid != null && content != null)
            {
                Border propertiesPanel = originalPanels["Properties"];
                propertiesPanel.Child = content;
                propertiesPanel.Visibility = Visibility.Visible;
                RestoreGridDimensions("Properties");
            }
            
            UpdateMenuItem(controlName, true);
        }
        
        private void DockControlAtTop(string controlName)
        {
            if (!floatingWindows.ContainsKey(controlName))
                return;
                
            FloatingWindow floatingWindow = floatingWindows[controlName];
            Grid content = floatingWindow.ContentBorder.Child as Grid;
            
            floatingWindow.ContentBorder.Child = null;
            floatingWindows.Remove(controlName);
            
            // Set content to top position (solution area)
            if (content != null)
            {
                Border solutionPanel = originalPanels["Solution"];
                solutionPanel.Child = content;
                solutionPanel.Visibility = Visibility.Visible;
                RestoreGridDimensions("Solution");
            }
            
            UpdateMenuItem(controlName, true);
        }
        
        private void DockControlAtBottom(string controlName)
        {
            if (!floatingWindows.ContainsKey(controlName))
                return;
                
            FloatingWindow floatingWindow = floatingWindows[controlName];
            Grid content = floatingWindow.ContentBorder.Child as Grid;
            
            floatingWindow.ContentBorder.Child = null;
            floatingWindows.Remove(controlName);
            
            // Set content to bottom position (output area)
            if (content != null)
            {
                Border outputPanel = originalPanels["Output"];
                outputPanel.Child = content;
                outputPanel.Visibility = Visibility.Visible;
                RestoreGridDimensions("Output");
            }
            
            UpdateMenuItem(controlName, true);
        }

        protected override void OnClosed(EventArgs e)
        {
            foreach (var window in floatingWindows.Values)
            {
                window.Close();
            }
            base.OnClosed(e);
        }
    }
}