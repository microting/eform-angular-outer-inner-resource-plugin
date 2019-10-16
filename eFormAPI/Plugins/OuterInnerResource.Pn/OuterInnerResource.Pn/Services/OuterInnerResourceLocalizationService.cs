using Microsoft.Extensions.Localization;
using Microting.eFormApi.BasePn.Localization.Abstractions;
using OuterInnerResource.Pn.Abstractions;

namespace OuterInnerResource.Pn.Services
{
    public class OuterInnerResourceLocalizationService : IOuterInnerResourceLocalizationService
    {
        private readonly IStringLocalizer _localizer;
 
        // ReSharper disable once SuggestBaseTypeForParameter
        public OuterInnerResourceLocalizationService(IEformLocalizerFactory factory)
        {
            _localizer = factory.Create(typeof(EformOuterInnerResourcePlugin));
        }
 
        public string GetString(string key)
        {
            LocalizedString str = _localizer[key];
            return str.Value;
        }

        public string GetString(string format, params object[] args)
        {
            LocalizedString message = _localizer[format];
            if (message?.Value == null)
            {
                return null;
            }

            return string.Format(message.Value, args);
        }
    }
}
