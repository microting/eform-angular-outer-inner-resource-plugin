namespace MachineArea.Pn.Abstractions
{
    public interface IMachineAreaLocalizationService
    {
        string GetString(string key);
        string GetString(string format, params object[] args);
    }
}