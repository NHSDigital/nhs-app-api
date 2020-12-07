import Foundation
import UIKit

@testable import NHSOnline

class FileDownloadStaticLibMock: FileDownloadHelperBridge {
    
    private static var outcome = DownloadOutcome.SUCCESS
    
    static func setDownloadFileResponse(outcome: DownloadOutcome) {
        FileDownloadStaticLibMock.outcome = outcome
    }
    
    static func downloadFile(data: String, view: UIView, documentInteractionController: UIDocumentInteractionController) -> DownloadOutcome {
        return FileDownloadStaticLibMock.outcome
    }
}

