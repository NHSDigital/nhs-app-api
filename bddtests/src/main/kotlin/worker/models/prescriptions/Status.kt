package worker.models.prescriptions

enum class Status(val status: Int){
    Approved(1),
    Ordered(2),
    Rejected(3),
    Cancelled(4)
}
