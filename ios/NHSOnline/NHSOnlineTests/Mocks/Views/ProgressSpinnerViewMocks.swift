import WebKit
@testable import NHSOnline

class ProgressSpinnerViewMocks: ProgressSpinner {
    var isVisible = true
    
    override func hide(uiView: UIView) {
        isVisible = false
    }
}
