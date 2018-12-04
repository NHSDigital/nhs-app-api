package mocking.throttling

import config.Config
import mocking.MappingBuilder

open class BrotherMailerMappingBuilder(method:String)
    : MappingBuilder(method, Config.instance.brotherMailerPath) {

    fun postToBrotherMailer() = BrotherMailerResultBuilder()
}