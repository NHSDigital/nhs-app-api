package mocking.externalSites

import mocking.MappingBuilder
import mocking.externalSites.oneOneOne.OneOneOneOnlineRequestBuilder
import mocking.externalSites.nhsUK.NhsUkRequestBuilder
import mocking.externalSites.organDonation.OrganDonationRequestBuilder

open class ExternalSitesMappingBuilder(method: String ="GET", relativePath:String="")
    : MappingBuilder(method, "/external$relativePath") {

    fun adviceAboutCoronavirusRequest() = AdviceAboutCoronavirusRequestBuilder()

    fun healthAToZRequest() = HealthAToZRequestBuilder()

    fun bloodDonorRequest() = BloodDonorRequestBuilder()

    fun informaticaRequest() = InformaticaRequestBuilder()

    fun oneOneOneOnlineRequest(path: String) = OneOneOneOnlineRequestBuilder("GET", path)

    fun organDonationRequest(path: String) = OrganDonationRequestBuilder("GET", path)

    fun nhsUkRequest(path: String) = NhsUkRequestBuilder("GET", path)

    fun getCovidPassRequest() = GetCovidPassRequestBuilder()

    fun getCovidPassOrProofRequest() = GetCovidPassOrProofRequestBuilder()

    fun getNorthernIrelandRequest() = GetNorthernIrelandRequestBuilder()
}
