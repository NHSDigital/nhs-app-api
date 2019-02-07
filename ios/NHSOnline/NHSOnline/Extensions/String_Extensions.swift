extension String
{
    func decodeUrl() -> String? {
        return self.removingPercentEncoding
    }
    
    func containsAnyOf(_ strings: [String]) -> Bool {
        for string in strings {
            if(self.contains(string)) {
                return true
            }
        }
        return false
    }
}
