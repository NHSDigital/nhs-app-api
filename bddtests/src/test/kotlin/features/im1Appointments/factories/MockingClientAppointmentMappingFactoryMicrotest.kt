package features.im1Appointments.factories

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.models.Mapping

class MockingClientAppointmentMappingFactoryMicrotest: MockingClientAppointmentMappingFactory(){
    override fun requestMapping(mapper: IAppointmentMappingBuilder.() -> Mapping) {
        mockingClient.forMicrotest { mapper(appointments) }
    }
}
