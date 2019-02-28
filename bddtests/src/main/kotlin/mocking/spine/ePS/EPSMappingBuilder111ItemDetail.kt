package mocking.spine.ePS

import mocking.spine.SpineConfiguration
import mocking.spine.ePS.prescriptions.EPS111ItemDetailBuilder

class EPSMappingBuilder111ItemDetail(private var configuration: SpineConfiguration?) {

    fun prescriptionDetailTrackingRequest(prescriptionId:String) = EPS111ItemDetailBuilder (
            prescriptionId, null)
}