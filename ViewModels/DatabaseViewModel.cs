using GrblController.Models;
using GrblController.Services;
using GrblController.ViewModels;
using GrblController.ViewModels.Command;
using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Windows;

namespace GrblController.Views.PopView
{
    class DatabaseViewModel : ViewModelBase
    {
        DatabasePopView databasePopView;
        DatabasePopViewModel databasePopViewModel;
        DatabaseModel databaseModel;

        private MySqlConnection connection;
        private string tableName;

        private bool mysqlState;
        public bool MysqlState
        {
            get => mysqlState; 
            set => SetProperty(ref mysqlState, value);
        }


        private RelayCommand mysqlCommand;
        public RelayCommand MysqlCommand
        {
            get => mysqlCommand;
            set => SetProperty(ref mysqlCommand, value);
        }


        public DatabaseViewModel()
        {
            MysqlState = false;
            MysqlCommand = new RelayCommand(OpenDatabase);
            databaseModel = new DatabaseModel();
        }

        public void OpenDatabase()
        {
            databasePopView = new DatabasePopView();
            databasePopView.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            databaseModel.State = false;
            databaseModel.TableName = DateTime.Now.ToString("yyMMdd_HHmm");
            databasePopViewModel = new DatabasePopViewModel(databaseModel, this);
            databasePopView.DataContext = databasePopViewModel;
            databasePopView.ShowDialog();
            if (databaseModel.State == false)
            {
                Close();
                return;
            }

            MysqlState = OpenDatabase(databaseModel.UserName, databaseModel.Password, databaseModel.Server, databaseModel.DatabaseServer, databaseModel.TableName);
            if (MysqlState)
            {
                MysqlCommand = new RelayCommand(CloseDatabase);
            }
        }

        public void Close()
        {
            databasePopView.Close();
        }

        public void CloseDatabase()
        {
            MysqlState = CloseDB();
            if (!MysqlState)
            {
                MysqlCommand = new RelayCommand(OpenDatabase);
            }
        }

        public void AddDatabase(string timer, double data)
        {
            string insertDataQuery = "INSERT INTO " + tableName + " (Time, value) " +
                        "VALUES (@Time, @value);";
            MySqlCommand insertDataCommand = new MySqlCommand(insertDataQuery, connection);

            insertDataCommand.Parameters.AddWithValue("@Time", timer);
            insertDataCommand.Parameters.AddWithValue("@Value", data);
            insertDataCommand.ExecuteNonQuery();
        }


        public bool OpenDatabase(string _userName, string _pw, string _server, string _database, string _tableName)
        {
            try
            {
                string uid = _userName;
                string password = _pw;
                string server = _server;
                string database = _database;
                string defaultTableName = DateTime.Now.ToString("yyMMdd_HHmm");
                tableName = _tableName;
                string connectionString = $"server={server};database={database};uid={uid};password={password};";
                connection = new MySqlConnection(connectionString);
                string createTableQuery = "CREATE TABLE IF NOT EXISTS `" + tableName + "` (`Pk` INT NOT NULL AUTO_INCREMENT, " +
                    "`Time` VARCHAR(45) NULL, " +
                    "`Value` VARCHAR(45) NULL, " +
                    "PRIMARY KEY (`Pk`));";

                connection.Open();
                MySqlCommand createTableCommand = new MySqlCommand(createTableQuery, connection);
                createTableCommand.ExecuteNonQuery();

                MessageBox.Show(tableName + " Connect !");
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public bool CloseDB()
        {
            try
            {
                connection.Close();
                MessageBox.Show(tableName + " Disconnect !");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return true;
            }

        }
    }
}
