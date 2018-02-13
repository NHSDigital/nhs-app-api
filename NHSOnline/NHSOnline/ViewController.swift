import UIKit
import WebKit

class ViewController: UIViewController {
    @IBOutlet weak var webView: WKWebView!
    @IBOutlet weak var tabBar: UITabBar!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        loadDefaultWebPage()
    }
    
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }
    
    private func loadDefaultWebPage() {
        let baseUrl = Bundle.main.infoDictionary!["BaseUrl"] as! String
        let urlRequest = URLRequest(url: URL(string: baseUrl)!)
        webView.load(urlRequest)
    }
}

