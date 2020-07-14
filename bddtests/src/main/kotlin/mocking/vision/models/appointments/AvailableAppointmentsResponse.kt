package mocking.vision.models.appointments

import mocking.vision.appointments.helpers.GeneralAppointmentsHelper
import mocking.vision.models.appointments.AvailableAppointmentsResponse.Companion.name
import net.serenitybdd.core.Serenity
import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(namespace = "urn:vision", name = name)
@XmlAccessorType(XmlAccessType.FIELD)
data class AvailableAppointmentsResponse(
        @XmlElement(namespace = "urn:vision", name = "slot")
        var slots: Slots? = null,
        @XmlElement(namespace = "urn:vision")
        var references: References? = null
) {
    init {
        if (references == null) {
            Serenity.setSessionVariable(GeneralAppointmentsHelper.Companion.VisionMetadata.LOCATIONS)
                    .to(listOf(Location(1, "Default " + "Location"))
            )
            Serenity.setSessionVariable(GeneralAppointmentsHelper.Companion.VisionMetadata.OWNERS)
                    .to(listOf(Owner(1, "Default " + "Owner"))
                    )
        }
    }

    companion object {
        const val name = "availableAppointmentsResponse"
    }
}
