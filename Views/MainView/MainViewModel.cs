using GrblController.Services;
using GrblController.ViewModels;
using GrblController.ViewModels.Command;
using GrblController.Views.PopView;
using System;
using System.ComponentModel;

namespace GrblController.Views.MainView
{
    class MainViewModel : ViewModelBase
    {
        public SerialViewModel GrblSerialViewModel { get; set; }
        public TimerViewModel GrblTimerViewModel { get; set; }

        public GetDataService grblGetDataService;

        public OxyPlotViewModel SensorValue { get; set; }
        public CsvViewModel CsvViewModel { get; set; }
        public DatabaseViewModel DatabaseViewModel { get; set; }
        public SerialViewModel SerialViewModel { get; set; }
        public TimerViewModel TimerViewModel { get; set; }

        public GetDataService GetDataService;

        private string commandLine;
        public string CommandLine
        {
            get => commandLine; set => SetProperty(ref commandLine, value);

        }

        private string x_axis;
        public string X_axis
        {
            get => x_axis; 
            set
            {
                if (int.TryParse(value, out _))
                {
                    SetProperty(ref x_axis, value);
                }
                else
                {
                    return;
                }
            }
        }

        private string y_axis;
        public string Y_axis
        {
            get => y_axis;
            set
            {
                if (int.TryParse(value, out _))
                {
                    SetProperty(ref y_axis, value);
                }
                else
                {
                    return;
                }

            }
        }

        private string z_axis;
        public string Z_axis
        {
            get => z_axis;
            set
            {
                if (int.TryParse(value, out _))
                {
                    SetProperty(ref z_axis, value);
                }
                else
                {
                    return;
                }

            }
        }

        private string speed;
        public string Speed
        {
            get => speed;
            set
            {
                if (int.TryParse(value, out _))
                {
                    SetProperty(ref speed, value);
                }
                else
                {
                    return;
                }

            }
        }

        private bool graphState;
        public bool GraphState
        {
            get => graphState; set => SetProperty(ref graphState, value);
        }

        private string graphContent;
        public string GraphContent
        {
            get => graphContent; set => SetProperty(ref graphContent, value);
        }

        private string receivedData;
        public string ReceivedData
        {
            get => receivedData; set => SetProperty(ref receivedData, value);
        }


        public RelayCommand GraphCommand { get; set; }
        public RelayCommand GraphClearCommand { get; set; }
        public RelayCommand ResetCommand { get; set; }
        public RelayCommand WriteCommand { get; set; }
        public RelayCommand IncreaseXCommand { get; set; }
        public RelayCommand DecreaseXCommand { get; set; }
        public RelayCommand IncreaseYCommand { get; set; }
        public RelayCommand DecreaseYCommand { get; set; }
        public RelayCommand IncreaseZCommand { get; set; }
        public RelayCommand DecreaseZCommand { get; set; }
        public RelayCommand DeXInYCommand { get; set; }
        public RelayCommand InXInYCommand { get; set; }
        public RelayCommand DeXDeYCommand { get; set; }
        public RelayCommand InXDeYCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand HomeCommand { get; set; }
        public RelayCommand PortCommand { get; set; }
        public RelayCommand PauseCommand { get; set; }

        private double dataCount = 1;


        public MainViewModel()
        {
            GrblTimerViewModel = new TimerViewModel();
            grblGetDataService = new GetDataService();
            GrblSerialViewModel = new SerialViewModel(GrblTimerViewModel, grblGetDataService);
            grblGetDataService.Method += GrblDataReceived;

            TimerViewModel = new TimerViewModel();
            GetDataService = new GetDataService();
            SerialViewModel = new SerialViewModel(TimerViewModel, GetDataService);
            GetDataService.Method += DataReceived;

            SensorValue = new OxyPlotViewModel("Sensor Value");

            string line = $"{"Time"},{"value"}";
            CsvViewModel = new CsvViewModel(line);
            DatabaseViewModel = new DatabaseViewModel();

            GraphState = true;
            GraphContent = "Stop";

            GraphCommand = new RelayCommand(GraphToggle);
            GraphClearCommand = new RelayCommand(ClearGraph);
            ResetCommand = new RelayCommand(Reset);
            WriteCommand = new RelayCommand(SendSerial);
            IncreaseXCommand = new RelayCommand(IncreaseX);
            DecreaseXCommand = new RelayCommand(DecreaseX);
            IncreaseYCommand = new RelayCommand(IncreaseY);
            DecreaseYCommand = new RelayCommand(DecreaseY);
            IncreaseZCommand = new RelayCommand(IncreaseZ);
            DecreaseZCommand = new RelayCommand(DecreaseZ);
            DeXInYCommand = new RelayCommand(DeXInY);
            InXInYCommand = new RelayCommand(InXInY);
            DeXDeYCommand = new RelayCommand(DeXDeY);
            InXDeYCommand = new RelayCommand(InXDeY);
            SaveCommand = new RelayCommand(SaveValue);
            HomeCommand = new RelayCommand(Home);
            PortCommand = new RelayCommand(PortUpdate);
            PauseCommand = new RelayCommand(Pause);
            Reset();

        }



