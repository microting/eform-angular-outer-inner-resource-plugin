namespace OuterInnerResource.Pn.Abstractions
{
    public interface IOuterInnerResourceLocalizationService
    {
        string GetString(string key);
        string GetString(string format, params object[] args);
    }
}