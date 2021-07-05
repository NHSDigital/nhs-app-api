import Foundation

class BlobSchemeHandler: BaseSchemeHandler {
    public var Scheme: String? { return "blob" }
    func handle(url: URL) -> Bool {
        let uc = URLComponents(url: url, resolvingAgainstBaseURL: true)
        if(uc?.scheme != Scheme) {
            return false
        }

        return true;
    }
}
