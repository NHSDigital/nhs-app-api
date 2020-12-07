import Foundation
import UIKit

@testable import NHSOnline

class FileDownloadHelperMock: FileDownloadHelper {
    
    override init() {
        super.init()
        (super.downloader as! FileDownloadHelperImpl).setFileDownloadLib(FileDownloadStaticLibMock.self)
    }

    func setDownloadFileResponse(outcome: DownloadOutcome) {
        FileDownloadStaticLibMock.setDownloadFileResponse(outcome: outcome)
        
    }
    
}
