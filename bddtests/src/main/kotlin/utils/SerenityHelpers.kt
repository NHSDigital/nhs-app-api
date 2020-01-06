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
            setSerenityVariableIfNotAlreadySet(
                    Patient::class,
                    patientToSet,
                    "Test setup incorrect, expected patients to be the same"
            )
        }

        fun resetPatient(patientToSet: Patient) {
            val currentStoredValue = getValueOrNull<Any>(Patient::class)
            if (currentStoredValue !== null) {
                Serenity.setSessionVariable(Patient::class).to(patientToSet)
            }
        }

        fun getPatient(): Patient {
            Assert.assertTrue("Test setup incorrect, patient needs to be set",
                    Serenity.hasASessionVariableCalled(Patient::class))
            return Serenity.sessionVariableCalled<Patient>(Patient::class)
        }

        fun getPatientOrNull(): Patient? {
            return getValueOrNull<Patient>(Patient::class)
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

        fun setHttpException(httpException: NhsoHttpException) {
            Serenity.setSessionVariable("HttpException").to(httpException)
        }

        fun clearHttpException() {
            Serenity.setSessionVariable("HttpException").to(null)
        }

        fun getHttpException(): NhsoHttpException? {
            return  getValueOrNull("HttpException")
        }

        fun setHttpResponse(response: HttpResponse) {
            Serenity.setSessionVariable("HttpResponse").to(response)
        }

        fun getHttpResponse(): HttpResponse? {
            return  getValueOrNull("HttpResponse")
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
