import Foundation

struct DataDownloadAlert {
    let title: String
    let message: String
    
    init(alertType: DataDownloadAlertType) {
        switch alertType {
            case .OSNotSupported:
                self.title = NSLocalizedString("DataDownloadOSNotSupportedTitle", comment: "")
                self.message = NSLocalizedString("DataDownloadOSNotSupportedMessage", comment: "")
            
        }
    }
}
