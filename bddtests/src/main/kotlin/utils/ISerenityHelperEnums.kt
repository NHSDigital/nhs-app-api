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
