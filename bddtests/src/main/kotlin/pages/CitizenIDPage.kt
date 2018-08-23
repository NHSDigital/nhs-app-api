package pages

class CitizenIDPage : HybridPageObject() {

    fun login(username: String, password: String) {
        findByXpath("//input[@id='username']").sendKeys(username)
        findByXpath("//input[@id='password']").sendKeys(password)
        if (onMobile()) {
            hideKeyboard()
        }
        findByXpath("//input[@name='login']").click()
    }
}