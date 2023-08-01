using GrblController.Services;
using GrblController.ViewModels.Command;
using System.ComponentModel;

namespace GrblController.ViewModels
{
    class CsvViewModel : INotifyPropertyChanged
    {
        private CsvService csvService;



        private bool csvState;
        public bool CsvState
        {
            get { return csvState; }
            set
            {
                csvState = value;
                OnPropertyChanged(nameof(CsvState));
            }
        }


        private string line;

        private RelayCommand _csvCommand;
        public RelayCommand CsvCommand
        {
            get { return _csvCommand; }
            set
            {
                _csvCommand = value;
                OnPropertyChanged(nameof(CsvCommand));
            }
        }

        public CsvViewModel(string line)
        {
            this.line = line;
            csvService = new CsvService();
            CsvCommand = new RelayCommand(Open);
            CsvState = false;
        }

        public void Open()
        {
            if (CsvState = csvService.CreateCsv(line))
            {
                CsvCommand = new RelayCommand(Close);
            }

        }
        public void Close()
        {
            CsvState = csvService.CloseCsv();
            CsvCommand = new RelayCommand(Open);
        }
        public void Add(string timer, double data)
        {
            csvService.AddCsv(timer, data);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
