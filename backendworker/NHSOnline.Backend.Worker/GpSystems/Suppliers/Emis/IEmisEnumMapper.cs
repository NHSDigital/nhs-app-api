using NHSOnline.Backend.Worker.Areas.SharedModels;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public interface IEmisEnumMapper
    {
        Necessity MapNecessity(string emisNecessity, Necessity? defaultValue);
    }
}
