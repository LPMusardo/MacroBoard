﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using System.IO;
using static MacroBoard.Utils;
using System.Text.RegularExpressions;
using MacroBoard.ScreenShot;

namespace MacroBoard.View
{
    public class Fields
    {

        private Panel Controls;
        private Window callerWindow;


        public Fields(Panel Controls, Window callerWindow)
        {
            this.Controls = Controls;
            this.callerWindow = callerWindow;
        }


        //---------------------------------------------------------------------------

        private TextBox baseTextBox()
        {
            TextBox textBox = new TextBox();
            textBox.GotFocus += (object sender, RoutedEventArgs args) => { textBox.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#AEAEAE"); };
            textBox.Height = 20;
            return textBox;
        }


        public (Label, TextBox) newTextBox(string labelTxt, string defaultText, TextCompositionEventHandler checkFormat = null)
        {
            Label label = newLabel(labelTxt); // pas de add() car fct 
            (label).Foreground = new SolidColorBrush(Color.FromRgb(127, 69, 152));
            TextBox textBox = baseTextBox();
            textBox.Width = (98d / 100d) * Controls.Width;
            textBox.HorizontalAlignment = HorizontalAlignment.Center;
            textBox.Text = defaultText;
            if (checkFormat != null)
                textBox.PreviewTextInput += checkFormat;
            Controls.Children.Add(textBox);
            return (label, textBox);
        }


        public void CheckDigits(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        public Button newButton(string content)
        {
            Button btn = new Button();
            btn.Content = content;
            btn.Height = 20;
            btn.Width = 50;
            Controls.Children.Add(btn);
            return btn;
        }


        public Label newLabel(string content)
        {
            Label label = new Label();
            (label).Foreground = new SolidColorBrush(Color.FromRgb(127, 69, 152));
            label.Margin = new Thickness(0, 10, 0, 0);
            label.Content = content;
            Controls.Children.Add(label);
            return label;
        }


        private (Label, TextBox, Button) BaseBrowse(string labelTxt, string defaultPath, double ratio = 73)
        {
            Label label = new Label();
            label.Margin = new Thickness(0, 10, 0, 0);
            label.Content = labelTxt;
            (label).Foreground = new SolidColorBrush(Color.FromRgb(127, 69, 152));
            Controls.Children.Add(label);

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            TextBox textBox = baseTextBox();
            textBox.Width = (ratio / 100d) * Controls.Width;
            textBox.Text = defaultPath;
            textBox.IsReadOnly = true;
            sp.Children.Add(textBox);

            Button btn = new Button();
            btn.Margin = new Thickness(5, 0, 0, 0);
            btn.Content = "Browse";
            btn.Height = 20;
            btn.Width = ((100d - ratio - 3) / 100d) * Controls.Width;
            sp.Children.Add(btn);

            Controls.Children.Add(sp);

            return (label, textBox, btn);
        }


        public (TextBox, Button) newFolderSelector(string labelTxt, string defaultPath)
        {
            (Label l, TextBox tb, Button btn) baseBrowse = BaseBrowse(labelTxt, defaultPath);
            baseBrowse.btn.Click += (object sender, RoutedEventArgs e) =>
            {
                using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        baseBrowse.tb.Text = fbd.SelectedPath;
                    }
                }
            };
            baseBrowse.btn.Content = "select folder";
            return (baseBrowse.tb, baseBrowse.btn);
        }


