package mocking.data.prescriptions.mappers

import mocking.tpp.models.ListRepeatMedicationReply
import mocking.vision.models.Repeat
import mocking.vision.models.RepeatCourse
import models.prescriptions.HistoricPrescription

object VisionCourseMapper {

        fun map(data: List<RepeatCourse>) : ArrayList<Repeat> {

            val repeats = ArrayList<Repeat>()

            for(repeatCourse in data){
                val repeat = Repeat(
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