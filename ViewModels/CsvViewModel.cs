using GrblController.Services;
using GrblController.ViewModels.Command;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;

namespace GrblController.ViewModels
{
    class CsvViewModel : ViewModelBase
    {
        private string csvFilePath; // CSV 파일 경로
        private StreamWriter writer; // CSV 파일 작성자

        private bool csvState;
        public bool CsvState
        {
            get => csvState; set => SetProperty(ref csvState, value);
        }


        private string line;

        private RelayCommand _csvCommand;
        public RelayCommand CsvCommand
        {
            get => _csvCommand; set => SetProperty(ref _csvCommand, value);
        }

        public CsvViewModel(string line)
        {
            this.line = line;
            CsvCommand = new RelayCommand(Open);
            CsvState = false;
        }

        public void Open()
        {
            if (CsvState = CreateCsv(line))
            {
                CsvCommand = new RelayCommand(Close);
            }

        }
        public void Close()
        {
            CsvState = CloseCsv();
            CsvCommand = new RelayCommand(Open);
        }
        public void Add(string timer, double data)
        {
            AddCsv(timer, data);
        }



        public bool CreateCsv(string line)
        {
            string currentDate = DateTime.Now.ToString("yyMMdd_HHmm");

            var dialog = new SaveFileDialog
            {
                FileName = currentDate,
                Filter = "CSV Files (*.csv)|*.csv",
                DefaultExt = "csv",
                AddExtension = true
            };

            if (dialog.ShowDialog() == true)
            {
                // 선택한 경로로 CSV 파일을 저장합니다.
                csvFilePath = dialog.FileName;

                writer = new StreamWriter(csvFilePath, true, Encoding.UTF8);
                writer.WriteLine(line);
                writer.Close();
                MessageBox.Show("CSV file saved successfully.");
                return true;
            }

            return false;
        }

        public void AddCsv(string timer, double data)
        {
            writer = new StreamWriter(csvFilePath, true, Encoding.UTF8);
            string result = timer;
            result += "," + data;
            writer.WriteLine(result);
            writer.Close();
        }

        public bool CloseCsv()
        {
            MessageBox.Show(csvFilePath + " Disconnect !");
            return false;
        }



    }
}
