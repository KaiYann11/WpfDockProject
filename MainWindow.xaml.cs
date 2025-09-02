using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockProject
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, FloatingWindow> floatingWindows = new Dictionary<string, FloatingWindow>();
        private Dictionary<string, Border> originalPanels = new Dictionary<string, Border>();

        public MainWindow()
        {
            InitializeComponent();
            InitializePanelDictionary();
        }

        private void InitializePanelDictionary()
        {
            originalPanels["Toolbox"] = ToolboxPanel;
            originalPanels["Properties"] = PropertiesPanel;
            originalPanels["Solution"] = SolutionPanel;
            originalPanels["Output"] = OutputPanel;
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
                Width = GetPanelWidth(controlName),
                Height = GetPanelHeight(controlName),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                PanelName = controlName,
                ParentMainWindow = this
            };

            floatingWindow.SetContent(content);
            floatingWindow.Closed += (s, e) => DockControl(controlName);

            floatingWindows[controlName] = floatingWindow;
            originalPanel.Visibility = Visibility.Collapsed;

            floatingWindow.Show();
            UpdateMenuItem(controlName, true);
        }

        private double GetPanelWidth(string controlName)
        {
            return controlName == "Toolbox" || controlName == "Properties" ? 250 : 400;
        }

        private double GetPanelHeight(string controlName)
        {
            return controlName == "Solution" || controlName == "Output" ? 200 : 300;
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