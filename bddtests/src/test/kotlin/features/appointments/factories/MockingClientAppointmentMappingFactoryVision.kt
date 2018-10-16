package features.appointments.factories

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.models.Mapping

class MockingClientAppointmentMappingFactoryVision: MockingClientAppointmentMappingFactory(){
    override fun requestMapping(mapper: IAppointmentMappingBuilder.() -> Mapping) {
        mockingClient.forVision { mapper(appointments) }
    }
}