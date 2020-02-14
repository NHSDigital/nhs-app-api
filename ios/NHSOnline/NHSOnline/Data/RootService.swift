import Foundation

struct RootService: KnownService, Codable {
    var url: String
    var javaScriptInteractionMode: JavaScriptInteractionMode
    var menuTab: MenuTab
    var viewMode: ViewMode
    var validateSession: Bool
    var subServices: [SubService]?

    init(url: String,
         javaScriptInteractionMode: JavaScriptInteractionMode,
         menuTab: MenuTab,
         viewMode: ViewMode,
         validateSession: Bool,
         subServices: [SubService]?) {
        self.url = url
        self.javaScriptInteractionMode = javaScriptInteractionMode
        self.menuTab = menuTab
        self.viewMode = viewMode
        self.validateSession = validateSession
        self.subServices = subServices
    }
}