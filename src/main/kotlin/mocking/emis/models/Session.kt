package mocking.emis.models

class Session {
    var SessionName: String? = null
    var SessionId: Int? = null
    var LocationId: Int? = null
    var DefaultDuration: Int? = null
    var SessionType: SessionType? = null
    var NumberOfSlots: Int? = null
    var ClinicianIds: ArrayList<Int> = ArrayList()
    var StartDate: String? = null
    var EndDate: String? = null
}
