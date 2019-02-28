package mocking.spine.ePS

import mocking.spine.SpineConfiguration
import mocking.spine.ePS.prescriptions.EPS111ItemSummaryBuilder

class EPSMappingBuilder111ItemSummary(private var configuration: SpineConfiguration?) {

    fun prescriptionTrackingRequest() = EPS111ItemSummaryBuilder (
            null,
            null,
            null,
            null,
            null)
}