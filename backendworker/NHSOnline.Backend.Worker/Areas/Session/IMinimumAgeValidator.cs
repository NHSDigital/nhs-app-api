using System;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    public interface IMinimumAgeValidator
    {
        bool IsValid(DateTime dateOfBirth, int minimumAge);
    }
}