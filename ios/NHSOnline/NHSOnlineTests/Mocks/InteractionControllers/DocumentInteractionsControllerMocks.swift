import WebKit
@testable import NHSOnline

class DocumentInteractionsControllerMocks: UIDocumentInteractionController {
    var menuOpened = false

    override func presentOpenInMenu(from rect: CGRect, in view: UIView, animated: Bool) -> Bool {
        menuOpened = true;
        return true;        }
    override func presentOptionsMenu(from rect: CGRect, in view: UIView, animated: Bool) -> Bool {
    menuOpened = true;
    return true;        }
}
