import UIKit
import WebKit
import os.log

class SymptomsViewController: BaseTabBarViewController {
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        pageUrl = config().Nhs111Url
        webViewDelegate = WebViewDelegate(controller: self)
        webViewController?.webView.navigationDelegate = webViewDelegate
        webViewController?.webView.loadPage(stringUrl: pageUrl!)
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == webViewSegue {
            webViewController = segue.destination as? WebViewController
        }
    }
}
