enum MenuTab: String, Codable {
   case None
   case Advice
   case Appointments
   case Prescriptions
   case MyRecord
   case More
   case Unknown

   init(from decoder: Decoder) throws {
      let label = try decoder.singleValueContainer().decode(String.self)

      self = MenuTab(rawValue: label) ?? .Unknown

      if (self == .Unknown ) {
          Logger.logInfo(message: "An unknown MenuTab enum has been encountered: %@", label)
      }
   }
}
