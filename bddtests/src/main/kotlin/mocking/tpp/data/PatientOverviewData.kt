package mocking.tpp.data

import mocking.tpp.models.ViewPatientOverviewItem
import mocking.tpp.models.ViewPatientOverviewReply
import java.time.OffsetDateTime

const val MINUS_DAYS = 180L

class PatientOverviewData{

    private val currentDate = OffsetDateTime.now()
    private val pastDate = currentDate.minusDays(MINUS_DAYS).toString()
    private val drugSensitivities = arrayListOf(ViewPatientOverviewItem(date = pastDate, value = "Calpol" ))
    private val drugs =  arrayListOf(ViewPatientOverviewItem(date = pastDate))
    private val pastReapeats = arrayListOf(ViewPatientOverviewItem(date = pastDate, value = "Ventolin"),
            ViewPatientOverviewItem(date = pastDate, value = "Calpol"))
    private val currentRepeats = arrayListOf(ViewPatientOverviewItem(date = pastDate, value = "Paracetamol"))
    private val allergies = arrayListOf(ViewPatientOverviewItem(id = "1", date = pastDate, value = "Nuts"))

    var patientOverviewData = ViewPatientOverviewReply(drugSensitivities = drugSensitivities, drugs = drugs,
            pastRepeats = pastReapeats, currentRepeats = currentRepeats, allergies = allergies)


}
