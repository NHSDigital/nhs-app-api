package features.appointments.factories

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.models.Mapping

class MockingClientAppointmentMappingFactoryEmis: MockingClientAppointmentMappingFactory(){
    override fun requestMapping(mapper: IAppointmentMappingBuilder.() -> Mapping) {
        mockingClient.forEmis { mapper() }
    }
}