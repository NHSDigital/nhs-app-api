package mocking.stubs.appointments.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import mocking.MockingClient
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.models.Mapping
import java.util.*

abstract class MockingClientAppointmentMappingFactory {

    var mockingClient = MockingClient.instance

    abstract fun requestMapping(mapper: IAppointmentMappingBuilder.() -> Mapping)

    companion object : SupplierSpecificFactory<MockingClientAppointmentMappingFactory>() {

        override val map: HashMap<Supplier, (() -> MockingClientAppointmentMappingFactory)> by lazy {
            hashMapOf(
                    Supplier.EMIS to { MockingClientAppointmentMappingFactoryEmis() },
                    Supplier.TPP to { MockingClientAppointmentMappingFactoryTpp() },
                    Supplier.VISION to { MockingClientAppointmentMappingFactoryVision() },
                    Supplier.MICROTEST to { MockingClientAppointmentMappingFactoryMicrotest() }
            )
        }
    }
}