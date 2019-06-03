package mocking.microtest.demographics

data class GetDemographicsResponseModel(
        var demographics : DemographicsData = DemographicsData())

data class DemographicsData(
        var title :String = "",
        var surname :String = "",
        var forenames1 :String = "",
        var forenames2 :String = "",
        var dob :String = "",
        var sex :String= "",
        var nhs :String = "",
        var houseName :String = "",
        var roadName :String = "",
        var locality:String = "",
        var post_town:String = "",
        var county:String = "",
        var postcode:String = "",
        var telephone1: String = "",
        var telephone2: String = ""
)
