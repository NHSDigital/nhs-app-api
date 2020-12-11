package utils

import constants.Supplier
import mocking.MockingClient
import models.Patient
import net.serenitybdd.core.Serenity
import org.apache.http.HttpResponse
import org.junit.Assert
import worker.NhsoHttpException

class SerenityHelpers {

    companion object {

        fun setPatient(patientToSet: Patient) {
            GlobalSerenityHelpers.PATIENT.setIfNotAlreadySet(patientToSet)
        }

        fun setPatientIfNull(patientToSet: Patient) {
            if (getPatientOrNull() == null) {
                setPatient(patientToSet)
            }
        }

        fun resetPatient(patientToSet: Patient) {
            val currentStoredValue = getValueOrNull<Any>(GlobalSerenityHelpers.PATIENT)
            if (currentStoredValue !== null) {
                Serenity.setSessionVariable(GlobalSerenityHelpers.PATIENT).to(patientToSet)
            }
        }

        fun getPatient(): Patient {
            return GlobalSerenityHelpers.PATIENT.getOrFail()
        }

        fun getPatientOrNull(): Patient? {
            return GlobalSerenityHelpers.PATIENT.getOrNull()
        }

        fun getGpSupplier():Supplier{
            return GlobalSerenityHelpers.GP_SYSTEM.getOrFail()
        }

        fun setGpSupplier(gpSupplier: Supplier){
            GlobalSerenityHelpers.GP_SYSTEM.setIfNotAlreadySet(gpSupplier)
        }

        fun getMockingClient(): MockingClient {
            val mockingClient = getValueOrNull<MockingClient>(MockingClient::class)
            if (mockingClient != null) {
                return mockingClient
            }
            val newMockingClient = MockingClient.instance
            Serenity.setSessionVariable(MockingClient::class).to(newMockingClient)
            return newMockingClient
        }

        fun clearHttpException() {
            GlobalSerenityHelpers.HTTP_EXCEPTION.set(null)
        }

        fun getHttpException(): NhsoHttpException? {
            return GlobalSerenityHelpers.HTTP_EXCEPTION.getOrNull()
        }

        fun setHttpResponse(response: HttpResponse?) {
            GlobalSerenityHelpers.HTTP_RESPONSE.set(response)
        }

        fun getHttpResponse(): HttpResponse? {
            return GlobalSerenityHelpers.HTTP_RESPONSE.getOrFail()
        }

        fun setSerenityVariableIfNotAlreadySet(
                key: Any,
                valueToSet: Any,
                assertionFailureMessage: String = "Test setup incorrect, trying to set a value already set"
        ) {
            val currentStoredValue = getValueOrNull<Any>(key)
            if (currentStoredValue == null) {
                Serenity.setSessionVariable(key).to(valueToSet)
            } else {
                Assert.assertEquals(assertionFailureMessage,
                        currentStoredValue,
                        valueToSet)
            }
        }

        fun <T>getValueOrNull(key: Any): T? {
            if (Serenity.hasASessionVariableCalled(key) ){
                return Serenity.sessionVariableCalled<T>(key)
            }
            return null
        }
    }
}
