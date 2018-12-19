using System.Reflection;
using MachineArea.Pn.Abstractions;
using Microsoft.Extensions.Localization;
using Microting.eFormApi.BasePn.Localization.Abstractions;

namespace MachineArea.Pn.Services
{
    public class MachineAreaLocalizationService : IMachineAreaLocalizationService
    {
        private readonly IStringLocalizer _localizer;
 
        public MachineAreaLocalizationService(IEformLocalizerFactory factory)
        {
            _localizer = factory.Create(typeof(EformMachineAreaPlugin));
        }
 
        public string GetString(string key)
        {
            var str = _localizer[key];
            return str.Value;
        }

        public string GetString(string format, params object[] args)
        {
            var message = _localizer[format];
            if (message?.Value == null)
            {
                return null;
            }

            return string.Format(message.Value, args);
        }
    }
}
