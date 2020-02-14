struct SubService: KnownService, Codable {
    var path: String?
    var queryString: String?
    var javaScriptInteractionMode: JavaScriptInteractionMode
    var menuTab: MenuTab
    var viewMode: ViewMode
    var validateSession: Bool

    init(path: String?,
         queryString: String?,
         javaScriptInteractionMode: JavaScriptInteractionMode,
         menuTab: MenuTab,
         viewMode: ViewMode,
         validateSession: Bool ) {
        self.path = path
        self.queryString = queryString
        self.javaScriptInteractionMode = javaScriptInteractionMode
        self.menuTab = menuTab
        self.viewMode = viewMode
        self.validateSession = validateSession
    }
}
