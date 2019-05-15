import Foundation

class ApplicationState {
    var lastInteraction = Date.distantPast
    
    public func block() {
        lastInteraction = Date()
    }
    
    public func unBlock() {
        lastInteraction = Date.distantPast
    }
    
    public func isReady() -> Bool {
        return Date().timeIntervalSince(lastInteraction) > config().MenuTimeoutSeconds
    }
}
