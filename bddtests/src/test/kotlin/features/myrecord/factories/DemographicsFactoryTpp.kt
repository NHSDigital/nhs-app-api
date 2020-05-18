package features.myrecord.factories

import mocking.data.myrecord.DemographicsData
import mocking.stubs.StubbedEnvironment
import mocking.tpp.models.Error
import models.Patient
import org.apache.http.HttpStatus
import java.time.Duration

class DemographicsFactoryTpp: DemographicsFactory() {
    override fun disabled(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.patientSelectedPost(patient.tppUserSession!!)
                    .respondWithError(Error("6", "Error Occurred", "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }

    override fun enabled(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.patientSelectedPost(patient.tppUserSession!!)
                    .respondWithSuccess(DemographicsData.getTppDemographicsData(patient))
        }
    }

    override fun enabledViaProxy(callingPatient: Patient, actingOnBehalfOf: Patient) {
        mockingClient.forTpp.mock {
            myRecord.patientSelectedPost(
                    actingOnBehalfOf.tppUserSession!!)
                    .respondWithSuccess(DemographicsData.getTppDemographicsData(actingOnBehalfOf))
        }
    }

    override fun enabledButTimesOut(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.patientSelectedPost(patient.tppUserSession!!)
                    .respondWithSuccess(DemographicsData.getTppDemographicsData(patient)).delayedBy(Duration.ofSeconds
                    (StubbedEnvironment.TIMEOUT_DELAY))
        }
    }

    override fun throwInternalError(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.patientSelectedPost(patient.tppUserSession!!)
                    .respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR,0){}
        }
    }

    override fun throwForbiddenError(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.patientSelectedPost(patient.tppUserSession!!)
                    .respondWith(HttpStatus.SC_FORBIDDEN,0){}
        }
    }

    override fun throwBadGateway(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.patientSelectedPost(patient.tppUserSession!!)
                    .respondWith(HttpStatus.SC_BAD_GATEWAY,0){}
        }
    }
}
