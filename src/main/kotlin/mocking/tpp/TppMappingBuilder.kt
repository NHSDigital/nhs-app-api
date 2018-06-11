package mocking.tpp

import mocking.MappingBuilder
import mocking.tpp.models.Authenticate
import mocking.tpp.session.TppSessionBuilder

open class TppMappingBuilder(private val method: String, relativePath: String) : MappingBuilder(method, "$relativePath") {

    fun sessionRequest(authenticate: Authenticate) = TppSessionBuilder(authenticate)

}