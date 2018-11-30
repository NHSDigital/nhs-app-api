package features.myrecord.factories

import mocking.data.myrecord.ImmunisationsData
import models.Patient

class ImmunisationsFactoryEmis: ImmunisationsFactory(){
    override fun enabledWithBlankRecord(patient: Patient) {
        mockingClient.forEmis {
            myRecord.immunisationsRequest(patient)
                    .respondWithSuccess(ImmunisationsData.getDefaultImmunisationsModel())
        }
    }

    override fun enabledWithRecords(patient: Patient) {
        mockingClient.forEmis {
            myRecord.immunisationsRequest(patient).respondWithSuccess(ImmunisationsData.getImmunisationsData())
        }
    }

    override fun errorRetrieving(patient: Patient) {
        mockingClient.forEmis {
            myRecord.immunisationsRequest(patient).respondWithNonDataAccessException()
        }
    }

    override fun noAccess(patient: Patient) {
        mockingClient.forEmis {
            myRecord.immunisationsRequest(patient).respondWithExceptionWhenNotEnabled()
        }
    }
}