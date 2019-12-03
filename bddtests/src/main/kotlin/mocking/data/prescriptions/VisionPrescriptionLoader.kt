package mocking.data.prescriptions

import mocking.data.prescriptions.courses.VisionCoursesLoader
import mocking.vision.models.VisionPrescriptionStatus
import mocking.vision.models.PrescriptionHistory
import mocking.vision.models.Repeat
import mocking.vision.models.Request
import mocking.vision.models.StatusCode
import models.prescriptions.PrescriptionLoaderConfiguration
import org.joda.time.DateTime
import java.util.*

object VisionPrescriptionLoader : IPrescriptionLoader<PrescriptionHistory> {
    override lateinit var data: PrescriptionHistory

    private const val MAX_RANDOM_NUMBER = 2

    override fun loadData(prescriptionLoaderConfig: PrescriptionLoaderConfiguration,
                          prescriptionCompletedByProxy: Boolean) {

        val prescriptions = mutableListOf<Request>()

        if (prescriptionLoaderConfig.noPrescriptions != 0) {
            // Create courses first as these will be used in the prescriptions
            VisionCoursesLoader.loadData(
                    maxCourses = prescriptionLoaderConfig.noCourses,
                    numOfRepeats = prescriptionLoaderConfig.noRepeats,
                    numCanBeRequested = prescriptionLoaderConfig.noRepeats,
                    includeDosage = prescriptionLoaderConfig.showDosage,
                    includeQuantity = prescriptionLoaderConfig.showQuantity)

            val repeatCourse = VisionCoursesLoader.data.repeat

            var maxNumberOfPrescriptions = prescriptionLoaderConfig.noPrescriptions.minus(1)
            var isSecondIteration = false
            var prescriptionNumber = 0
            var courseNumber = repeatCourse.count().minus(1)

            while (prescriptionNumber <= maxNumberOfPrescriptions) {
                val repeats = ArrayList<Repeat>()

                val time = DateTime.now().minusDays(prescriptionNumber).toLocalDateTime()

                val course = repeatCourse.get(courseNumber)

                val repeat = Repeat(course.drug, course.dosage, course.quantity, null)

                if (!isSecondIteration) {

                    val myEnum = VisionPrescriptionLoader.getPrescriptionStatus()

                    repeats.add(repeat)
                    prescriptions.add(
                            Request(time.toString(), StatusCode(myEnum.value, myEnum.getDisplayName()), repeats))
                } else {

                    repeats.add(repeat)

                    prescriptions.get(
                            prescriptionNumber).repeat.addAll(repeats)
                }

                courseNumber--

                if (prescriptionNumber == maxNumberOfPrescriptions && courseNumber >= 0) {
                    isSecondIteration = true
                    maxNumberOfPrescriptions = prescriptionLoaderConfig.noPrescriptions.minus(1)
                    prescriptionNumber = 0
                } else if (prescriptionNumber < maxNumberOfPrescriptions && courseNumber == -1) {
                    prescriptionNumber++
                    courseNumber = repeatCourse.count().minus(1)
                } else {
                    prescriptionNumber++
                }
            }
        }

        data = PrescriptionHistory(prescriptions)
    }

    fun getPrescriptionStatus(): VisionPrescriptionStatus {
        val statusEnums = enumValues<VisionPrescriptionStatus>()
        return statusEnums[(0..VisionPrescriptionLoader.MAX_RANDOM_NUMBER).random()]
    }

    fun IntRange.random() =
            Random().nextInt((endInclusive + 1) - start) +  start
}
