package features.appointments.factories

import mocking.MockingClient
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.models.Mapping
import org.junit.Assert
import java.util.HashMap

abstract class MockingClientAppointmentMappingFactory{

    var mockingClient = MockingClient.instance

abstract fun requestMapping(mapper: IAppointmentMappingBuilder.() -> Mapping) :Unit

    companion object {
        private val map: HashMap<String, MockingClientAppointmentMappingFactory> by lazy{
                hashMapOf(
                        "EMIS" to MockingClientAppointmentMappingFactoryEmis(),
                        "TPP" to MockingClientAppointmentMappingFactoryTpp())}

        fun getForSupplier(gpSystem: String): MockingClientAppointmentMappingFactory {
            if (!map.containsKey(gpSystem)) {
                Assert.fail("GP system '$gpSystem' is not set up.")
            }
            return map.getValue(gpSystem)
        }
    }
}