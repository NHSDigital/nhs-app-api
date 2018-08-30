package mocking.ndop

import config.Config
import mocking.MappingBuilder

open class NdopMappingBuilder(method: String)
    : MappingBuilder(method, Config.instance.dataPreferencesPath) {

    fun postTokenToNdop() = NdopLinkRequestBuilder()
}