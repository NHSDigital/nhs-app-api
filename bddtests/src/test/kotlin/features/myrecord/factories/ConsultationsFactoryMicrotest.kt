package features.myrecord.factories

import models.Patient
import worker.models.myrecord.ConsultationItem

class ConsultationsFactoryMicrotest: ConsultationsFactory() {
    override fun disabled(patient: Patient) {
        throw UnsupportedOperationException("Not yet implemented")
    }

    override fun enabledWithRecords(patient: Patient) {
        throw UnsupportedOperationException("Not yet implemented")
    }

    override fun getExpectedConsultations(): List<ConsultationItem> {
        throw UnsupportedOperationException("Not yet implemented")
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        throw UnsupportedOperationException("Not yet implemented")
    }

    override fun errorRetrieving(patient: Patient) {
        throw UnsupportedOperationException("Not yet implemented")
    }

    override fun recordWithBadConsultationsData(patient: Patient) {
        throw UnsupportedOperationException("Not yet implemented")
    }
}