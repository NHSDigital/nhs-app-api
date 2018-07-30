package features.oneOneOneOnline.Steps

import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import pages.CheckMySymtomsPage


open class CheckMySymptoms {

    lateinit var checkMySymptoms: CheckMySymtomsPage


    @Step
    fun isPageDisplayedLoggedOut(): Boolean {
        return checkMySymptoms.getCheckSymptomsTitleLoggedOut()
    }

    @Step
    fun isPageDisplayedLoggedIn(): Boolean {
        return checkMySymptoms.getCheckSymptomsTitleLoggedIn()

    }
}