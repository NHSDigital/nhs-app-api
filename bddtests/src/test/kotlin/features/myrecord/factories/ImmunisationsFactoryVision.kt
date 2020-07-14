package features.myrecord.factories

import mocking.data.myrecord.ImmunisationsData
import mocking.vision.VisionConstants
import models.Patient
import worker.models.myrecord.ImmunisationItem

class ImmunisationsFactoryVision: ImmunisationsFactory() {

    private val responseFormat = VisionConstants.xmlResponseFormat
    private var mocker: MyRecordVisionMocker = MyRecordVisionMocker(mockingClient)

    override fun enabledWithBlankRecord(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.immunisationsView, responseFormat)
        { request -> request.respondWithSuccess(ImmunisationsData.getVisionImmunisationsDataWithNoImmunisations()) }
    }

    override fun enabledWithRecords(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.immunisationsView, responseFormat)
        { request -> request.respondWithSuccess(ImmunisationsData.getVisionImmunisationsData(2)) }
    }

    override fun errorRetrieving(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.immunisationsView, responseFormat)
        { request -> request.respondWithUnknownError() }
    }

    override fun noAccess(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.immunisationsView, responseFormat)
        { request -> request.respondWithAccessDeniedError() }
    }

    override fun respondWithACorruptedResponse(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.immunisationsView, responseFormat)
        { request -> request.respondWithCorruptedContent("Bad data") }
    }

    override fun getExpectedImmunisations(): List<ImmunisationItem> {
        throw UnsupportedOperationException()
    }
}
