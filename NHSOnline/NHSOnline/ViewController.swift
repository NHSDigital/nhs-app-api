import UIKit
import WebKit

class ViewController: UIViewController, UITabBarDelegate {
    @IBOutlet weak var webView: WKWebView!
    @IBOutlet weak var tabBar: UITabBar!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        initializeTabBar()
        loadWebView()
    }
    
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }
    
    public func tabBar(_ tabBar: UITabBar, didSelect item: UITabBarItem) {
        switch item.tag {
        case 4:
            print("tab item 4")
        case 3:
            print("tab item 3")
        case 2:
            print("tab item 2")
        case 1:
            print("tab item 1")
        default:
            print("tab item 0")
        }
    }
    
    private func initializeTabBar() {
        tabBar.delegate = self
        tabBar.itemSpacing = 20.0
    }
    
    private func loadWebView() {
        let webViewEndpointUrl = Bundle.main.infoDictionary!["WebViewEndpointUrl"] as! String
        let urlRequest = URLRequest(url: URL(string: webViewEndpointUrl)!)
        webView.frame = view.bounds

        webView.load(urlRequest)
    }
}

