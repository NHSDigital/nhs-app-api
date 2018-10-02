package mocking.data.prescriptions

import mocking.data.prescriptions.courses.VisionCoursesLoader
import mocking.emis.models.VisionPrescriptionStatus
import mocking.vision.models.PrescriptionHistory
import mocking.vision.models.Repeat
import mocking.vision.models.Request
import mocking.vision.models.StatusCode
import org.joda.time.DateTime
import java.util.*

object VisionPrescriptionLoader : IPrescriptionLoader<PrescriptionHistory> {
    override lateinit var data: PrescriptionHistory

    private const val MAX_RANDOM_NUMBER = 2

    @Suppress("ComplexMethod")
    override fun loadData(noPrescriptions: Int,
                          noCourses: Int,
                          noRepeats: Int,
                          showDosage: Boolean,
                          showQuantity: Boolean) {

        var prescriptions = mutableListOf<Request>()

        if (noPrescriptions != 0) {
            // Create courses first as these will be used in the prescriptions
            VisionCoursesLoader.loadData(
                    maxCourses = noCourses,
                    numOfRepeats = noRepeats,
                    numCanBeRequested = noRepeats,
                    includeDosage = showDosage,
                    includeQuantity = showQuantity)

            var repeatCourse = VisionCoursesLoader.data.request

            var maxNumberOfPrescriptions = noPrescriptions.minus(1)
            var isSecondIteration = false
            var prescriptionNumber = 0
            var courseNumber = repeatCourse!!.count().minus(1)

            while (prescriptionNumber <= maxNumberOfPrescriptions) {
                var repeats = ArrayList<Repeat>()

                var time = DateTime.now().minusDays(prescriptionNumber).toLocalDateTime()

                var course = repeatCourse.get(courseNumber)

                var repeat = Repeat(course.drug, course.dosage, course.quantity, null)

                if (!isSecondIteration) {

                    var myEnum = VisionPrescriptionLoader.getPrescriptionStatus()

                    repeats.add(repeat)
                    prescriptions.add(
                            Request(time.toString(), StatusCode(myEnum.value, myEnum.getDisplayName()), repeats))
                } else {

                    repeats.add(repeat)

                    prescriptions.get(
                            prescriptionNumber).repeat?.addAll(repeats)
                }

                courseNumber--

                if (prescriptionNumber == maxNumberOfPrescriptions && courseNumber >= 0) {
                    isSecondIteration = true
                    maxNumberOfPrescriptions = noPrescriptions.minus(1)
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
