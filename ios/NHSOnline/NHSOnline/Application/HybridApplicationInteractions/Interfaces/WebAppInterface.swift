import WebKit

class WebAppInterface {
    let controller: HomeViewController
    
    init(controller: HomeViewController) {
        self.controller = controller
    }
    
    func onLogout() {
        controller.dimissAlert()
    }
}
