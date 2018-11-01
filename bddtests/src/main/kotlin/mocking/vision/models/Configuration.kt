package mocking.vision.models

import mocking.vision.models.appointments.Appointments
import mocking.vision.models.appointments.References
import javax.xml.bind.annotation.XmlRootElement
import javax.xml.bind.annotation.XmlElement

@XmlRootElement(name = "configuration")
data class Configuration(

        @XmlElement(namespace = "urn:vision")
        var account: Account = Account(),

        @XmlElement(namespace = "urn:vision", name = "prescriptions")
        var prescriptions: Prescriptions = Prescriptions(),

        @XmlElement(namespace = "urn:vision", name = "appointments")
        var appointments: Appointments = Appointments(),

        @XmlElement(namespace = "urn:vision")
        var references: References = References()

)
