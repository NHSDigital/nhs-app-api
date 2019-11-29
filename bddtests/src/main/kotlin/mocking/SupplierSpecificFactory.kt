package mocking

import constants.Supplier
import org.junit.Assert
import utils.SerenityHelpers

abstract class SupplierSpecificFactory<T>{

    protected abstract val map: java.util.HashMap<Supplier, (() -> (T))>

    fun getForSupplier(gpSystem: Supplier): T {
        if (!map.containsKey(gpSystem)) {
            Assert.fail("GP system '$gpSystem' is not set up.")
        }
        SerenityHelpers.setGpSupplier(gpSystem)
        return map.getValue(gpSystem).invoke()
    }
}


