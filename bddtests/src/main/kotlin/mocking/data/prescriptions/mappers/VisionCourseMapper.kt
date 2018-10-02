package mocking.data.prescriptions.mappers

import mocking.vision.models.Repeat
import mocking.vision.models.RepeatCourse

object VisionCourseMapper {

    fun map(data: List<RepeatCourse>) : ArrayList<Repeat> {
        var repeats = ArrayList<Repeat>()
        for(repeatCourse in data){
            var repeat = Repeat(
                    repeatCourse.drug,
                    repeatCourse.dosage,
                    repeatCourse.quantity,
                    repeatCourse.lastIssued
            )
            repeats.add(repeat)
        }
        return repeats
    }
}