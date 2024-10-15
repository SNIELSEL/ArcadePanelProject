#region Using

using FluentFTP;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

#endregion

namespace WPFAdminControlApp
{
    public partial class MainWindow : Window
    {
        #region Variables
        //login variables
        public string panelLoginUsername = "Niels";
        public string panelLoginPassword = "Test";

        public string defaultConnection = "192.168.1.1";

        public int sqlConnectionAttempts;
        public int ftpConnectionAttempts;

        //ftp stuff default for me is 192.168.56.1
        public string ftp;
        public string user;
        public string pass;

        public List<RadioButton> radioButtonList = new List<RadioButton>(100);
        public List<string> radioButtonContentList = new List<string>(100);

        /// <summary>
        /// SQL Info
        ///Login Data///
        public string loginIP = "192.168.1.33";
        public string loginUser = "User";
        public string loginPass = "Pass";

        ////////Creation data////////
        public string SQLuser;
        public string SQLpass;
        public bool SQLHeadAdmin;

        public string connectionToDatabaseString = "Server=192.168.1.33,54469\\SQLEXPRESS;Database=master;User Id=User;Password=Pass;";
        public string connectionToTableString = "Server=192.168.1.33,54469\\SQLEXPRESS; Database=ArcadeDataBase;User Id=User;Password=Pass";


        public string connectionToSiteDatabaseString = "Server=193.203.168.80,3306;Database=u716772088_Testing;User Id=u716772088_TestUser;Password=TestPass00;";
        /// </summary>

        /// <animationSettings>
        public float colorFadeTime = 0.5f;
        //////////

        public bool ConnectedToSQL;
        public bool ConnectedToFTP;
        
        public bool wasLoading;

        #endregion
        public MainWindow()
        {
            InitializeComponent();

            OpenUserSQLButton.IsEnabled = false;
            UserSQLConnectionTestButton.IsEnabled = false;

            MoveBar.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;
            QuitBar.MouseLeftButtonDown += QuitBar_MouseLeftButtonDown;

            this.Loaded += MainWindow_Loaded;
        }

        #region UIelements 

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void QuitBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard TitleStoryboard = (Storyboard)FindResource("TitleColorAnimationStoryboard");
            Storyboard QuitStoryboard = (Storyboard)FindResource("QuitColorAnimationStoryboard");
            Storyboard Titlestoryboard = TitleStoryboard.Clone();
            Storyboard quitstoryboardClone = QuitStoryboard.Clone();
            Storyboard.SetTarget(Titlestoryboard, TitleBar);
            Storyboard.SetTarget(quitstoryboardClone, QuitBar);
            quitstoryboardClone.Begin();
            Titlestoryboard.Begin();

            await SaveRadioButtonsInWindow();
            await DisableRadioButtonsInWindow();
        }

        public async void ChangeButtonState(object sender, RoutedEventArgs e)
        {
            RadioButton clickedButton = sender as RadioButton;

            await RemoveUserTask();
            await RemoveSQLUserTask();

            if (clickedButton == LoginToPanelButton)
            {
                InterfacePanel.IsEnabled = false;

                ArcadeFTPPanel.IsEnabled = false;

                ArcadeUsersPanel.IsEnabled = false;

                ConnectPanel.IsEnabled = false;

                LoginAccountPanel.IsEnabled = false;

                LoginPanel.IsEnabled = true;

                myEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            }
            else if (clickedButton == ConnectButton)
            {

                if(wasLoading == true)
                {
                    ConnectPanelLoadingGif.IsEnabled = true;
                }

                InterfacePanel.IsEnabled = false;

                ArcadeFTPPanel.IsEnabled = false;

                ArcadeUsersPanel.IsEnabled = false;

                if(ConnectPanelLoadingGif.IsEnabled != true)
                {
                    ConnectPanel.IsEnabled = true;
                }

                LoginAccountPanel.IsEnabled = false;

                myEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            }
            else if (clickedButton == UIButton)
            {
                InterfacePanel.IsEnabled = true;

                ArcadeFTPPanel.IsEnabled = false;

                ArcadeUsersPanel.IsEnabled = false;

                ConnectPanel.IsEnabled = false;

                LoginAccountPanel.IsEnabled = false;

                myEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            }
            else if (clickedButton == FTPButton)
            {
                InterfacePanel.IsEnabled = false;

                ArcadeFTPPanel.IsEnabled = true;

                ArcadeUsersPanel.IsEnabled = false;

                ConnectPanel.IsEnabled = false;

                LoginAccountPanel.IsEnabled = false;

                myEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
            }
            else if (clickedButton == ArcadeAdminButton)
            {
                InterfacePanel.IsEnabled = false;

                ArcadeFTPPanel.IsEnabled = false;

                ArcadeUsersPanel.IsEnabled = true;

                ConnectPanel.IsEnabled = false;

                LoginAccountPanel.IsEnabled = false;

                myEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
            }
            else if (clickedButton == LoginAccountsButton)
            {
                InterfacePanel.IsEnabled = false;

                ArcadeFTPPanel.IsEnabled = false;

                ArcadeUsersPanel.IsEnabled = false;

                ConnectPanel.IsEnabled = false;

                LoginAccountPanel.IsEnabled = true;

                myEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
            }
            else if (clickedButton == InfoButton)
            {
                wasLoading = true;
                ConnectPanelLoadingGif.IsEnabled = false;

                InterfacePanel.IsEnabled = false;

                ArcadeFTPPanel.IsEnabled = false;

                ArcadeUsersPanel.IsEnabled = false;

                ConnectPanel.IsEnabled = false;

                LoginAccountPanel.IsEnabled = false;

                LoginPanel.IsEnabled = false;

                FTPEcclipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                SQLEcclipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                myEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            }

            if (ArcadeUsersPanel.IsEnabled == true)
            {
                OpenUserSQLButton.IsEnabled = true;
                UserSQLConnectionTestButton.IsEnabled = true;
                AddArcadeUserSQLUser.IsEnabled = true;
                AddUser_PasswordText.IsEnabled = true;
                AddUser_UserText.IsEnabled = true;
                IsHeadAdminCheckbox.IsEnabled = true;
                IsHeadAdminCheckbox.Opacity = 1;

                DisableRadioButtonsInWindow();
            }
            else
            {
                OpenUserSQLButton.IsEnabled = false;
                UserSQLConnectionTestButton.IsEnabled = false;
                AddArcadeUserSQLUser.IsEnabled = false;
                AddUser_PasswordText.IsEnabled = false;
                AddUser_UserText.IsEnabled = false;
                IsHeadAdminCheckbox.IsEnabled = false;
                IsHeadAdminCheckbox.Opacity = 0;

                DisableRadioButtonsInWindow();
            }

            if (ConnectButton.IsChecked == true)
            {
                if(MakeSQLConnection.IsEnabled == true)
                {
                    SQLEcclipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
                }
                else
                {
                    SQLEcclipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                }

                if(MakeFTPConnection.IsEnabled == true)
                {
                    FTPEcclipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
                }
                else
                {
                    FTPEcclipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                }
            }
            else
            {
                FTPEcclipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                SQLEcclipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            }

            DisableRadioButtonsInWindow();
        }
        #endregion

        #region SetDataFromText

        public void SetCurrentUsername(object sender, TextChangedEventArgs args)
        {
            panelLoginUsername = LoginUsernameInfo.Text;
        }

