using System.Collections;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.Emis
{
    public class EmisCoursesNoMedicationsBehaviour: IEmisCoursesBehaviour
    {
        public IActionResult Behave()
        {
            return new JsonResult(new
            {
                Courses = new ArrayList{}
            });
        }
    }
}