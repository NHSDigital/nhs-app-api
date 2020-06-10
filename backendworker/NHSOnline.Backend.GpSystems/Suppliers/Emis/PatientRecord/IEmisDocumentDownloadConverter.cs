namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public interface IEmisDocumentDownloadConverter
    {
        byte[] ConvertToText(string content);
        byte[] ConvertToImage(string content);
        byte[] ConvertToWordDocument(string content);
        byte[] ConvertToPdf(string content);
    }
}