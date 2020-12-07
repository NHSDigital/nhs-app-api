import Foundation
import UIKit
@testable import NHSOnline

class DataDownloadAlertStringHandlerMock: DataDownloadAlertHandler {
    var getDownloadAlertWasCalled = false
    
    override func getDownloadAlert(type: DataDownloadAlertType) -> UIAlertController {
        getDownloadAlertWasCalled = true
        
        let downloadAlert = DataDownloadAlert(alertType: (type))
        let alertController = UIAlertController(title: downloadAlert.title, message: downloadAlert.message, preferredStyle: .alert)
        let cancel = UIAlertAction(title: NSLocalizedString("AppUpdateRequiredCloseButtonText", comment: ""), style: .cancel) { (action) -> Void in
            alertController.dismiss(animated: true, completion: nil)
        }

        alertController.addAction(cancel)
        return alertController
    }
    
}
