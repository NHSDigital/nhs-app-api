import WebKit

extension WKWebView {
    
    func loadPage(url: String) {
        let urlRequest = URLRequest(url: URL(string: url)!)
        
        self.load(urlRequest)
    }
}

