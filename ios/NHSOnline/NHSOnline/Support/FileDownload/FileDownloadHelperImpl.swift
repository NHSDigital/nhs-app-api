import Foundation
import UIKit

protocol FileDownloadProtocol {
    func downloadFile(
        data: String,
        view: UIView,
        documentInteractionController: UIDocumentInteractionController,
        viewController: UIViewController) -> DownloadOutcome
}

class FileDownloadHelperImpl: FileDownloadProtocol {
    
    private var fileDownloadLib: FileDownloadHelperBridge.Type!
    
    func setFileDownloadLib(_ lib: FileDownloadHelperBridge.Type) {
        self.fileDownloadLib = lib
    }
    
    func downloadFile(
        data: String,
        view: UIView,
        documentInteractionController: UIDocumentInteractionController,
        viewController: UIViewController) -> DownloadOutcome {
        return self.fileDownloadLib.downloadFile(
            data: data,
            view: view,
            documentInteractionController: documentInteractionController,
            viewController: viewController)
    }
}
    
protocol FileDownloadHelperBridge {
        
    static func downloadFile(
        data: String,
        view: UIView,
        documentInteractionController: UIDocumentInteractionController,
        viewController: UIViewController) -> DownloadOutcome
}


extension FileDownloadStaticLib: FileDownloadHelperBridge {}