        private void PortUpdate()
        {
            GrblSerialViewModel.LoadSerial();
            SerialViewModel.LoadSerial();
        }

        private void DataReceived()
        {
            string data = GetDataService.StringData;
            if (double.TryParse(data, out double value))
            {
                SensorValue.Output = value;
                SensorValue.GrpahUpdate(dataCount, GraphState);

                dataCount++;
            }

        }

        private void SaveValue()
        {
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("yy/MM/dd HH:mm:ss");

            if (CsvViewModel.CsvState)
            {
                CsvViewModel.Add(formattedTime, SensorValue.Output);
            }

            if (DatabaseViewModel.MysqlState)
            {
                DatabaseViewModel.AddDatabase(formattedTime, SensorValue.Output);
            }
        }

        private void GrblDataReceived()
        {
            ReceivedData += grblGetDataService.StringData;
        }

        private void SendSerial()
        {
            if (CommandLine != "")
            {
                GrblSerialViewModel.SendSerial(CommandLine);
                ReceivedData += CommandLine + "\n";
            }
        }
        public void Reset()
        {
            ReceivedData = "";
            CommandLine = "";
            X_axis = "1";
            Y_axis = "1";
            Z_axis = "1";
            Speed = "1000";

        }
        private void ClearGraph()
        {
            dataCount = 0;
            SensorValue.GrahpClear();

        }
        private void GraphToggle()
        {
            if (GraphState)
            {
                GraphState = !GraphState;
                GraphContent = "Live";
            }
            else
            {
                GraphState = !GraphState;
                GraphContent = "Stop";
            }
        }

        public void IncreaseX()
        {
            string command = "$J=G21G91X" + X_axis + "F" + Speed;
            ReceivedData += "X축 " + X_axis + " 이동 \n";
            GrblSerialViewModel.SendSerial(command);
        }
        public void DecreaseX()
        {
            string command = "$J=G21G91X-" + X_axis + "F" + Speed;
            ReceivedData += "X축 -" + X_axis + " 이동 \n";
            GrblSerialViewModel.SendSerial(command);
        }

        public void IncreaseY()
        {

            string command = "$J=G21G91Y" + Y_axis + "F" + Speed;
            ReceivedData += "Y축 " + Y_axis + " 이동 \n";
            GrblSerialViewModel.SendSerial(command);
        }

        public void DecreaseY()
        {
            string command = "$J=G21G91Y-" + Y_axis + "F" + Speed;
            ReceivedData += "Y축 -" + Y_axis + " 이동 \n";
            GrblSerialViewModel.SendSerial(command);
        }

        public void IncreaseZ()
        {
            string command = "$J=G21G91Z" + Z_axis + "F" + Speed;
            ReceivedData += "Z축 " + Z_axis + " 이동 \n";
            GrblSerialViewModel.SendSerial(command);
        }

        public void DecreaseZ()
        {
            string command = "$J=G21G91Z-" + Z_axis + "F" + Speed;
            ReceivedData += "Z축 -" + Z_axis + " 이동 \n";
            GrblSerialViewModel.SendSerial(command);
        }

        public void DeXInY()
        {
            string command = "$J=G21G91X-" + X_axis + "Y" + Y_axis + "F" + Speed;
            ReceivedData += "X축 -" + X_axis + " Y축 " + Y_axis + " 이동 \n";
            GrblSerialViewModel.SendSerial(command);
        }

        public void InXInY()
        {
            string command = "$J=G21G91X" + X_axis + "Y" + Y_axis + "F" + Speed;
            ReceivedData += "X축 " + X_axis + " Y축 " + Y_axis + " 이동 \n";
            GrblSerialViewModel.SendSerial(command);
        }

        public void DeXDeY()
        {
            string command = "$J=G21G91X-" + X_axis + "Y-" + Y_axis + "F" + Speed;
            ReceivedData += "X축 -" + X_axis + " Y축 -" + Y_axis + " 이동 \n";
            GrblSerialViewModel.SendSerial(command);
        }

        public void InXDeY()
        {
            string command = "$J=G21G91X" + X_axis + "Y-" + Y_axis + "F" + Speed;
            ReceivedData += "X축 " + X_axis + " Y축 " + Y_axis + " 이동 \n";
            GrblSerialViewModel.SendSerial(command);
        }
        private void Home()
        {
            string command = "G28";
            ReceivedData += "Home \n";
            GrblSerialViewModel.SendSerial(command);
        }
        private void Pause()
        {
            string command = "!";
            ReceivedData += "!!! Pause !!! \n";
            GrblSerialViewModel.SendSerial(command);

            string command2 = "~";
            GrblSerialViewModel.SendSerial(command2);
        }

        
    }
}
