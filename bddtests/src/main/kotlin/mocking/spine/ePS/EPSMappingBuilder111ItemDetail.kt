package mocking.spine.ePS

import mocking.spine.ePS.prescriptions.EPS111ItemDetailBuilder

class EPSMappingBuilder111ItemDetail {

    fun prescriptionDetailTrackingRequest(prescriptionId:String) = EPS111ItemDetailBuilder (
            prescriptionId, null)
}
