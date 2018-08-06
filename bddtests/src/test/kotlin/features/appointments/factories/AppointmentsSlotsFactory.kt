package features.appointments.factories

import features.appointments.data.AppointmentsSlotsExample
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import org.junit.Assert
import java.time.Duration
import java.util.*

abstract class AppointmentsSlotsFactory(gpSupplier:String): AppointmentsFactory(gpSupplier) {

    fun generateDefaultAvailableAppointmentSlotExample(startDate: String? = null, endDate: String? = null) {
        generateDefaultUserData()
        val appointmentSlotsResponseModel = AppointmentsSlotsExample.getExample()
        generateAppointmentSlotResponse(startDate, endDate) {
            respondWithSuccess(appointmentSlotsResponseModel)
        }
    }

    abstract fun generateAppointmentSlotResponse(startDate: String?,
                                                           endDate: String?,
                                                           mapping: (IAppointmentSlotsBuilder.() -> Mapping))

    companion object {

        private val map: HashMap<String, (() -> (AppointmentsSlotsFactory))> by lazy {
            hashMapOf(
                    "EMIS" to { AppointmentsSlotsFactoryEmis() },
                    "TPP" to { AppointmentsSlotsFactoryTpp() })
        }

        fun getForSupplier(gpSystem: String): AppointmentsSlotsFactory {
            if (!map.containsKey(gpSystem)) {
                Assert.fail("GP system '$gpSystem' is not set up.")
            }
            return map.getValue(gpSystem).invoke()
        }
    }
}