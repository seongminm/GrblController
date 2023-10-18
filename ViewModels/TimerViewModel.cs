using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace GrblController.ViewModels
{
    class TimerViewModel : ViewModelBase
    {
        private DispatcherTimer _timer;
        private int _seconds; // 초 단위 시간 변수 선언


        private string _timerContent;
        public string TimerContent
        {
            get => _timerContent; 
            set => SetProperty(ref _timerContent, value);
            
        }

        public TimerViewModel()
        {
            TimerContent = "00:00:00";

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _seconds = 0;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            _seconds++;
            TimerContent = TimeSpan.FromSeconds(_seconds).ToString(@"hh\:mm\:ss");
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _seconds = 0;
            TimerContent = "00:00:00";
        }

        
    }
}
