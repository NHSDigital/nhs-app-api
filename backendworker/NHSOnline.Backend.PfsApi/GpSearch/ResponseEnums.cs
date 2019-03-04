namespace NHSOnline.Backend.Worker.GpSearch
{
    public static class ResponseEnums
    {
        public enum GeoCodeType
        {
            Point = 0,
            name = 1,
        }

        public enum WeekDay
        {
            Monday = 0,
            Tuesday = 1,
            Wednesday = 2,
            Thursday = 3,
            Friday = 4,
            Saturday = 5,
            Sunday = 6,
        }

        public enum OpeningTimeType
        {
            General = 0,
            Reception = 1,
            Surgery = 2,
        }

        public enum OrganisationContactMethodType
        {
            Telephone = 0,
            Website = 1,
            Email = 2,
        }
    }
}
