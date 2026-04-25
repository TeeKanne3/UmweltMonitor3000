using CommunityToolkit.Mvvm.ComponentModel;

namespace UmweltMonitor3000.Application.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MqttViewModel MqttViewModel { get; }
    public PlantViewModel PlantViewModel { get; }
    public StatisticViewModel StatisticViewModel { get; }

    public MainViewModel()
    {
        var logic = new MainWindowLogic();
        MqttViewModel = new MqttViewModel(logic);
        PlantViewModel = new PlantViewModel(logic);
        StatisticViewModel = new StatisticViewModel(logic, PlantViewModel);
    }
}
