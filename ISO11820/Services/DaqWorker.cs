using ISO11820.Models;

namespace ISO11820.Services;

public class DaqWorker : IDisposable
{
    private readonly System.Timers.Timer _timer;
    private readonly SensorSimulator _simulator;
    private readonly SimulationConfig _config;
    private Dictionary<string, double> _temperatures;
    private readonly List<double> _furnaceTempHistory = new();
    private readonly List<MasterMessage> _pendingMessages = new();
    private double _accumulatedMs;

    public event EventHandler<DataBroadcastEventArgs>? DataBroadcast;
    public event Action? SecondElapsed;

    public Dictionary<string, double> Temperatures => _temperatures;
    public TestState CurrentState { get; set; } = TestState.Idle;
    public int ElapsedSeconds { get; private set; }

    public DaqWorker(SensorSimulator simulator, SimulationConfig config)
    {
        _simulator = simulator;
        _config = config;
        _temperatures = simulator.GetInitialTemperatures();
        _timer = new System.Timers.Timer(800);
        _timer.Elapsed += OnTick;
        _timer.AutoReset = true;
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();
    public void ResetElapsed() { ElapsedSeconds = 0; _accumulatedMs = 0; }

    private void OnTick(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (_config.EnableSimulation)
            _temperatures = _simulator.Update(_temperatures, CurrentState);

        _furnaceTempHistory.Add(_temperatures["TF1"]);
        if (_furnaceTempHistory.Count > 600) _furnaceTempHistory.RemoveAt(0);

        double drift = DriftCalculator.CalculateDrift(_furnaceTempHistory);

        _accumulatedMs += 800;
        if (_accumulatedMs >= 1000)
        {
            _accumulatedMs -= 1000;
            if (CurrentState == TestState.Recording)
                ElapsedSeconds++;
            SecondElapsed?.Invoke();
        }

        var args = new DataBroadcastEventArgs
        {
            Temperatures = new Dictionary<string, double>(_temperatures),
            CurrentState = CurrentState.ToString(),
            ElapsedSeconds = ElapsedSeconds,
            IsStable = false,
            Drift = drift,
            Messages = new List<MasterMessage>(_pendingMessages)
        };

        DataBroadcast?.Invoke(this, args);
        _pendingMessages.Clear();
    }

    public void AddMessage(string message)
    {
        _pendingMessages.Add(new MasterMessage
        {
            Time = DateTime.Now.ToString("HH:mm:ss"),
            Message = message
        });
    }

    public List<double> GetFurnaceTempHistory() => new(_furnaceTempHistory);

    public double GetCurrentDrift() => DriftCalculator.CalculateDrift(_furnaceTempHistory);

    public void Dispose() => _timer.Dispose();
}
