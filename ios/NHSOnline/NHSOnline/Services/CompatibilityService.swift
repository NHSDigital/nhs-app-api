import UIKit

class CompatibilityService {
    let viewController: HomeViewController
    var hasShownIncompatibleScreen = false
    var hasShownUpdateDialog = false
    
    
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
        
        if (!deviceCompatibility.canUpdate) {
            self.viewController.loadIncompatibleScreen()
            self.hasShownIncompatibleScreen = true
        } else {
            // This will be changed to load the compatible shutter screen
            self.viewController.lifecycleHandlers?.showUpdateDialog()
            self.hasShownUpdateDialog = true
        }
    }
}
