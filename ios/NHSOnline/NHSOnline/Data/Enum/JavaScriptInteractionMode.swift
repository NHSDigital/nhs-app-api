enum JavaScriptInteractionMode: String, Codable {
    case None
    case NhsApp
    case Unknown

    init(from decoder: Decoder) throws {
        let label = try decoder.singleValueContainer().decode(String.self)
        self = JavaScriptInteractionMode(rawValue: label) ?? .Unknown

        if (self == .Unknown ) {
            Logger.logInfo(message: "An unknown JavaScriptInteractionMode enum has been encountered: %@", label)
        }
    }
}
