enum ViewMode: String, Codable {
    case AppTab
    case WebView
    case Unknown

    init(from decoder: Decoder) throws {
        let label = try decoder.singleValueContainer().decode(String.self)

        self = ViewMode(rawValue: label) ?? .Unknown

        if (self == .Unknown ) {
            Logger.logInfo(message: "An unknown ViewMode enum has been encountered: %@", label)
        }
    }
}