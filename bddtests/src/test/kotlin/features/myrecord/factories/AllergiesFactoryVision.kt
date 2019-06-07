package features.myrecord.factories

import mocking.data.myrecord.AllergiesData
import mocking.vision.VisionConstants
import models.Patient
import worker.models.myrecord.AllergyItem

class AllergiesFactoryVision: AllergiesFactory() {

    private var mocker: MyRecordVisionMocker = MyRecordVisionMocker(mockingClient)

    override fun disabled(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.allergiesView, VisionConstants.htmlResponseFormat)
        { request -> request.respondWithAccessDeniedError() }
    }

    override fun enabledWithRecords(patient: Patient, count: Int) {
        mocker.generatePatientDataResponse(patient, VisionConstants.allergiesView, VisionConstants.htmlResponseFormat)
        { request -> request.respondWithSuccess(AllergiesData.getVisionAllergiesData(count)) }
    }

    override fun getExpectedAllergies(): List<AllergyItem> {
        throw UnsupportedOperationException("Not yet implemented")
    }
}