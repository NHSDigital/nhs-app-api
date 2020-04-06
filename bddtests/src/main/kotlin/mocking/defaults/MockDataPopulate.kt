package mocking.defaults

import mocking.stubs.StubbedEnvironment
import utils.GlobalSerenityHelpers
import utils.set

open class MockDataPopulate {

    companion object {
        @JvmStatic
        fun main(arguments: Array<String>) {
            GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.set(false)
            StubbedEnvironment().generateStubs()
        }
    }
}
