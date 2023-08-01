using GrblController.Models;
using GrblController.Services;
using GrblController.ViewModels.Command;
using System;
using System.ComponentModel;
using System.Windows;

namespace GrblController.Views.PopView
{
    class DatabaseViewModel : INotifyPropertyChanged
    {
        DatabasePopView databasePopView;
        DatabasePopViewModel databasePopViewModel;
        DatabaseModel databaseModel;

        private DataBaseService databaseService;

        private bool mysqlState;
        public bool MysqlState
        {
            get { return mysqlState; }
            set
            {
                mysqlState = value;
                OnPropertyChanged(nameof(MysqlState));
            }
        }


        private RelayCommand mysqlCommand;
        public RelayCommand MysqlCommand
        {
            get { return mysqlCommand; ; }
            set
            {
                mysqlCommand = value;
                OnPropertyChanged(nameof(MysqlCommand));
            }
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

            databaseService = new DataBaseService();

            MysqlState = databaseService.OpenDatabase(databaseModel.UserName, databaseModel.Password, databaseModel.Server, databaseModel.DatabaseServer, databaseModel.TableName);
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
            MysqlState = databaseService.CloseDatabase();
            if (!MysqlState)
            {
                MysqlCommand = new RelayCommand(OpenDatabase);
            }
        }

        public void AddDatabase(string timer, double data)
        {

            databaseService.AddData(timer, data);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
