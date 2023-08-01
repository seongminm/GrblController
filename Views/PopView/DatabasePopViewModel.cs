using GrblController.Models;
using GrblController.ViewModels.Command;

namespace GrblController.Views.PopView
{
    class DatabasePopViewModel
    {
        DatabaseModel _databaseModel;
        DatabaseViewModel _databaseViewModel;

        public string Server { get; set; }
        public string DatabaseServer { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TableName { get; set; }
        public RelayCommand SetCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public DatabasePopViewModel(DatabaseModel databaseModel, DatabaseViewModel dataBaseViewModel)
        {
            _databaseModel = databaseModel;
            _databaseViewModel = dataBaseViewModel;
            SetCommand = new RelayCommand(Set);
            CancelCommand = new RelayCommand(Close);

            Server = _databaseModel.Server;
            DatabaseServer = _databaseModel.DatabaseServer;
            UserName = _databaseModel.UserName;
            Password = _databaseModel.Password;
            TableName = _databaseModel.TableName;
        }

        private void Close()
        {
            _databaseViewModel.Close();
        }

        private void Set()
        {
            _databaseModel.Server = Server;
            _databaseModel.DatabaseServer = DatabaseServer;
            _databaseModel.UserName = UserName;
            _databaseModel.Password = Password;
            _databaseModel.TableName = TableName;
            _databaseModel.State = true;
            _databaseViewModel.Close();
        }


    }
}
