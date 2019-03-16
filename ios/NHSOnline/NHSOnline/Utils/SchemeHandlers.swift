import Foundation

class SchemeHandlers {
    private var handlers = Dictionary<String?, BaseSchemeHandler>()
    
    func registerHandler(handler: BaseSchemeHandler) {
        handlers[handler.Scheme] = handler
    }
    
    func handleUrl(url: URL) -> Bool {
        let uc = URLComponents(url: url, resolvingAgainstBaseURL: true)
        
        if let selectedHandler = handlers[uc?.scheme] {
            return selectedHandler.handle(url: url)
        }
        return false
    }
}
