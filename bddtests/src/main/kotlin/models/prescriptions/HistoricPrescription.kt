package models.prescriptions

data class HistoricPrescription(val name:String, val dosage:String){
    var orderDate: String? = null
    var status: String? = null
}
