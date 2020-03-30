enum IntegrationLevel: String, Codable {
    case Gold
    case GoldOverlay
    case GoldWithNoHeaders
    case SilverWithoutWebNavigation
    case SilverWithWebNavigation
    case Bronze
    case Unknown

    init(from decoder: Decoder) throws {
        let label = try decoder.singleValueContainer().decode(String.self)

        self = IntegrationLevel(rawValue: label) ?? .Unknown

        if (self == .Unknown ) {
            Logger.logInfo(message: "An unknown IntegrationLevel enum has been encountered: %@", label)
        }
    }
}