        public void SetCurrentPassword(object sender, TextChangedEventArgs args)
        {
            panelLoginPassword = LoginPasswordInfo.Text;
        }

        public void SetIP(object sender, TextChangedEventArgs args)
        {
            ftp = ("ftp://" + FTPIPtext.Text);
            loginIP = (SQLIPtext.Text);

            connectionToDatabaseString = $"Server={loginIP},54469\\SQLEXPRESS;Database=master;User Id={loginUser};Password={loginPass};";
            connectionToTableString = $"Server={loginIP},54469\\SQLEXPRESS; Database=ArcadeDataBase;User Id={loginUser};Password={loginPass}";

            //MessageBox.Show(ftp);
        }

        public void SetUser(object sender, TextChangedEventArgs args)
        {
            user = FTPUserText.Text;
            loginUser = (SQLUserText.Text);

            connectionToDatabaseString = $"Server={loginIP},54469\\SQLEXPRESS;Database=master;User Id={loginUser};Password={loginPass};";
            connectionToTableString = $"Server={loginIP},54469\\SQLEXPRESS; Database=ArcadeDataBase;User Id={loginUser};Password={loginPass}";

            //MessageBox.Show(ftp);
        }

        public void SetPassword(object sender, TextChangedEventArgs args)
        {
            pass = FTPPasswordText.Text;
            loginPass = (SQLPasswordText.Text);

            connectionToDatabaseString = $"Server={loginIP},54469\\SQLEXPRESS;Database=master;User Id={loginUser};Password={loginPass};";
            connectionToTableString = $"Server={loginIP},54469\\SQLEXPRESS; Database=ArcadeDataBase;User Id={loginUser};Password={loginPass}";

            //MessageBox.Show(ftp);
        }

        public void SetSQLUser(object sender, TextChangedEventArgs args)
        {
            SQLuser = AddUser_UserText.Text;
        }

        public void SetSQLPassword(object sender, TextChangedEventArgs args)
        {
            SQLpass = AddUser_PasswordText.Text;
        }

        public void HandleCheck(object sender, RoutedEventArgs e)
        {
            SQLHeadAdmin = true;
        }

        public void HandleUnchecked(object sender, RoutedEventArgs e)
        {
            SQLHeadAdmin = false;
        }
        public void HandleThirdState(object sender, RoutedEventArgs e)
        {
            SQLHeadAdmin = false;
        }

        #endregion

        #region UI

