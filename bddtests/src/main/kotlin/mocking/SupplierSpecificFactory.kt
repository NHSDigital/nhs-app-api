package mocking

import org.junit.Assert
import utils.GlobalSerenityHelpers
import utils.set

abstract class SupplierSpecificFactory<T>{

    protected abstract val map: java.util.HashMap<String, (() -> (T))>

    fun getForSupplier(gpSystem: String): T {
        if (!map.containsKey(gpSystem)) {
            Assert.fail("GP system '$gpSystem' is not set up.")
        }
        GlobalSerenityHelpers.GP_SYSTEM.set(gpSystem)
        return map.getValue(gpSystem).invoke()
    }
}


