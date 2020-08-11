package mocking.externalSites

import mocking.MappingBuilder

open class ExternalSitesMappingBuilder(method: String ="GET", relativePath:String="")
    : MappingBuilder(method, "/external$relativePath") {

    fun adviceAboutCoronavirusRequest() = AdviceAboutCoronavirusRequestBuilder()

    fun healthAToZRequest() = HealthAToZRequestBuilder()

    fun oneOneOneOnlineRequest() = OneOneOneOnlineRequestBuilder()
}
