package features.im1Appointments.factories

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.models.Mapping

class MockingClientAppointmentMappingFactoryTpp: MockingClientAppointmentMappingFactory(){
    override fun requestMapping(mapper: IAppointmentMappingBuilder.() -> Mapping) {
        mockingClient.forTpp { mapper(appointments) }
    }
}