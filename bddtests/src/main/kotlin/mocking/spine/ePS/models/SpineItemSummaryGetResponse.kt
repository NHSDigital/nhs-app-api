package mocking.spine.ePS.models

class SpineItemSummaryGetResponse(
        statusCode: String,
        version: String,
        reason: String,
        var prescriptions: Map<String, SpineItemSummaryPrescription>

): SpineItemGetResponse(statusCode, version, reason)
