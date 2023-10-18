using GrblController.ViewModels;
using OxyPlot;
using OxyPlot.Series;
using System.ComponentModel;
using System.Configuration;

namespace GrblController.Views
{
    class OxyPlotViewModel : ViewModelBase
    {
        private double output;
        public double Output
        {
            get => output; 
            set => SetProperty(ref output, value);
        }

        public PlotModel PlotModel { get; set; }

        private LineSeries linePlotModel;


        public OxyPlotViewModel(string title)
        {
            PlotModel = new PlotModel { Title = title, TitleFontSize = 11 };
            linePlotModel = new LineSeries();

            PlotModel.Series.Add(linePlotModel);

        }

        public void GrpahUpdate(double x, bool state)
        {
            linePlotModel.Points.Add(new DataPoint(x, output));
            
            if(linePlotModel.Points.Count > 100)
            {
                linePlotModel.Points.RemoveAt(0);
            }
            PlotModel.InvalidatePlot(state);
        }


        public void GrahpClear()
        {
            linePlotModel.Points.Clear();
            PlotModel.InvalidatePlot(true);
            Output = 0;
        }

        

    }
}
