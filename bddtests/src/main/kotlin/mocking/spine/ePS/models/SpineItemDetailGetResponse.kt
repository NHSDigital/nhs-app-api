package mocking.spine.ePS.models

class SpineItemDetailGetResponse(

    statusCode: String,
    version: String,
    reason: String,
    var prescriptions: Map<String, SpineItemDetailPrescription>

): SpineItemGetResponse(statusCode, version, reason)