        public (TextBox, Button) newFileSelector(string labelTxt, string defaultPath)
        {
            (Label l, TextBox tb, Button btn) baseBrowse = BaseBrowse(labelTxt, defaultPath);
            baseBrowse.btn.Click += (object sender, RoutedEventArgs e) =>
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "All Files  (*)|*|" +
                             "JPEG Files (*.jpeg)|*.jpeg|" +
                             "JPG Files  (*.jpg)|*.jpg|" +
                             "PNG Files  (*.png)|*.png|" +
                             "GIF Files  (*.gif)|*.gif";
                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    baseBrowse.tb.Text = dlg.FileName;
                }
            };
            baseBrowse.btn.Content = "select file";
            return (baseBrowse.tb, baseBrowse.btn);
        }


        public (TextBox, Button) newSave(string labelTxt, string defaultPath)
        {
            (Label l, TextBox tb, Button btn) baseBrowse = BaseBrowse(labelTxt, defaultPath);
            baseBrowse.btn.Click += (object sender, RoutedEventArgs e) =>
            {
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();

                saveFileDialog.AddExtension = true;
                saveFileDialog.FileName = "filename";
                saveFileDialog.Filter = "jpg (*.jpg)|*.jpg" + "|image (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg" + "|txt files (*.txt)|*.txt" + "|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    baseBrowse.tb.Text = saveFileDialog.FileName;
                }
            };
            baseBrowse.btn.Content = "save";
            return (baseBrowse.tb, baseBrowse.btn);
        }


        public (TextBox, Button) newAutomationIdPicker(string labelTxt, string defaultPath)
        {
            (Label l, TextBox tb, Button btn) baseBrowse = BaseBrowse(labelTxt, defaultPath);
            baseBrowse.tb.IsReadOnly = false;
            baseBrowse.btn.Click += (object sender, RoutedEventArgs e) =>
            {
                string[] res = new string[1];
                Window w = new FlaUInspect.Views.MainWindow(res);
                callerWindow.Hide();
                w.ShowDialog();
                baseBrowse.tb.Text = res[0];
                callerWindow.ShowDialog();
                callerWindow.Activate();
            };
            baseBrowse.btn.Content = "pick id";
            return (baseBrowse.tb, baseBrowse.btn);
        }


        public (Label, TextBox, Button, Button) newScreenShotPicker(string labelTxt, string defaultPath)
        {
            Label label = new Label();
            label.Margin = new Thickness(0, 10, 0, 0);
            label.Content = labelTxt;
            (label).Foreground = new SolidColorBrush(Color.FromRgb(127, 69, 152));
            Controls.Children.Add(label);

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            TextBox textBox = baseTextBox();
            textBox.Width = (46d / 100d) * Controls.Width;
            textBox.Text = defaultPath;
            textBox.IsReadOnly = true;
            sp.Children.Add(textBox);

            Button btnFile = new Button();
            btnFile.Margin = new Thickness(5, 0, 0, 0);
            btnFile.Content = "Browse";
            btnFile.Height = 20;
            btnFile.Width = (24d/ 100d) * Controls.Width;
            sp.Children.Add(btnFile);
            btnFile.Click += (object sender, RoutedEventArgs e) =>
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "All Files  (*)|*|" +
                             "JPEG Files (*.jpeg)|*.jpeg|" +
                             "JPG Files  (*.jpg)|*.jpg|" +
                             "PNG Files  (*.png)|*.png";
                if (dlg.ShowDialog() == true)
                {
                    textBox.Text = dlg.FileName;
                }
            };

            Button btnScreen = new Button();
            btnScreen.Margin = new Thickness(5, 0, 0, 0);
            btnScreen.Content = "Screen";
            btnScreen.Height = 20;
            btnScreen.Width = (25d / 100d) * Controls.Width;
            sp.Children.Add(btnScreen);
            btnScreen.Click += (object sender, RoutedEventArgs e) =>
            {
                ScreenShotWindow w = new ScreenShotWindow();
                if (w.ShowDialog() == true)
                {
                    textBox.Text = w.filepath;
                }
            };

            Controls.Children.Add(sp);
            return (label, textBox, btnFile, btnScreen);
        }


        private ComboBox BaseComboBox(string labelTxt)
        {
            Label label = newLabel(labelTxt);
            (label).Foreground = new SolidColorBrush(Color.FromRgb(127, 69, 152));
            ComboBox cb = new ComboBox();
            cb.Width = (98d / 100d) * Controls.Width;
            Controls.Children.Add(cb);
            return cb;
        }


        public ComboBox newComboBoxBool(string labelTxt, bool value)
        {
            ComboBox cb = BaseComboBox(labelTxt);
            cb.ItemsSource = new bool[] { true, false };
            cb.SelectedIndex = (value) ? 0 : 1;
            return cb;
        }


        public ComboBox newComboBoxList(string labelTxt, System.Collections.IEnumerable values, object SelectedItem, DataTemplate template = null)
        {
            ComboBox cb = BaseComboBox(labelTxt);
            cb.ItemsSource = values;
            cb.SelectedItem = SelectedItem;
            if (template != null) cb.ItemTemplate = template;
            return cb;
        }


        public ComboBox newComboBoxList(string labelTxt, System.Collections.IEnumerable values, int SelectedIndex, DataTemplate template = null)
        {
            ComboBox cb = BaseComboBox(labelTxt);
            cb.ItemsSource = values;
            cb.SelectedIndex = SelectedIndex;
            if (template != null) cb.ItemTemplate = template;
            return cb;
        }


        //---------------------------------------------------------------------------

        public DataTemplate dataTemplateForScreen()
        {
            DataTemplate template = new DataTemplate();
            template.DataType = typeof(System.Windows.Forms.Screen);
            FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(StackPanel));
            spFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            FrameworkElementFactory DeviceName = new FrameworkElementFactory(typeof(TextBlock));
            DeviceName.SetBinding(TextBlock.TextProperty, new Binding("DeviceName"));
            spFactory.AppendChild(DeviceName);
            FrameworkElementFactory Bounds = new FrameworkElementFactory(typeof(TextBlock));
            Bounds.SetValue(TextBlock.MarginProperty, new Thickness(15, 0, 0, 0));
            Bounds.SetBinding(TextBlock.TextProperty, new Binding("Bounds"));
            spFactory.AppendChild(Bounds);
            template.VisualTree = spFactory;
            return template;
        }



        //---------------------------------------------------------------------------











    }
}
