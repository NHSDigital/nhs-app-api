package mocking.stubs.appointments.factories

import mocking.SupplierSpecificFactory
import mocking.MockingClient
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.models.Mapping
import java.util.*

abstract class MockingClientAppointmentMappingFactory {

    var mockingClient = MockingClient.instance

    abstract fun requestMapping(mapper: IAppointmentMappingBuilder.() -> Mapping)

    companion object : SupplierSpecificFactory<MockingClientAppointmentMappingFactory>() {

        override val map: HashMap<String, (() -> MockingClientAppointmentMappingFactory)> by lazy {
            hashMapOf(
                    "EMIS" to { MockingClientAppointmentMappingFactoryEmis() },
                    "TPP" to { MockingClientAppointmentMappingFactoryTpp() },
                    "VISION" to { MockingClientAppointmentMappingFactoryVision() },
                    "MICROTEST" to { MockingClientAppointmentMappingFactoryMicrotest() }
            )
        }
    }
}