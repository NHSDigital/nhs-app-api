@testable import NHSOnline

class SplashScreenViewMocks: SplashScreen {
    var isVisible = true
    
    override func hide() {
        isVisible = false
    }
}
