import UIKit

class BaseTabBarViewController: UIViewController {
    @IBOutlet weak var WebViewContainer: UIView!
    @IBOutlet weak var NativeViewContainer: UIView!
    
    var webViewController: WebViewController?
    var nativeViewController: NativeViewController?
    var webViewDelegate: WebViewDelegate?
    var pageUrl: String?
    let webViewSegue = "webViewSegue"
    let nativeViewSegue = "nativeViewSegue"
}
