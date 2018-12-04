package mocking.throttling

import mocking.MappingBuilder
import config.Config

open class BrotherMailerRedirectMappingBuilder(method: String)
    : MappingBuilder(method, Config.instance.brotherMailerRedirectPath) {

    fun redirectBrotherMailer() = BrotherMailerRedirectResultBuilder()
}