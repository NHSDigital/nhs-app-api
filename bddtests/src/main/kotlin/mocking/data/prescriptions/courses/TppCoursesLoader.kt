package mocking.data.prescriptions.courses

import mocking.data.prescriptions.TppPrescriptionLoader
import mocking.tpp.models.ListRepeatMedicationReply
import models.prescriptions.MedicationCourse


object TppCoursesLoader: ICoursesLoader<ListRepeatMedicationReply> {

    override lateinit var data: ListRepeatMedicationReply

    override fun loadData(maxCourses: Int,
                          numOfRepeats: Int,
                          numCanBeRequested: Int,
                          includeDosage: Boolean,
                          includeQuantity: Boolean) {

        TppPrescriptionLoader.loadData(maxCourses, numCanBeRequested, numOfRepeats, includeDosage, includeQuantity  )
        data = TppPrescriptionLoader.data
    }

    override fun getAvailableCoursesFilteredSortedOrdered(): List<MedicationCourse> {
        var coursesDataFiltered = data.Medication.filter { mc ->
            mc.requestable.toLowerCase() == "y"
        }
        coursesDataFiltered = coursesDataFiltered.sortedBy{ m -> m.drug }
        coursesDataFiltered = coursesDataFiltered.take(100).toMutableList()

        return coursesDataFiltered.map { m -> MedicationCourse(m.drugId, m.drug, m.details) }
    }
}