        public void ChangeNewsImage(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region SQL

        public async void SQLMakeConnection(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(connectionToDatabaseString);

            using (SqlConnection myConn = new SqlConnection(connectionToDatabaseString))
            {
                try
                {
                    myConn.Open();

                    //checkt als database bestaat
                    string databaseName = "ArcadeDatabase";
                    if (!DatabaseExists(myConn, databaseName))
                    {
                        FirstTimeSQLSetup(myConn);
                    }
                    else
                    {
                        //MessageBox.Show("Database already exists", "MyProgram", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    if (myConn.State == ConnectionState.Open)
                    {
                        ColorAnimation colorAnimation = new ColorAnimation
                        {
                            From = ((SolidColorBrush)SQLEcclipse.Fill).Color,
                            To = (Color)ColorConverter.ConvertFromString("#006400"),
                            Duration = new Duration(TimeSpan.FromSeconds(colorFadeTime)),
                            AutoReverse = false,
                        };

                        SolidColorBrush brush = (SolidColorBrush)SQLEcclipse.Fill;
                        brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                        await Task.Delay(1500);

                        MakeSQLConnection.IsEnabled = false;
                        SQLEnter.IsEnabled = false;
                        SQLIPtext.IsEnabled = false;
                        SQLUserText.IsEnabled = false;
                        SQLPasswordText.IsEnabled = false;
                        SQLIPtext.Visibility = Visibility.Collapsed;
                        SQLUserText.Visibility = Visibility.Collapsed;
                        SQLPasswordText.Visibility = Visibility.Collapsed;
                        SQLEcclipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                        LoginAccountsButton.IsEnabled = true;
                        UIButton.IsEnabled = true;
                        ArcadeAdminButton.IsEnabled = true;
                        if (MakeFTPConnection.IsEnabled == false && MakeSQLConnection.IsEnabled == false)
                        {
                            ConnectButton.IsEnabled = false;
                            ConnectPanel.IsEnabled = false;
                            InterfacePanel.IsEnabled = true;
                            UIButton.IsChecked = true;
                        }

                        DisableRadioButtonsInWindow();

                        MessageBoxResult Result = MessageBox.Show("Would you like to set this as your default connection?", "Set as default connection?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        string filepath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\ArcadeInfo\DefaultConnection.txt";

                        if (Result == MessageBoxResult.Yes)
                        {
                            TextReader tr = new StreamReader(filepath);
                            if (SQLIPtext.Text != tr.ReadLine())
                            {
                                tr.Close();
                                TextWriter tw = new StreamWriter(filepath);
                                tw.WriteLine(SQLIPtext.Text);
                                tw.Close();
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    //MessageBox.Show(connectionToDatabaseString);

                    MessageBox.Show($"SQL Error: {ex.Message}\nError Number: {ex.Number}\nState: {ex.State}\nClass: {ex.Class}",
                                    "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    ColorAnimation colorAnimation = new ColorAnimation
                    {
                        From = ((SolidColorBrush)SQLEcclipse.Fill).Color,
                        To = (Color)ColorConverter.ConvertFromString("#8B0000"),
                        Duration = new Duration(TimeSpan.FromSeconds(colorFadeTime)),
                        AutoReverse = false,
                    };

                    SolidColorBrush brush = (SolidColorBrush)SQLEcclipse.Fill;
                    brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                    MakeSQLConnection.IsEnabled = true;

                    DisableRadioButtonsInWindow();
                }
                finally
                {
                    if (myConn.State == ConnectionState.Open)
                    {
                        myConn.Close();
                    }
                }
            }
        }

        public async Task SQLMakeConnection()
        {
            using (SqlConnection myConn = new SqlConnection(connectionToDatabaseString))
            {
                try
                {
                    await myConn.OpenAsync();

                    if (myConn.State == ConnectionState.Open)
                    {
                        await this.Dispatcher.InvokeAsync(async() =>
                        {
                            ColorAnimation colorAnimation = new ColorAnimation
                            {
                                From = ((SolidColorBrush)SQLEcclipse.Fill).Color,
                                To = (Color)ColorConverter.ConvertFromString("#006400"),
                                Duration = new Duration(TimeSpan.FromSeconds(colorFadeTime)),
                                AutoReverse = false,
                            };

                            SolidColorBrush brush = (SolidColorBrush)SQLEcclipse.Fill;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                            await Task.Delay(1500);

                            ConnectedToSQL = true;
                            MakeSQLConnection.IsEnabled = false;
                            SQLEnter.IsEnabled = false;
                            SQLIPtext.IsEnabled = false;
                            SQLUserText.IsEnabled = false;
                            SQLPasswordText.IsEnabled = false;
                            SQLIPtext.Visibility = Visibility.Collapsed;
                            SQLUserText.Visibility = Visibility.Collapsed;
                            SQLPasswordText.Visibility = Visibility.Collapsed;
                            SQLEcclipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                            LoginAccountsButton.IsEnabled = true;
                            UIButton.IsEnabled = true;
                            ArcadeAdminButton.IsEnabled = true;
                            if (ConnectedToFTP == true && ConnectedToSQL == true)
                            {
                                ConnectButton.IsEnabled = false;
                                ConnectPanelLoadingGif.IsEnabled = false;
                                InterfacePanel.IsEnabled = true;
                                UIButton.IsChecked = true;
                            }
                            await this.Dispatcher.InvokeAsync(() => DisableRadioButtonsInWindow());
                        });
                    }
                }
                catch (SqlException ex)
                {
                    await this.Dispatcher.InvokeAsync(() =>
                    {
                        MakeSQLConnection.IsEnabled = true;

                        sqlConnectionAttempts++;
                        CheckStandardPorts();
                    });

                    await this.Dispatcher.InvokeAsync(() => DisableRadioButtonsInWindow());
                }
                finally
                {
                    if (myConn.State == ConnectionState.Open)
                    {
                        await myConn.CloseAsync();
                    }
                }
            }
        }



        public void SQLConnectionCheck(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(connectionToDatabaseString);

            using (SqlConnection myConn = new SqlConnection(connectionToDatabaseString))
            {
                try
                {
                    myConn.Open();

                    //checkt als database bestaat
                    string databaseName = "ArcadeDatabase";
                    if (!DatabaseExists(myConn, databaseName))
                    {
                        FirstTimeSQLSetup(myConn);
                    }
                    else
                    {
                        //MessageBox.Show("Database already exists", "MyProgram", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    if (myConn.State == ConnectionState.Open)
                    {
                        ColorAnimation colorAnimation = new ColorAnimation
                        {
                            From = ((SolidColorBrush)myEllipse.Fill).Color,
                            To = (Color)ColorConverter.ConvertFromString("#006400"),
                            Duration = new Duration(TimeSpan.FromSeconds(colorFadeTime)),
                            AutoReverse = false,
                        };

                        SolidColorBrush brush = (SolidColorBrush)myEllipse.Fill;
                        brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                        //AddUserToDatabase("Niels", "123456", true);
                    }
                }
                catch (SqlException ex)
                {
                    //MessageBox.Show(connectionToDatabaseString);

                    MessageBox.Show($"SQL Error: {ex.Message}\nError Number: {ex.Number}\nState: {ex.State}\nClass: {ex.Class}",
                                    "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    ColorAnimation colorAnimation = new ColorAnimation
                    {
                        From = ((SolidColorBrush)myEllipse.Fill).Color,
                        To = (Color)ColorConverter.ConvertFromString("#8B0000"),
                        Duration = new Duration(TimeSpan.FromSeconds(colorFadeTime)),
                        AutoReverse = false,
                    };

                    SolidColorBrush brush = (SolidColorBrush)myEllipse.Fill;
                    brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
                }
                finally
                {
                    if (myConn.State == ConnectionState.Open)
                    {
                        myConn.Close();
                    }
                }
            }
        }

        public void MySQLConnectionCheck(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(connectionToDatabaseString);

            using (MySqlConnection myConn = new MySqlConnection(connectionToSiteDatabaseString))
            {
                try
                {
                    myConn.Open();

                    if (myConn.State == ConnectionState.Open)
                    {
                        ColorAnimation colorAnimation = new ColorAnimation
                        {
                            From = ((SolidColorBrush)myEllipse.Fill).Color,
                            To = (Color)ColorConverter.ConvertFromString("#006400"),
                            Duration = new Duration(TimeSpan.FromSeconds(colorFadeTime)),
                            AutoReverse = false,
                        };

                        SolidColorBrush brush = (SolidColorBrush)myEllipse.Fill;
                        brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                        //AddUserToDatabase("Niels", "123456", true);
                    }
                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show(connectionToDatabaseString);

                    MessageBox.Show($"SQL Error: {ex.Message}\nError Number: {ex.Number}\nState: {ex.SqlState}\nClass: {ex.ErrorCode}",
                                    "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    ColorAnimation colorAnimation = new ColorAnimation
                    {
                        From = ((SolidColorBrush)myEllipse.Fill).Color,
                        To = (Color)ColorConverter.ConvertFromString("#8B0000"),
                        Duration = new Duration(TimeSpan.FromSeconds(colorFadeTime)),
                        AutoReverse = false,
                    };

                    SolidColorBrush brush = (SolidColorBrush)myEllipse.Fill;
                    brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
                }
                finally
                {
                    if (myConn.State == ConnectionState.Open)
                    {
                        myConn.Close();
                    }
                }
            }
        }

        #region SQL_Querry

        public async void LoginToAdminPanel(object sender, RoutedEventArgs e)
        {
            string sql = $"SELECT Name, Password FROM UserAccounts";
            List<MySQLUserAccount> userAccountsList = new List<MySQLUserAccount>();

            using (MySqlConnection connection = new MySqlConnection(connectionToSiteDatabaseString))
            {
                MySqlCommand command = new MySqlCommand(sql, connection);
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create a new UserAccount object
                        MySQLUserAccount account = new MySQLUserAccount
                        {
                            Name = reader["Name"].ToString(),
                            Password = reader["Password"].ToString(),
                        };

                        // Add the account to the list
                        userAccountsList.Add(account);

                        List<string> addedUsernames = new List<string>(); // List to track usernames already added

                        foreach (var account1 in userAccountsList)
                        {
                            if(account1.Name == LoginUsernameInfo.Text && account1.Password == LoginPasswordInfo.Text)
                            {
                                LoginPanel.IsEnabled = false;
                                LoginToPanelButton.IsEnabled = false;

                                ConnectButton.IsEnabled = true;
                                ConnectButton.IsChecked = true;
                                ConnectPanelLoadingGif.IsEnabled = true;

                                await DisableRadioButtonsInWindow();

                                await Task.Delay(200);

                                CheckStandardPorts();
                            }
                            else
                            {
                                MessageBox.Show("Couldnt find account with that username or password");
                            }
                        }
                    }
                }
            }

            DisableRadioButtonsInWindow();
        }

        private bool DatabaseExists(SqlConnection connection, string databaseName)
        {
            string checkDbQuery = $"SELECT database_id FROM sys.databases WHERE Name = '{databaseName}'";
            using (SqlCommand command = new SqlCommand(checkDbQuery, connection))
            {
                object result = command.ExecuteScalar();
                return result != null;
            }
        }


        private void FirstTimeSQLSetup(SqlConnection connection)
        {
            string createDatabaseQuery = "CREATE DATABASE ArcadeDatabase";

            using (SqlCommand command = new SqlCommand(createDatabaseQuery, connection))
            {
                command.ExecuteNonQuery();
                //MessageBox.Show("Database created successfully", "MyProgram", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public async void GetUsers(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            radioButton.IsHitTestVisible = false;
            await RemoveUserTask();
            await Task.Delay(500);

            List<UserAccount> userAccountsList = new List<UserAccount>();

            string selectQuery = "SELECT Name, Password, isHeadAdmin FROM useraccounts";

            //button creation Info
            int fontSize = 14;
            int textHeight = 50;
            Color textColor = Colors.White;

            using (SqlConnection connection = new SqlConnection(connectionToTableString))
            {
                SqlCommand command = new SqlCommand(selectQuery, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create a new UserAccount object
                        UserAccount account = new UserAccount
                        {
                            Name = reader["Name"].ToString(),
                            Password = reader["Password"].ToString(),
                            IsHeadAdmin = (bool)reader["isHeadAdmin"]
                        };

                        // Add the account to the list
                        userAccountsList.Add(account);

                        List<string> addedUsernames = new List<string>(); // List to track usernames already added

                        foreach (var account1 in userAccountsList)
                        {
                            // Check for duplicate usernames
                            if (!addedUsernames.Contains(account.Name))
                            {
                                Grid accountInfoGrid = new Grid();

                                ColumnDefinition c1 = new ColumnDefinition();
                                c1.Width = new GridLength(200, GridUnitType.Pixel);

                                ColumnDefinition c2 = new ColumnDefinition();
                                c2.Width = new GridLength(220, GridUnitType.Pixel);

                                ColumnDefinition c3 = new ColumnDefinition();
                                c3.Width = new GridLength(180, GridUnitType.Pixel);

                                ColumnDefinition c4 = new ColumnDefinition();
                                c4.Width = new GridLength(150, GridUnitType.Pixel);

                                accountInfoGrid.ColumnDefinitions.Add(c1);
                                accountInfoGrid.ColumnDefinitions.Add(c2);
                                accountInfoGrid.ColumnDefinitions.Add(c3);
                                accountInfoGrid.ColumnDefinitions.Add(c4);
                                accountInfoGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                                UserStackPanel.Children.Add(accountInfoGrid);

                                RadioButton NameRadioButton = new RadioButton();

                                NameRadioButton.Content = $"Name: {account.Name}";
                                NameRadioButton.Height = textHeight;
                                NameRadioButton.Foreground = new SolidColorBrush(textColor);
                                NameRadioButton.FontSize = fontSize;
                                NameRadioButton.IsEnabled = false;
                                NameRadioButton.Visibility = Visibility.Visible;
                                NameRadioButton.SetValue(Grid.ColumnProperty, 0);

                                NameRadioButton.Style = (Style)FindResource("MenuButtonTheme");

                                accountInfoGrid.Children.Add(NameRadioButton);

                                ////////////////////////////////////////////////////////////////////////

                                RadioButton PasswordRadioButton = new RadioButton();

                                PasswordRadioButton.Content = $"Password: {account.Password}";
                                PasswordRadioButton.Height = textHeight;
                                PasswordRadioButton.Foreground = new SolidColorBrush(textColor);
                                PasswordRadioButton.FontSize = fontSize;
                                PasswordRadioButton.IsEnabled = false;
                                PasswordRadioButton.Visibility = Visibility.Visible;
                                PasswordRadioButton.SetValue(Grid.ColumnProperty, 1);

                                PasswordRadioButton.Style = (Style)FindResource("MenuButtonTheme");

                                accountInfoGrid.Children.Add(PasswordRadioButton);

                                //////////////////////////////////////////////////////////////////////////

                                RadioButton HeadAdminButton = new RadioButton();

                                HeadAdminButton.Content = $"HeadAdmin: {account.IsHeadAdmin}";
                                HeadAdminButton.Height = textHeight;
                                HeadAdminButton.Foreground = new SolidColorBrush(textColor);
                                HeadAdminButton.FontSize = fontSize;
                                HeadAdminButton.IsEnabled = false;
                                HeadAdminButton.Visibility = Visibility.Visible;
                                HeadAdminButton.SetValue(Grid.ColumnProperty, 2);

                                HeadAdminButton.Style = (Style)FindResource("MenuButtonTheme");

                                accountInfoGrid.Children.Add(HeadAdminButton);

                                addedUsernames.Add(account.Name);

                                //////////////////////////////////////////////////////////////////////////
                                RadioButton Deletebutton = new RadioButton();
                                RadioButton DeleteInfo = new RadioButton();

                                DeleteInfo.Foreground = new SolidColorBrush(Colors.Transparent);
                                DeleteInfo.Background = new SolidColorBrush(Colors.Transparent);
                                DeleteInfo.BorderBrush = new SolidColorBrush(Colors.Transparent);
                                DeleteInfo.BorderThickness = new Thickness(0);
                                DeleteInfo.Style = (Style)FindResource("NoHover");
                                DeleteInfo.Content = account.Name.ToString();

                                Deletebutton.Margin = new Thickness(-100, 0, 0, 0);
                                Deletebutton.Style = (Style)FindResource("DarkenOnHoverRadioButtonStyle");
                                Deletebutton.Content = new Image
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/Images/Delete.png")),
                                };


                                Deletebutton.IsEnabled = false;
                                Deletebutton.Visibility = Visibility.Visible;
                                Deletebutton.SetValue(Grid.ColumnProperty, 3);
                                DeleteInfo.SetValue(Grid.ColumnProperty, 3);
                                DeleteInfo.Click += new RoutedEventHandler(RemoveUsers);

                                accountInfoGrid.Children.Add(Deletebutton);
                                accountInfoGrid.Children.Add(DeleteInfo);

                                addedUsernames.Add(account.Name);

                                await Task.Delay(100);
                            }
                            else
                            {
                                //MessageBox.Show($"Duplicate account found for: {account.Name}. Skipping...");
                            }
                        }
                    }
                }
                radioButton.IsHitTestVisible = true;
            }
        }

        public async Task GetUsers()
        {
            await RemoveUserTask();
            await Task.Delay(500);

            List<UserAccount> userAccountsList = new List<UserAccount>();

            string selectQuery = "SELECT Name, Password, isHeadAdmin FROM useraccounts";

            //button creation Info
            int fontSize = 14;
            int textHeight = 50;
            Color textColor = Colors.White;

            using (SqlConnection connection = new SqlConnection(connectionToTableString))
            {
                SqlCommand command = new SqlCommand(selectQuery, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create a new UserAccount object
                        UserAccount account = new UserAccount
                        {
                            Name = reader["Name"].ToString(),
                            Password = reader["Password"].ToString(),
                            IsHeadAdmin = (bool)reader["isHeadAdmin"]
                        };

                        // Add the account to the list
                        userAccountsList.Add(account);

                        List<string> addedUsernames = new List<string>(); // List to track usernames already added

                        foreach (var account1 in userAccountsList)
                        {
                            // Check for duplicate usernames
                            if (!addedUsernames.Contains(account.Name))
                            {
                                Grid accountInfoGrid = new Grid();

                                ColumnDefinition c1 = new ColumnDefinition();
                                c1.Width = new GridLength(200, GridUnitType.Pixel);

                                ColumnDefinition c2 = new ColumnDefinition();
                                c2.Width = new GridLength(220, GridUnitType.Pixel);

                                ColumnDefinition c3 = new ColumnDefinition();
                                c3.Width = new GridLength(180, GridUnitType.Pixel);

                                ColumnDefinition c4 = new ColumnDefinition();
                                c4.Width = new GridLength(150, GridUnitType.Pixel);

                                accountInfoGrid.ColumnDefinitions.Add(c1);
                                accountInfoGrid.ColumnDefinitions.Add(c2);
                                accountInfoGrid.ColumnDefinitions.Add(c3);
                                accountInfoGrid.ColumnDefinitions.Add(c4);
                                accountInfoGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                                UserStackPanel.Children.Add(accountInfoGrid);

                                RadioButton NameRadioButton = new RadioButton();

                                NameRadioButton.Content = $"Name: {account.Name}";
                                NameRadioButton.Height = textHeight;
                                NameRadioButton.Foreground = new SolidColorBrush(textColor);
                                NameRadioButton.FontSize = fontSize;
                                NameRadioButton.IsEnabled = false;
                                NameRadioButton.Visibility = Visibility.Visible;
                                NameRadioButton.SetValue(Grid.ColumnProperty, 0);

                                NameRadioButton.Style = (Style)FindResource("MenuButtonTheme");

                                accountInfoGrid.Children.Add(NameRadioButton);

                                ////////////////////////////////////////////////////////////////////////

                                RadioButton PasswordRadioButton = new RadioButton();

                                PasswordRadioButton.Content = $"Password: {account.Password}";
                                PasswordRadioButton.Height = textHeight;
                                PasswordRadioButton.Foreground = new SolidColorBrush(textColor);
                                PasswordRadioButton.FontSize = fontSize;
                                PasswordRadioButton.IsEnabled = false;
                                PasswordRadioButton.Visibility = Visibility.Visible;
                                PasswordRadioButton.SetValue(Grid.ColumnProperty, 1);

                                PasswordRadioButton.Style = (Style)FindResource("MenuButtonTheme");

                                accountInfoGrid.Children.Add(PasswordRadioButton);

                                //////////////////////////////////////////////////////////////////////////

                                RadioButton HeadAdminButton = new RadioButton();

                                HeadAdminButton.Content = $"HeadAdmin: {account.IsHeadAdmin}";
                                HeadAdminButton.Height = textHeight;
                                HeadAdminButton.Foreground = new SolidColorBrush(textColor);
                                HeadAdminButton.FontSize = fontSize;
                                HeadAdminButton.IsEnabled = false;
                                HeadAdminButton.Visibility = Visibility.Visible;
                                HeadAdminButton.SetValue(Grid.ColumnProperty, 2);

                                HeadAdminButton.Style = (Style)FindResource("MenuButtonTheme");

                                accountInfoGrid.Children.Add(HeadAdminButton);

                                addedUsernames.Add(account.Name);

                                //////////////////////////////////////////////////////////////////////////

                                RadioButton Deletebutton = new RadioButton();
                                RadioButton DeleteInfo = new RadioButton();

                                DeleteInfo.Foreground = new SolidColorBrush(Colors.Transparent);
                                DeleteInfo.Background = new SolidColorBrush(Colors.Transparent);
                                DeleteInfo.BorderBrush = new SolidColorBrush(Colors.Transparent);
                                DeleteInfo.BorderThickness = new Thickness(0);
                                DeleteInfo.Style = (Style)FindResource("NoHover");
                                DeleteInfo.Content = account.Name.ToString();

                                Deletebutton.Margin = new Thickness(-100, 0, 0, 0);
                                Deletebutton.Style = (Style)FindResource("DarkenOnHoverRadioButtonStyle");
                                Deletebutton.Content = new Image
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/Images/Delete.png")),
                                };


                                Deletebutton.IsEnabled = false;
                                Deletebutton.Visibility = Visibility.Visible;
                                Deletebutton.SetValue(Grid.ColumnProperty, 3);
                                DeleteInfo.SetValue(Grid.ColumnProperty, 3);
                                DeleteInfo.Click += new RoutedEventHandler(RemoveUsers);

                                accountInfoGrid.Children.Add(Deletebutton);
                                accountInfoGrid.Children.Add(DeleteInfo);

                                addedUsernames.Add(account.Name);

                                await Task.Delay(100);
                            }
                            else
                            {
                                //MessageBox.Show($"Duplicate account found for: {account.Name}. Skipping...");
                            }
                        }
                    }
                }
            }
        }

        public async void GetMySQLUsers(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            radioButton.IsHitTestVisible = false;
            await RemoveSQLUserTask();
            await Task.Delay(500);

            List<MySQLUserAccount> userAccountsList = new List<MySQLUserAccount>();

            string selectQuery = "SELECT Name, Password FROM UserAccounts";

            //button creation Info
            int fontSize = 14;
            int textHeight = 50;
            Color textColor = Colors.White;

            using (MySqlConnection connection = new MySqlConnection(connectionToSiteDatabaseString))
            {
                MySqlCommand command = new MySqlCommand(selectQuery, connection);
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create a new UserAccount object
                        MySQLUserAccount account = new MySQLUserAccount
                        {
                            Name = reader["Name"].ToString(),
                            Password = reader["Password"].ToString(),
                        };

                        // Add the account to the list
                        userAccountsList.Add(account);

                        List<string> addedUsernames = new List<string>(); // List to track usernames already added

                        foreach (var account1 in userAccountsList)
                        {
                            // Check for duplicate usernames
                            if (!addedUsernames.Contains(account.Name))
                            {
                                Grid accountInfoGrid = new Grid();

                                ColumnDefinition c1 = new ColumnDefinition();
                                c1.Width = new GridLength(200, GridUnitType.Pixel);

                                ColumnDefinition c2 = new ColumnDefinition();
                                c2.Width = new GridLength(220, GridUnitType.Pixel);

                                ColumnDefinition c3 = new ColumnDefinition();
                                c3.Width = new GridLength(180, GridUnitType.Pixel);

                                ColumnDefinition c4 = new ColumnDefinition();
                                c4.Width = new GridLength(150, GridUnitType.Pixel);

                                accountInfoGrid.ColumnDefinitions.Add(c1);
                                accountInfoGrid.ColumnDefinitions.Add(c2);
                                accountInfoGrid.ColumnDefinitions.Add(c3);
                                accountInfoGrid.ColumnDefinitions.Add(c4);
                                accountInfoGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                                SQLUserStackPanel.Children.Add(accountInfoGrid);

                                RadioButton NameRadioButton = new RadioButton();

                                NameRadioButton.Content = $"Name: {account.Name}";
                                NameRadioButton.Height = textHeight;
                                NameRadioButton.Foreground = new SolidColorBrush(textColor);
                                NameRadioButton.FontSize = fontSize;
                                NameRadioButton.IsEnabled = false;
                                NameRadioButton.Visibility = Visibility.Visible;
                                NameRadioButton.SetValue(Grid.ColumnProperty, 0);

                                NameRadioButton.Style = (Style)FindResource("MenuButtonTheme");

                                accountInfoGrid.Children.Add(NameRadioButton);

                                ////////////////////////////////////////////////////////////////////////

                                RadioButton PasswordRadioButton = new RadioButton();

                                PasswordRadioButton.Content = $"Password: {account.Password}";
                                PasswordRadioButton.Height = textHeight;
                                PasswordRadioButton.Foreground = new SolidColorBrush(textColor);
                                PasswordRadioButton.FontSize = fontSize;
                                PasswordRadioButton.IsEnabled = false;
                                PasswordRadioButton.Visibility = Visibility.Visible;
                                PasswordRadioButton.SetValue(Grid.ColumnProperty, 1);

                                PasswordRadioButton.Style = (Style)FindResource("MenuButtonTheme");

                                accountInfoGrid.Children.Add(PasswordRadioButton);

                                //////////////////////////////////////////////////////////////////////////

                                addedUsernames.Add(account.Name);

                                //////////////////////////////////////////////////////////////////////////

                                RadioButton Deletebutton = new RadioButton();
                                RadioButton DeleteInfo = new RadioButton();

                                DeleteInfo.Foreground = new SolidColorBrush(Colors.Transparent);
                                DeleteInfo.Background = new SolidColorBrush(Colors.Transparent);
                                DeleteInfo.BorderBrush = new SolidColorBrush(Colors.Transparent);
                                DeleteInfo.BorderThickness = new Thickness(0);
                                DeleteInfo.Style = (Style)FindResource("NoHover");
                                DeleteInfo.Content = account.Name.ToString();

                                Deletebutton.Margin = new Thickness(-100, 0, 0, 0);
                                Deletebutton.Style = (Style)FindResource("DarkenOnHoverRadioButtonStyle");
                                Deletebutton.Content = new Image
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/Images/Delete.png")),
                                };


                                Deletebutton.IsEnabled = false;
                                Deletebutton.Visibility = Visibility.Visible;
                                Deletebutton.SetValue(Grid.ColumnProperty, 3);
                                DeleteInfo.SetValue(Grid.ColumnProperty, 3);
                                DeleteInfo.Click += new RoutedEventHandler(RemoveMySQLUsers);

                                accountInfoGrid.Children.Add(Deletebutton);
                                accountInfoGrid.Children.Add(DeleteInfo);

                                addedUsernames.Add(account.Name);

                                await Task.Delay(100);
                            }
                            else
                            {
                                //MessageBox.Show($"Duplicate account found for: {account.Name}. Skipping...");
                            }
                        }
                    }
                }
            }
            radioButton.IsHitTestVisible = true;
        }

        public async Task GetMySQLUsers()
        {
            await RemoveSQLUserTask();
            await Task.Delay(500);

            List<MySQLUserAccount> userAccountsList = new List<MySQLUserAccount>();

            string selectQuery = "SELECT Name, Password FROM UserAccounts";

            //button creation Info
            int fontSize = 14;
            int textHeight = 50;
            Color textColor = Colors.White;

            using (MySqlConnection connection = new MySqlConnection(connectionToSiteDatabaseString))
            {
                MySqlCommand command = new MySqlCommand(selectQuery, connection);
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create a new UserAccount object
                        MySQLUserAccount account = new MySQLUserAccount
                        {
                            Name = reader["Name"].ToString(),
                            Password = reader["Password"].ToString(),
                        };

                        // Add the account to the list
                        userAccountsList.Add(account);

                        List<string> addedUsernames = new List<string>(); // List to track usernames already added

                        foreach (var account1 in userAccountsList)
                        {
                            // Check for duplicate usernames
                            if (!addedUsernames.Contains(account.Name))
                            {
                                Grid accountInfoGrid = new Grid();

                                ColumnDefinition c1 = new ColumnDefinition();
                                c1.Width = new GridLength(200, GridUnitType.Pixel);

                                ColumnDefinition c2 = new ColumnDefinition();
                                c2.Width = new GridLength(220, GridUnitType.Pixel);

                                ColumnDefinition c3 = new ColumnDefinition();
                                c3.Width = new GridLength(180, GridUnitType.Pixel);

                                ColumnDefinition c4 = new ColumnDefinition();
                                c4.Width = new GridLength(150, GridUnitType.Pixel);

                                accountInfoGrid.ColumnDefinitions.Add(c1);
                                accountInfoGrid.ColumnDefinitions.Add(c2);
                                accountInfoGrid.ColumnDefinitions.Add(c3);
                                accountInfoGrid.ColumnDefinitions.Add(c4);
                                accountInfoGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                                SQLUserStackPanel.Children.Add(accountInfoGrid);

                                RadioButton NameRadioButton = new RadioButton();

                                NameRadioButton.Content = $"Name: {account.Name}";
                                NameRadioButton.Height = textHeight;
                                NameRadioButton.Foreground = new SolidColorBrush(textColor);
                                NameRadioButton.FontSize = fontSize;
                                NameRadioButton.IsEnabled = false;
                                NameRadioButton.Visibility = Visibility.Visible;
                                NameRadioButton.SetValue(Grid.ColumnProperty, 0);

                                NameRadioButton.Style = (Style)FindResource("MenuButtonTheme");

                                accountInfoGrid.Children.Add(NameRadioButton);

                                ////////////////////////////////////////////////////////////////////////

                                RadioButton PasswordRadioButton = new RadioButton();

                                PasswordRadioButton.Content = $"Password: {account.Password}";
                                PasswordRadioButton.Height = textHeight;
                                PasswordRadioButton.Foreground = new SolidColorBrush(textColor);
                                PasswordRadioButton.FontSize = fontSize;
                                PasswordRadioButton.IsEnabled = false;
                                PasswordRadioButton.Visibility = Visibility.Visible;
                                PasswordRadioButton.SetValue(Grid.ColumnProperty, 1);

                                PasswordRadioButton.Style = (Style)FindResource("MenuButtonTheme");

                                accountInfoGrid.Children.Add(PasswordRadioButton);

                                //////////////////////////////////////////////////////////////////////////

                                addedUsernames.Add(account.Name);

                                //////////////////////////////////////////////////////////////////////////

                                RadioButton Deletebutton = new RadioButton();
                                RadioButton DeleteInfo = new RadioButton();

                                DeleteInfo.Foreground = new SolidColorBrush(Colors.Transparent);
                                DeleteInfo.Background = new SolidColorBrush(Colors.Transparent);
                                DeleteInfo.BorderBrush = new SolidColorBrush(Colors.Transparent);
                                DeleteInfo.BorderThickness = new Thickness(0);
                                DeleteInfo.Style = (Style)FindResource("NoHover");
                                DeleteInfo.Content = account.Name.ToString();

                                Deletebutton.Margin = new Thickness(-100, 0, 0, 0);
                                Deletebutton.Style = (Style)FindResource("DarkenOnHoverRadioButtonStyle");
                                Deletebutton.Content = new Image
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/Images/Delete.png")),
                                };


                                Deletebutton.IsEnabled = false;
                                Deletebutton.Visibility = Visibility.Visible;
                                Deletebutton.SetValue(Grid.ColumnProperty, 3);
                                DeleteInfo.SetValue(Grid.ColumnProperty, 3);
                                DeleteInfo.Click += new RoutedEventHandler(RemoveMySQLUsers);

                                accountInfoGrid.Children.Add(Deletebutton);
                                accountInfoGrid.Children.Add(DeleteInfo);

                                addedUsernames.Add(account.Name);

                                await Task.Delay(100);
                            }
                            else
                            {
                                //MessageBox.Show($"Duplicate account found for: {account.Name}. Skipping...");
                            }
                        }
                    }
                }
            }
        }

        public async void RemoveUsers(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            SqlConnection con = new SqlConnection(connectionToTableString);
            con.Open();

            string sql = $"DELETE FROM [ArcadeDataBase].[dbo].[useraccounts] WHERE [Name] = '{radioButton.Content}'";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            con.Close();

            await GetUsers();
        }


        public async void RemoveMySQLUsers(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            MySqlConnection con = new MySqlConnection(connectionToSiteDatabaseString);
            con.Open();

            string sql = $"DELETE FROM UserAccounts WHERE `Name` = '{radioButton.Content}'";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            con.Close();

            await GetMySQLUsers();
        }

        public async Task RemoveUserTask()
        {

            for (int i = UserStackPanel.Children.Count; i > 0; i--)
            {
                //MessageBox.Show(i.ToString());

                if (UserStackPanel.Children.Count != 0)
                {
                    UserStackPanel.Children.RemoveAt(i - 1);
                }
            }
        }

        public async Task RemoveSQLUserTask()
        {

            for (int i = SQLUserStackPanel.Children.Count; i > 0; i--)
            {
                //MessageBox.Show(i.ToString());

                if (SQLUserStackPanel.Children.Count != 0)
                {
                    SQLUserStackPanel.Children.RemoveAt(i - 1);
                }
            }
        }

        public async void AddUser(object sender, RoutedEventArgs e)
        {
            AddUserToDatabase(SQLuser, SQLpass, SQLHeadAdmin);
        }

        public async void AddMySQLUser(object sender, RoutedEventArgs e)
        {
            AddUserToMySqlDatabase(AddLoginUser_UserText.Text, AddLoginUser_PasswordText.Text);
        }

        public void AddUserToDatabase(string name, string password, bool isHeadAdmin)
        {
            string insertQuery = "INSERT INTO useraccounts (Name, Password, isHeadAdmin) VALUES (@Name, @Password, @isHeadAdmin)";

            using (SqlConnection connection = new SqlConnection(connectionToTableString))
            {
                try
                {
                    connection.Open();

                    // Create a SqlCommand to execute the insert query
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        // Define the parameters and their values
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@isHeadAdmin", isHeadAdmin);

                        // Execute the insert command
                        int rowsAffected = command.ExecuteNonQuery();

                        // Check if the insert was successful
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User added successfully.");

                            GetUsers();
                        }
                        else
                        {
                            MessageBox.Show("No rows were affected.");
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Handle SQL errors
                    MessageBox.Show($"SQL Error: {ex.Message}");
                }
            }
        }

        public async void AddUserToMySqlDatabase(string name, string password)
        {
            string insertQuery = "INSERT INTO UserAccounts (Name, Password) VALUES (@Name, @Password)";

            using (MySqlConnection connection = new MySqlConnection(connectionToSiteDatabaseString))
            {
                try
                {
                    connection.Open();

                    // Create a SqlCommand to execute the insert query
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        // Define the parameters and their values
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Password", password);

                        // Execute the insert command
                        int rowsAffected = command.ExecuteNonQuery();

                        // Check if the insert was successful
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User added successfully.");

                           await GetMySQLUsers();
                        }
                        else
                        {
                            MessageBox.Show("No rows were affected.");
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Handle SQL errors
                    MessageBox.Show($"SQL Error: {ex.Message}");
                }
            }
        }

        public async void ChangeUsername(object sender, RoutedEventArgs e)
        {
            MySqlConnection con = new MySqlConnection(connectionToSiteDatabaseString);
            con.Open();

            string sql = $"UPDATE `UserAccounts` SET `Name` = '{NewUsernameField.Text}' WHERE `Name` = '{panelLoginUsername}'";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            con.Close();

            await GetMySQLUsers();

            panelLoginUsername = NewUsernameField.Text;
        }

        public async void ChangePassword(object sender, RoutedEventArgs e)
        {
            MySqlConnection con = new MySqlConnection(connectionToSiteDatabaseString);
            con.Open();

            string sql = $"UPDATE `UserAccounts` SET `Password` = '{NewPasswordField.Text}' WHERE `Password` = '{panelLoginPassword}'";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            con.Close();

            await GetMySQLUsers();

            panelLoginPassword = NewPasswordField.Text;
        }

        #endregion

        #endregion

        #region FTP

        public async void UploadFile(object sender, RoutedEventArgs e)
        {
            if (ftp == null || user == null || pass == null)
            {
                ftp = "ftp://192.168.1.33";
                user = "anonymous";
                pass = "anonymous";
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = @"" + ftp + "/",
                UseShellExecute = true
            });
        }

        public async void FTPMakeConnection(object sender, RoutedEventArgs e)
        {
            if (ftp != null || user != null || pass != null)
            {
                using (var conn = new FtpClient(ftp, user, pass))
                {
                    conn.Config.EncryptionMode = FtpEncryptionMode.None;
                    conn.Config.ValidateAnyCertificate = false;
                    try
                    {
                        conn.Connect();

                        if (conn.IsConnected)
                        {
                            ColorAnimation colorAnimation = new ColorAnimation
                            {
                                From = ((SolidColorBrush)FTPEcclipse.Fill).Color,
                                To = (Color)ColorConverter.ConvertFromString("#006400"),
                                Duration = new Duration(TimeSpan.FromSeconds(colorFadeTime)),
                                AutoReverse = false,
                            };

                            SolidColorBrush brush = (SolidColorBrush)FTPEcclipse.Fill;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                            await Task.Delay(1500);

                            MakeFTPConnection.IsEnabled = false;
                            FTPEnter.IsEnabled = false;
                            FTPIPtext.IsEnabled = false;
                            FTPUserText.IsEnabled = false;
                            FTPPasswordText.IsEnabled = false;
                            FTPIPtext.Visibility = Visibility.Collapsed;
                            FTPUserText.Visibility = Visibility.Collapsed;
                            FTPPasswordText.Visibility = Visibility.Collapsed;
                            FTPEcclipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                            FTPButton.IsEnabled = true;

                            if (MakeFTPConnection.IsEnabled == false && MakeSQLConnection.IsEnabled == false)
                            {
                                ConnectButton.IsEnabled = false;
                                ConnectPanel.IsEnabled = false;
                                InterfacePanel.IsEnabled = true;
                                UIButton.IsChecked = true;
                            }
                            DisableRadioButtonsInWindow();

                            MessageBoxResult Result = MessageBox.Show("Would you like to set this as your default connection?", "Set as default connection?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            string filepath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\ArcadeInfo\DefaultConnection.txt";

                            if (Result == MessageBoxResult.Yes)
                            {
                                TextReader tr = new StreamReader(filepath);
                                if(SQLIPtext.Text != tr.ReadLine())
                                {
                                    tr.Close();
                                    TextWriter tw = new StreamWriter(filepath);
                                    tw.WriteLine(SQLIPtext.Text);
                                    tw.Close();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ColorAnimation colorAnimation = new ColorAnimation
                        {
                            From = ((SolidColorBrush)FTPEcclipse.Fill).Color,
                            To = (Color)ColorConverter.ConvertFromString("#8B0000"),
                            Duration = new Duration(TimeSpan.FromSeconds(colorFadeTime)),
                            AutoReverse = false,
                        };

                        SolidColorBrush brush = (SolidColorBrush)FTPEcclipse.Fill;
                        brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                        MakeFTPConnection.IsEnabled = true;
                        FTPButton.IsEnabled = false;
                        DisableRadioButtonsInWindow();
                    }
                    finally
                    {
                        conn.Disconnect();
                    }
                }
            }
        }

        public async Task FTPMakeConnection()
        {
            using (var conn = new FtpClient(ftp, user, pass))
            {
                conn.Config.EncryptionMode = FtpEncryptionMode.None;
                conn.Config.ValidateAnyCertificate = false;
                try
                {
                    conn.Connect();

                    if (conn.IsConnected)
                    {
                        await this.Dispatcher.InvokeAsync(async () =>
                        {
                            ColorAnimation colorAnimation = new ColorAnimation
                            {
                                From = ((SolidColorBrush)FTPEcclipse.Fill).Color,
                                To = (Color)ColorConverter.ConvertFromString("#006400"),
                                Duration = new Duration(TimeSpan.FromSeconds(colorFadeTime)),
                                AutoReverse = false,
                            };

                            SolidColorBrush brush = (SolidColorBrush)FTPEcclipse.Fill;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                            await Task.Delay(1500);

                            ConnectedToFTP = true;
                            MakeFTPConnection.IsEnabled = false;
                            FTPEnter.IsEnabled = false;
                            FTPIPtext.IsEnabled = false;
                            FTPUserText.IsEnabled = false;
                            FTPPasswordText.IsEnabled = false;
                            FTPIPtext.Visibility = Visibility.Collapsed;
                            FTPUserText.Visibility = Visibility.Collapsed;
                            FTPPasswordText.Visibility = Visibility.Collapsed;
                            FTPEcclipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                            FTPButton.IsEnabled = true;

                            if (ConnectedToFTP == true && ConnectedToSQL == true)
                            {
                                ConnectButton.IsEnabled = false;
                                ConnectPanelLoadingGif.IsEnabled = false;
                                InterfacePanel.IsEnabled = true;
                                UIButton.IsChecked = true;
                            }
                            await DisableRadioButtonsInWindow();
                        });
                    }
                }
                catch (Exception ex)
                {
                    await this.Dispatcher.InvokeAsync(async() =>
                    {
                        ftpConnectionAttempts++;
                        CheckStandardPorts();

                        MakeFTPConnection.IsEnabled = true;
                        FTPButton.IsEnabled = false;

                        await DisableRadioButtonsInWindow();
                    });
                }
                finally
                {
                    conn.Disconnect();
                }
            }
        }
        public void FTPConnectionCheck(object sender, RoutedEventArgs e)
        {
            if (ftp == null || user == null || pass == null)
            {
                ftp = "ftp://192.168.1.33";
                user = "anonymous";
                pass = "anonymous";
            }

            using (var conn = new FtpClient(ftp, user, pass))
            {
                conn.Config.EncryptionMode = FtpEncryptionMode.None;
                conn.Config.ValidateAnyCertificate = false;
                try
                {
                    conn.Connect();

                    if (conn.IsConnected)
                    {
                        ColorAnimation colorAnimation = new ColorAnimation
                        {
                            From = ((SolidColorBrush)myEllipse.Fill).Color,
                            To = (Color)ColorConverter.ConvertFromString("#006400"),
                            Duration = new Duration(TimeSpan.FromSeconds(colorFadeTime)),
                            AutoReverse = false,
                        };

                        SolidColorBrush brush = (SolidColorBrush)myEllipse.Fill;
                        brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
                    }
                }
                catch (Exception ex)
                {
                    ColorAnimation colorAnimation = new ColorAnimation
                    {
                        From = ((SolidColorBrush)myEllipse.Fill).Color,
                        To = (Color)ColorConverter.ConvertFromString("#8B0000"),
                        Duration = new Duration(TimeSpan.FromSeconds(colorFadeTime)),
                        AutoReverse = false,
                    };

                    SolidColorBrush brush = (SolidColorBrush)myEllipse.Fill;
                    brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
                }
                finally
                {
                    conn.Disconnect();
                }
            }
        }

        public void GetWorkingDirectory(object sender, RoutedEventArgs e)
        {
            using (var conn = new FtpClient(ftp, user, pass))
            {
                conn.Connect();

                foreach (var item in conn.GetListing())
                {
                    //doe iets met de folder/itemnames
                }
            }
        }

        public void CreateDirectory(object sender, RoutedEventArgs e)
        {
            using (var conn = new FtpClient(ftp, user, pass))
            {
                conn.Connect();

                conn.CreateDirectory("/test", true);
            }
        }

        #endregion

        #region Etc

        private async Task CheckStandardPorts()
        {
            if (ftpConnectionAttempts == 0 && sqlConnectionAttempts == 0)
            {
                MessageBox.Show("Checking default connection please wait");

                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ArcadeInfo");
                string filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ArcadeInfo", "DefaultConnection.txt");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                else
                {
                    using (TextReader tr = new StreamReader(filepath))
                    {
                        defaultConnection = await tr.ReadLineAsync();
                        tr.Close();
                    }
                }
            }

            if (ftpConnectionAttempts == 0)
            {
                ftp = "ftp://192.168.1.33";
                user = "anonymous";
                pass = "anonymous";
            }
            else if (ftpConnectionAttempts == 1 && defaultConnection != "")
            {
                ftp = $"ftp://{defaultConnection}";
                user = "anonymous";
                pass = "anonymous";
            }

            if (sqlConnectionAttempts == 0)
            {
                connectionToDatabaseString = "Server=192.168.1.33,54469\\SQLEXPRESS;Database=master;User Id=User;Password=Pass;";
                connectionToTableString = "Server=192.168.1.33,54469\\SQLEXPRESS; Database=ArcadeDataBase;User Id=User;Password=Pass;";
            }
            else if (sqlConnectionAttempts == 1 && defaultConnection != "")
            {
                connectionToDatabaseString = $"Server={defaultConnection},54469\\SQLEXPRESS;Database=master;User Id=User;Password=Pass;";
                connectionToTableString = $"Server={defaultConnection},54469\\SQLEXPRESS; Database=ArcadeDataBase;User Id=User;Password=Pass;";
            }

            if(sqlConnectionAttempts == 2 && ftpConnectionAttempts == 2)
            {
                MessageBox.Show("Couldn't find default connection. Try entering it manually.");
                ConnectPanelLoadingGif.IsEnabled = false;
                ConnectPanel.IsEnabled = true;
            }

            // Run tasks asynchronously for SQL and FTP connections
            if (sqlConnectionAttempts != 2)
            {
                await Task.Run(SQLMakeConnection);
            }
            if (ftpConnectionAttempts != 2)
            {
                await Task.Run(FTPMakeConnection);
            }
        }


        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    return (T)child;
                }

                var result = FindVisualChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public async Task DisableAllRadioButtons(DependencyObject parent)
        {
            this.Dispatcher.Invoke(() =>
            {
                // Check if the parent is a valid DependencyObject
                if (parent == null)
                    return;

                // Iterate over all the child elements of the parent
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);

                    // Check if the child is a RadioButton
                    if (child is RadioButton radioButton)
                    {
                        string buttonName = radioButton.Content.ToString();
                        if (radioButton.IsEnabled == false)
                        {
                            radioButton.Content = "";
                            radioButton.IsHitTestVisible = false;
                            radioButton.IsChecked = false;
                        }
                        else if (radioButton.IsEnabled == true)
                        {
                            for (int j = 0; j < radioButtonList.Count; j++)
                            {
                                if (radioButton == radioButtonList[j])
                                {
                                    radioButton.Content = radioButtonContentList[j];
                                    radioButton.IsHitTestVisible = true;
                                }
                            }
                        }
                    }

                    // Recursively call this method for each child
                    DisableAllRadioButtons(child);
                }
            });

        }

        private async Task DisableRadioButtonsInWindow()
        {
            // Call this method with the root element of your window or container
            await DisableAllRadioButtons(this); // 'this' refers to the Window or UserControl instance
        }

        public async Task SaveAllRadioButtons(DependencyObject parent)
        {
            // Check if the parent is a valid DependencyObject
            if (parent == null)
                return;

            // Iterate over all the child elements of the parent
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // Check if the child is a RadioButton
                if (child is RadioButton radioButton)
                {
                    radioButtonList.Add(radioButton);
                    radioButtonContentList.Add(radioButton.Content.ToString());
                }

                // Recursively call this method for each child
                SaveAllRadioButtons(child);
            }
        }

        private async Task SaveRadioButtonsInWindow()
        {
            // Call this method with the root element of your window or container
            SaveAllRadioButtons(this); // 'this' refers to the Window or UserControl instance
        }

        #endregion
    }

    public class UserAccount
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsHeadAdmin { get; set; }
    }

    public class MySQLUserAccount
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class NotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrEmpty(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}

