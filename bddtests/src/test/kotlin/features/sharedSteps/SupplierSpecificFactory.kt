package features.sharedSteps

import org.junit.Assert

abstract class SupplierSpecificFactory<T>{

    protected abstract val map: java.util.HashMap<String, (() -> (T))>

    fun getForSupplier(gpSystem: String): T {
        if (!map.containsKey(gpSystem)) {
            Assert.fail("GP system '$gpSystem' is not set up.")
        }
        return map.getValue(gpSystem).invoke()
    }
}