using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace NerdAppV3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// learn: https://www.wpf-tutorial.com/
    /// </summary>
    public partial class MainWindow : Window
    {
        bool _isDynamicMenuLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeDynamicMainMenuUsingButtons();
            InitializeFirstNote();

            //Attache event handler after InitializeComponent - will not fire on loading all RichTextBox`es
            this.NoteArea.TextChanged += new TextChangedEventHandler(NoteTextChangedEventHandler);
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
            //todo clean exiting buttons (prevent duplicates after adding)

            _isDynamicMenuLoaded = false;
            var jsonConverter = new JsonConverter();
            var menuItemGroups = jsonConverter.GetMenu().ToList();



            foreach (var group in menuItemGroups)
            {
                foreach (var item in group)
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
                                Content = $"  ├─{splitedItem[1]}",
                                Tag = item,
                            };
                            button2.Click += new RoutedEventHandler(DynamiMenuAction_Click);
                            this.MainMenu.Children.Add(button2);
                            break;

                        default:
                            break;
                    }
                }

                AddCreateNewSubMenuButtonEnding(group.First().Split(';', System.StringSplitOptions.RemoveEmptyEntries).First());
            }
            AddCreateNewMainMenuButton(string.Empty);
            AddEditJsonButton();
            _isDynamicMenuLoaded = true;
        }

        private void InitializeFirstNote()
        {
            _isDynamicMenuLoaded = false;

            var key = "MyNote"; // TODO
            var jsonConverter = new JsonConverter();
            var result = jsonConverter.GetContentByKey(key);

            this.NoteArea.Document.Blocks.Clear();
            this.NoteArea.Document.Blocks.Add(new Paragraph(new Run(result)));
            this.NoteArea.Tag = key;

            _isDynamicMenuLoaded = true;
        }

        private void AddCreateNewSubMenuButton(string tagsToAppend)
        {
            Button button = new Button()
            {
                Content = "  ├─[+]",// "  └─[+]",
                Tag = tagsToAppend,
            };
            button.Click += new RoutedEventHandler(AddDynamiMenuAction_Click);
            this.MainMenu.Children.Add(button);
        }

        private void AddCreateNewSubMenuButtonEnding(string tagsToAppend)
        {
            Button button = new Button()
            {
                Content = "  └─[+]",
                Tag = tagsToAppend,
            };
            button.Click += new RoutedEventHandler(AddDynamiMenuAction_Click);
            this.MainMenu.Children.Add(button);
        }

        private void AddCreateNewMainMenuButton(string tagsToAppend)
        {
            Button button = new Button()
            {
                Content = "[+]",
                Tag = tagsToAppend,
            };
            button.Click += new RoutedEventHandler(AddDynamiMenuAction_Click);
            this.MainMenu.Children.Add(button);
        }

        private void AddEditJsonButton()
        {
            Button button = new Button()
            {
                Content = "[EditJSON]"
            };
            button.Click += new RoutedEventHandler(EditJson_Click);
            this.MainMenu.Children.Add(button);
        }

        private void EditJson_Click(object sender, RoutedEventArgs e)
        {
            var jsonConverter = new JsonConverter();
            var path = jsonConverter.GetFilePath();

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = true
            };
            p.Start();
        }

        private void AddDynamiMenuAction_Click(object sender, RoutedEventArgs e)
        {
            var jsonConverter = new JsonConverter();
            var tagsToAppend = (sender as Button).Tag.ToString();
            jsonConverter.AddNewMenu(tagsToAppend);

            InitializeDynamicMainMenuUsingButtons();
        }

        private void DynamiMenuAction_Click(object sender, RoutedEventArgs e)
        {
            _isDynamicMenuLoaded = false;

            var jsonConverter = new JsonConverter();
            var key = (sender as Button).Tag.ToString();
            var result = jsonConverter.GetContentByKey(key);

            this.NoteArea.Document.Blocks.Clear();
            this.NoteArea.Document.Blocks.Add(new Paragraph(new Run(result)));
            this.NoteArea.Tag = key;

            _isDynamicMenuLoaded = true;

        }
        #endregion

        #region Save RichTextBox after each typing
        private void NoteTextChangedEventHandler(object sender, TextChangedEventArgs args)
        {
            if (_isDynamicMenuLoaded == false)
                return;

            var key = (sender as RichTextBox).Tag.ToString();
            var value = new TextRange((sender as RichTextBox).Document.ContentStart, (sender as RichTextBox).Document.ContentEnd).Text;

            var jsonConverter = new JsonConverter();
            var menuItems = jsonConverter.UpdateContentByKey(key, value);

            //MessageBox.Show("Save action activated!");
        }
        #endregion
    }
}
