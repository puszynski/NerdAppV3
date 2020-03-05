using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using System.Windows.Documents;

namespace NerdAppV3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeDynamicMainMenuUsingButtons();

            //Attache event handler after InitializeComponent - will not fire on loading all RichTextBox`es
            this.NoteArea.TextChanged += new TextChangedEventHandler(NoteTextChangedEventHandler);
            this.NoteArea.Tag = "Foo"; //TODO - add key (first opened - first or last open in the future?)
        }



        #region TitleBar
        /// <summary>
        /// Allows to drag all window when drag and move
        /// </summary>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            // Begin dragging the window
            this.DragMove();
        }

        /// <summary>
        /// CloseButton_Clicked
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// MaximizedButton_Clicked
        /// </summary>
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            AdjustWindowSize();
        }

        /// <summary>
        /// Minimized Button_Clicked
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Adjusts the WindowSize to correct parameters when Maximize button is clicked
        /// </summary>
        private void AdjustWindowSize()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }

        }
        #endregion

        #region Dynamic menu and click event
        private void InitializeDynamicMainMenuUsingButtons()
        {
            var jsonConverter = new JsonConverter();
            var menuItems = jsonConverter.GetMenu().ToList();

            foreach (var item in menuItems)
            {
                var splitedItem = item.Split(';', System.StringSplitOptions.RemoveEmptyEntries);

                switch (splitedItem.Count())
                {
                    case 0:
                        break;

                    case 1:
                        Button button = new Button()
                        {
                            Content = splitedItem.First(),
                            Tag = item,
                        };
                        button.Click += new RoutedEventHandler(DynamiMenuAction_Click);
                        this.MainMenu.Children.Add(button);
                        break;

                    case 2:
                        Button button2 = new Button()
                        {
                            Content = $"{splitedItem.First()}-{splitedItem[1]}",
                            Tag = item,
                        };
                        button2.Click += new RoutedEventHandler(DynamiMenuAction_Click);
                        this.MainMenu.Children.Add(button2);
                        break;

                    default:
                        break;
                }
            }
        }

        private void DynamiMenuAction_Click(object sender, RoutedEventArgs e)
        {
            var jsonConverter = new JsonConverter();
            var key = (sender as Button).Tag.ToString();
            var result = jsonConverter.GetContentByKey(key);

            this.NoteArea.Document.Blocks.Clear();
            this.NoteArea.Document.Blocks.Add(new Paragraph(new Run(result)));
            this.NoteArea.Tag = key; //adding key to tag
        }
        #endregion

        #region Save RichTextBox after each typing
        private void NoteTextChangedEventHandler(object sender, TextChangedEventArgs args)
        {
            //skąd wiedzieć jaki value ma key? 
            var key = (sender as RichTextBox).Tag.ToString(); // czy zadziała? spr

            //jak niewywolywać przy odpalaniu?
            MessageBox.Show("Jup!");
        }
        #endregion
    }
}
