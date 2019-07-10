package features.myrecord.factories

import mocking.data.myrecord.ImmunisationsData
import mocking.vision.VisionConstants
import models.Patient
import worker.models.myrecord.ImmunisationItem

class ImmunisationsFactoryVision: ImmunisationsFactory() {

    private var mocker: MyRecordVisionMocker = MyRecordVisionMocker(mockingClient)

    override fun enabledWithBlankRecord(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.immunisationsView)
        { request -> request.respondWithSuccess(ImmunisationsData.getVisionImmunisationsDataWithNoImmunisations()) }
    }

    override fun enabledWithRecords(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.immunisationsView)
        { request -> request.respondWithSuccess(ImmunisationsData.getVisionImmunisationsData(2)) }
    }

    override fun errorRetrieving(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.immunisationsView)
        { request -> request.respondWithUnknownError() }
    }

    override fun noAccess(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.immunisationsView)
        { request -> request.respondWithAccessDeniedError() }
    }

    override fun getExpectedImmunisations(): List<ImmunisationItem> {
        throw UnsupportedOperationException()
    }
}