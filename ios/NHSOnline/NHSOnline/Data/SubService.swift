struct SubService: KnownService, Codable {
    var path: String?
    var queryString: String?
    var javaScriptInteractionMode: JavaScriptInteractionMode
    var menuTab: MenuTab
    var integrationLevel: IntegrationLevel
    var validateSession: Bool

    init(path: String?,
         queryString: String?,
         javaScriptInteractionMode: JavaScriptInteractionMode,
         menuTab: MenuTab,
         integrationLevel: IntegrationLevel,
         validateSession: Bool ) {
        self.path = path
        self.queryString = queryString
        self.javaScriptInteractionMode = javaScriptInteractionMode
        self.menuTab = menuTab
        self.integrationLevel = integrationLevel
        self.validateSession = validateSession
    }
}
