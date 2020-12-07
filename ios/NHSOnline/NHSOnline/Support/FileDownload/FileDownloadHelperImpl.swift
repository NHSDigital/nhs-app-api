import Foundation
import UIKit

protocol FileDownloadProtocol {
    func downloadFile(data: String, view: UIView, documentInteractionController: UIDocumentInteractionController) -> DownloadOutcome
}

class FileDownloadHelperImpl: FileDownloadProtocol {
    
    private var fileDownloadLib: FileDownloadHelperBridge.Type!
    
    func setFileDownloadLib(_ lib: FileDownloadHelperBridge.Type) {
        self.fileDownloadLib = lib
    }
    
    func downloadFile(data: String, view: UIView, documentInteractionController: UIDocumentInteractionController) -> DownloadOutcome {
        return self.fileDownloadLib.downloadFile(data: data, view: view, documentInteractionController: documentInteractionController)
    }
}
    
protocol FileDownloadHelperBridge {
        
    static func downloadFile(data: String, view: UIView, documentInteractionController: UIDocumentInteractionController) -> DownloadOutcome
}


extension FileDownloadStaticLib: FileDownloadHelperBridge {}
