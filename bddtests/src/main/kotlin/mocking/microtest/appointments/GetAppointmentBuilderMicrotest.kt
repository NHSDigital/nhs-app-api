package mocking.microtest.appointments

import mocking.GsonFactory
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.microtest.MicrotestMappingBuilder
import mocking.models.Mapping
import mockingFacade.appointments.MyAppointmentsFacade
import org.apache.http.HttpStatus.SC_NOT_FOUND

class GetAppointmentBuilderMicrotest
    : MicrotestMappingBuilder(method = "GET", relativePath = "/appointments"), IMyAppointmentsBuilder {
    override fun respondWithSuccess(facade: MyAppointmentsFacade): Mapping {
        return respondWith(SC_NOT_FOUND) { andJsonBody("", GsonFactory.asPascal) }
    }

    override fun respondWithSuccess(body: String): Mapping {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun respondWithCorrupted(): Mapping {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun respondWithGPServiceUnavailableException(): Mapping {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun respondWithUnknownException(): Mapping {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }
}
