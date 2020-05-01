package models.patients

import constants.Supplier
import mocking.SupplierSpecificFactory
import models.Patient
import java.util.HashMap

abstract class PatientHandler {

    abstract fun getDefault(): Patient
    abstract fun getPatientWithLinkedProfiles(): Patient
    abstract fun getPatientWithNoLinkedProfiles(): Patient
    abstract fun setOdsCode(patient: Patient, provider: String)

    companion object : SupplierSpecificFactory<PatientHandler>() {
        override val map: HashMap<Supplier, (() -> (PatientHandler))> by lazy {
            hashMapOf(
                    Supplier.EMIS to { EmisPatients },
                    Supplier.TPP to { TppPatients },
                    Supplier.VISION to { VisionPatients },
                    Supplier.MICROTEST to { MicrotestPatients }
            )
        }
    }
}