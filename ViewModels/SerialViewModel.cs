using GrblController.Services;
using GrblController.ViewModels.Command;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows;

namespace GrblController.ViewModels
{
    class SerialViewModel : ViewModelBase
    {
        SerialPort serialPort;
        GetDataService getDataService;

        private RelayCommand serialCommand;
        public RelayCommand SerialCommand
        {
            get => serialCommand;   
            set => SetProperty(ref serialCommand, value);
        }
        public RelayCommand SerialPortCommand { get; set; }

        private string _serialContent;
        public string SerialContent
        {
            get => _serialContent; set => SetProperty(ref _serialContent, value);
        }

        private bool _serialState;
        public bool SerialState
        {
            get => _serialState; 
            set
            {
                SetProperty(ref _serialState, value);
                SerialToggleState = !_serialState;
            }
        }

        private bool serialToggleState;
        public bool SerialToggleState
        {
            get => serialToggleState; set => SetProperty(ref serialToggleState, value);
        }
        private string _selectedSerialPort;
        public string SelectedSerialPort
        {
            get => _selectedSerialPort; set => SetProperty(ref _selectedSerialPort, value);
        }

        private string _selectedSerialBaudRate;
        public string SelectedSerialBaudRate
        {
            get => _selectedSerialBaudRate; set => SetProperty(ref _selectedSerialBaudRate, value);
        }

        private List<string> serialPorts;
        public List<string> SerialPorts
        {
            get => serialPorts; set => SetProperty(ref serialPorts, value);
            
        }

        public List<int> SerialBaudRate { get; set; }

        TimerViewModel timerViewModel;

        public SerialViewModel(TimerViewModel timerViewModel, GetDataService getDataService)
        {

            this.getDataService = getDataService;
            this.timerViewModel = timerViewModel;

            SerialCommand = new RelayCommand(OpenSerial);
            
            SerialContent = "Open";
            SerialState = true;
            SerialToggleState = false;
            SerialPorts = new List<string>(SerialPort.GetPortNames());
            SerialBaudRate = new List<int> { 9600, 14400, 19200, 38400, 57600, 115200 };
            SerialPortCommand = new RelayCommand(LoadSerial);
        }

        public void LoadSerial()
        {
            SerialPorts = new List<string>(SerialPort.GetPortNames());
        }

        private void OpenSerial()
        {
            try
            {
                serialPort = new SerialPort();
                serialPort.PortName = SelectedSerialPort;
                serialPort.BaudRate = int.Parse(SelectedSerialBaudRate);
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (serialPort.IsOpen)
            {
                SerialCommand = new RelayCommand(CloseSerial);
                SerialContent = "Close";
                SerialState = false;
                timerViewModel.Start();
            }
        }

        public void CloseSerial()
        {
            serialPort.DiscardInBuffer();
            serialPort.DataReceived -= SerialPort_DataReceived;
            try
            {
                serialPort.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            if (!serialPort.IsOpen)
            {
                SerialCommand = new RelayCommand(OpenSerial);
                SerialContent = "Open";
                SerialState = true;
                timerViewModel.Stop();
            }
        }

        public void SendSerial(string message)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine(message);
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (serialPort.BytesToRead > 0)
            {
                getDataService.StringData = serialPort.ReadExisting();
            }

        }

    }
}
