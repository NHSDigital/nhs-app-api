extension String
{
    func decodeUrl() -> String?
    {
        return self.removingPercentEncoding
    }
}
