namespace NHSOnline.Backend.Support
{
    public enum Supplier
    {
        [SourceApi(SourceApi.None)]
        Unknown = 0,
        [SourceApi(SourceApi.Emis)]
        Emis = 1,
        [SourceApi(SourceApi.Tpp)]
        Tpp = 2,
        [SourceApi(SourceApi.Vision)]
        Vision = 3,
        [SourceApi(SourceApi.Microtest)]
        Microtest = 4,
        Qualtrics = 5,
        [SourceApi(SourceApi.Fake)]
        Fake = 6
    }
}