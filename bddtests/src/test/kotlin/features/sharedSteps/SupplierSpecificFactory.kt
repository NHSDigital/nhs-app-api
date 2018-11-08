package features.sharedSteps

import org.junit.Assert
import utils.SerenityHelpers.Companion.setSerenityVariableIfNotAlreadySet

abstract class SupplierSpecificFactory<T>{

    protected abstract val map: java.util.HashMap<String, (() -> (T))>

    fun getForSupplier(gpSystem: String): T {
        if (!map.containsKey(gpSystem)) {
            Assert.fail("GP system '$gpSystem' is not set up.")
        }
        setSerenityVariableIfNotAlreadySet(SerenityKey.GP_SYSTEM, gpSystem)
        return map.getValue(gpSystem).invoke()
    }

    enum class SerenityKey {
        GP_SYSTEM
    }
}