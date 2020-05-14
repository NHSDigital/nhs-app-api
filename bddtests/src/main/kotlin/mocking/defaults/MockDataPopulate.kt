package mocking.defaults

import mocking.stubs.StubbedEnvironment

open class MockDataPopulate {

    companion object {
        @JvmStatic
        fun main() {
            StubbedEnvironment().generateStubs()
        }
    }
}
