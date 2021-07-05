import Foundation
import UIKit
import PassKit

enum DownloadOutcome {
    case SUCCESS
    case ERROR
    case NOT_SUPPORTED
}

class FileDownloadHelper {
    internal var downloader: FileDownloadProtocol
    
    init() {
        let impl = FileDownloadHelperImpl()
        impl.setFileDownloadLib(FileDownloadStaticLib.self)
        
        self.downloader = impl
    }
    
    func getDownloader() -> FileDownloadProtocol {    
        self.downloader
    }
}

class FileDownloadStaticLib {
    private static let MAGIC_SIZE_VALUE: CGFloat = 64
    
    static func downloadFile(
        data: String,
        view: UIView,
        documentInteractionController: UIDocumentInteractionController,
        viewController: UIViewController) -> DownloadOutcome {
        if #available(iOS 10.0, *) {
            let base64File = createBase64File(data: data)
            return tryDownload(base64File: base64File, view: view, documentInteractionController: documentInteractionController, viewController: viewController)
        } else {
            return DownloadOutcome.NOT_SUPPORTED
        }
    }
    
    private static func createBase64File(data: String) -> Base64File {
        let splitMessage = data.components(separatedBy: "|split|")
        var base64Data = ""
        var fileName = ""
        var mimeType = ""
        if (splitMessage.count > 2) {
            base64Data = splitMessage[0]
            fileName = splitMessage[1]
            mimeType = splitMessage[2]
        }
        
        return Base64File(fileName: fileName, mimeType: mimeType, encoding: base64Data)
    }
    
    @available(iOS 10.0, *)
    private static func tryDownload(
        base64File: Base64File, view: UIView,
        documentInteractionController: UIDocumentInteractionController,
        viewController: UIViewController) -> DownloadOutcome {
        
        if (base64File.data.isEmpty) {
            return DownloadOutcome.ERROR
        }
        
        do {
            let convertedData = try base64File.decode()
            let tmpURL = FileManager.default.temporaryDirectory
                .appendingPathComponent(base64File.fileName)
            
            try convertedData.write(to: tmpURL)
            
            if base64File.fileMimeType.caseInsensitiveCompare("application/vnd.apple.pkpass") == .orderedSame {
                return handlePKPassFile(convertedData: convertedData, controller: viewController)
            }
            
            setUpDocumentController(url: tmpURL, mimeType: base64File.fileMimeType, view: view, documentInteractionController: documentInteractionController)
            
            return DownloadOutcome.SUCCESS
        } catch {
            return DownloadOutcome.ERROR
        }
    }
    
    private static func handlePKPassFile(convertedData: Data, controller: UIViewController) -> DownloadOutcome {
        guard let pkPass = try? PKPass(data: convertedData) else {
            return DownloadOutcome.ERROR
        }
        
        guard let passController = PKAddPassesViewController(pass: pkPass) else {
            return DownloadOutcome.ERROR
        }
        
        controller.present(passController, animated: true) {
            return
        }
        
        return DownloadOutcome.SUCCESS
    }
    
    private static func setUpDocumentController( url: URL, mimeType: String, view: UIView, documentInteractionController: UIDocumentInteractionController) {
        documentInteractionController.url = url
        documentInteractionController.uti = "public.image, public.content"
        documentInteractionController.name = url.lastPathComponent
        
        var frame = view.frame
        
        if (UIDevice.current.userInterfaceIdiom == .pad) {
            let midX = view.bounds.midX
            let midY = view.bounds.midY
            frame = CGRect(x: midX-MAGIC_SIZE_VALUE/2, y: midY-MAGIC_SIZE_VALUE/2, width: MAGIC_SIZE_VALUE, height: MAGIC_SIZE_VALUE)
        }
        
        if (mimeType.containsAnyOf(["image"])) {
            documentInteractionController.presentOptionsMenu(
                from: frame,
                in: view,
                animated: true
            )
        }
        else {
            documentInteractionController.presentOpenInMenu(
                from: frame,
                in: view,
                animated: true
            )
        }
    }
}

