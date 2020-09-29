import UIKit

class CompatibilityService {
    let viewController: HomeViewController
    var hasShownCompatibilityScreen = false
    
    
    init(viewController: HomeViewController) {
        self.viewController = viewController
    }
    
    func check(isCheckEnabled: Bool = false) {
        if (!isCheckEnabled) {
            self.viewController.lifecycleHandlers?.performAppVersionCheck()
            return
        }
        
        let deviceCompatibility = self.viewController.deviceService!.performCompatibilityCheck()
        
        if (deviceCompatibility.correctVersion) {
            return
        }
        
        self.viewController.loadCompatibilityScreen(isCompatible: deviceCompatibility.canUpdate)
        self.hasShownCompatibilityScreen = true
    }
}
