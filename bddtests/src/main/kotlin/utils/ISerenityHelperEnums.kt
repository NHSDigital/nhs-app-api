package utils

import net.serenitybdd.core.Serenity
import org.junit.Assert

interface ISerenityHelperEnums

fun <T>ISerenityHelperEnums.getOrFail() : T {
    Assert.assertTrue("Test setup incorrect, $this to be set",
            Serenity.hasASessionVariableCalled(this))
    return Serenity.sessionVariableCalled<T>(this)
}

fun <T>ISerenityHelperEnums.getOrNull() : T? {
    return SerenityHelpers.getValueOrNull(this)
}

fun <T>ISerenityHelperEnums.set(value : T) {
    Serenity.setSessionVariable(this).to(value)
}

fun ISerenityHelperEnums.isTrueOrFalse() : Boolean {
    return this.getOrNull<Boolean>() == true
}

fun <T>ISerenityHelperEnums.setIfNotAlreadySet(valueToSet : T) {
    val currentStoredValue = this.getOrNull<Any>()
    if (currentStoredValue == null) {
        this.set(valueToSet)
    }
    else if(currentStoredValue != valueToSet)
        Assert.assertEquals("Test setup incorrect, $this changing values",
                currentStoredValue,
                valueToSet)
}

