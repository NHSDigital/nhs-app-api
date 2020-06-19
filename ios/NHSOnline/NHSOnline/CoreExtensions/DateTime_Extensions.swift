import Foundation

extension Date {
    init(ticks: UInt64) {
        self.init(timeIntervalSince1970: Double(ticks)/10_000_000 - 62_135_596_800)
    }

    var ticks: UInt64 {
        return UInt64((self.timeIntervalSince1970 + 62_135_596_800) * 10_000_000)
    }
    
    public var iso8601: String {
        if #available(iOS 10.0, *) {
            return ISO8601DateFormatter().string(from: self)
        } else {
            let formatter = DateFormatter()
            formatter.locale = Locale(identifier: "en_US_POSIX")
            formatter.dateFormat = "yyyy-MM-dd'T'HH:mm:ssZZZZZ"
            
            return formatter.string(from: self)
        }
    }
}
