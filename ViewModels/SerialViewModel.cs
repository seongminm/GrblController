using GrblController.Services;
using GrblController.ViewModels.Command;
using System.Collections.Generic;
using System.ComponentModel;

namespace GrblController.ViewModels
{
    class SerialViewModel : INotifyPropertyChanged
    {
        private RelayCommand serialCommand;
        public RelayCommand SerialCommand
        {
            get { return serialCommand; }
            set
            {
                serialCommand = value;
                OnPropertyChanged(nameof(SerialCommand));
            }
        }
        public RelayCommand SerialPortCommand { get; set; }

        private string _serialContent;
        public string SerialContent
        {
            get { return _serialContent; }
            set
            {
                _serialContent = value;
                OnPropertyChanged(nameof(SerialContent));
            }
        }

        private bool _serialState;
        public bool SerialState
        {
            get { return _serialState; }
            set
            {
                _serialState = value;
                SerialToggleState = !_serialState;
                OnPropertyChanged(nameof(SerialState));
            }
        }

        private bool serialToggleState;
        public bool SerialToggleState
        {
            get { return serialToggleState; }
            set
            {
                serialToggleState = value;
                OnPropertyChanged(nameof(SerialToggleState));
            }
        }
        private string _selectedSerialPort;
        public string SelectedSerialPort
        {
            get { return _selectedSerialPort; }
            set
            {
                _selectedSerialPort = value;
                OnPropertyChanged(nameof(SelectedSerialPort));
            }
        }

        private string _selectedSerialBaudRate;
        public string SelectedSerialBaudRate
        {
            get { return _selectedSerialBaudRate; }
            set
            {
                _selectedSerialBaudRate = value;
                OnPropertyChanged(nameof(SelectedSerialBaudRate));
            }
        }

        private List<string> serialPorts;
        public List<string> SerialPorts
        {
            get
            {
                return serialPorts;
            }  
            set
            {
                serialPorts = value;
                OnPropertyChanged(nameof(SerialPorts));
            }
            
        }

        public List<int> SerialBaudRate { get; set; }


        private SerialService serialService;
        private GetDataService getDataService;

        TimerViewModel timerViewModel;

        public SerialViewModel(TimerViewModel timerViewModel, GetDataService getDataService)
        {

            this.getDataService = getDataService;
            this.timerViewModel = timerViewModel;

            serialService = new SerialService(this.getDataService);
            SerialCommand = new RelayCommand(OpenSerial);
            
            SerialContent = "Open";
            SerialState = true;
            SerialToggleState = false;
            SerialPorts = serialService.SerialPorts;
            SerialBaudRate = serialService.SerialBaudRate;
            SerialPortCommand = new RelayCommand(LoadSerial);

        }

        public void LoadSerial()
        {
            serialService.LoadSerialPorts();
            SerialPorts = serialService.SerialPorts;
        }

        private void OpenSerial()
        {

            serialService.OpenSerial(SelectedSerialPort, SelectedSerialBaudRate);
            if (serialService.isOpen())
            {
                SerialCommand = new RelayCommand(CloseSerial);
                SerialContent = "Close";
                SerialState = false;
                timerViewModel.Start();
            }
        }

        public void CloseSerial()
        {
            serialService.CloseSerial();
            if (!serialService.isOpen())
            {
                SerialCommand = new RelayCommand(OpenSerial);
                SerialContent = "Open";
                SerialState = true;
                timerViewModel.Stop();
            }
        }

        public void SendSerial(string message)
        {
            if(serialService.isOpen())
            {
                serialService.SendSerial(message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
