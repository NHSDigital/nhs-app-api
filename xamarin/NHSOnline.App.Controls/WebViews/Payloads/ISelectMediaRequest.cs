namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public interface ISelectMediaRequest
    {
        SelectMediaType Type { get; }
        SelectMediaMode Mode { get; }

        void MediaSelected(string path);
        void NoMediaSelected();
    }
}