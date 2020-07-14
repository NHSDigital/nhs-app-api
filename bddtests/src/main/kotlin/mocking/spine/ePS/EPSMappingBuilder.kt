package mocking.spine.ePS

import mocking.spine.SpineConfiguration
import mocking.spine.SpineMappingBuilder

open class EPSMappingBuilder(method: String, relativePath: String = "")
    : SpineMappingBuilder(method, relativePath) {

    companion object {
        val configuration = SpineConfiguration("", "", "", "")
    }
}
