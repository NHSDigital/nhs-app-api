import Foundation

struct RootService: KnownService, Codable {
    var url: String
    var javaScriptInteractionMode: JavaScriptInteractionMode
    var menuTab: MenuTab
    var integrationLevel: IntegrationLevel
    var validateSession: Bool
    var subServices: [SubService]?

    init(url: String,
         javaScriptInteractionMode: JavaScriptInteractionMode,
         menuTab: MenuTab,
         integrationLevel: IntegrationLevel,
         validateSession: Bool,
         subServices: [SubService]?) {
        self.url = url
        self.javaScriptInteractionMode = javaScriptInteractionMode
        self.menuTab = menuTab
        self.integrationLevel = integrationLevel
        self.validateSession = validateSession
        self.subServices = subServices
    }
}
