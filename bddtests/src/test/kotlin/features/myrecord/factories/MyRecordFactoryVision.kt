package features.myrecord.factories

import mocking.data.myrecord.AllergiesData
import mocking.data.myrecord.ExaminationsData
import mocking.data.myrecord.ImmunisationsData
import mocking.data.myrecord.ProceduresData
import mocking.data.myrecord.TestResultsData
import mocking.vision.VisionConstants
import models.Patient

class MyRecordFactoryVision: MyRecordFactory() {

    private var mocker: MyRecordVisionMocker = MyRecordVisionMocker(mockingClient)

    override fun disabled(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.allergiesView, VisionConstants.htmlResponseFormat)
        { request -> request.respondWithAccessDeniedError()}
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.allergiesView, VisionConstants.htmlResponseFormat)
        { request -> request.respondWithSuccess(AllergiesData.getVisionAllergiesData(0))}

        mocker.generatePatientDataResponse(patient, VisionConstants.immunisationsView)
        { request -> request.respondWithSuccess(ImmunisationsData.getVisionImmunisationsDataWithNoImmunisations())}

        mocker.generatePatientDataResponse(patient, VisionConstants.medicationsView)
         { request -> request.respondWithSuccess(ImmunisationsData.getVisionImmunisationsDataWithNoImmunisations())}

        mocker.generatePatientDataResponse(patient, VisionConstants.testResultsView)
        { request -> request.respondWithSuccess(TestResultsData.getVisionTestResultsDataWithNoTestResults()) }

        mocker.generatePatientDataResponse(patient, VisionConstants.examinationsView)
        { request -> request.respondWithSuccess(ExaminationsData.getVisionExaminationsDataWithNoExaminations()) }
        
        mocker.generatePatientDataResponse(patient, VisionConstants.proceduresView)
        { request -> request.respondWithSuccess(ProceduresData.getVisionProceduresDataWithNoProcedures()) }
    }
}