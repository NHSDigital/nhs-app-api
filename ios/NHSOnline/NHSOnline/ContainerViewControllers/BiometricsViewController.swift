import UIKit

class BiometricsViewController: UIViewController {

    @IBOutlet weak var HeaderTextView: UITextView!
    @IBOutlet weak var ContentTextView: ErrorTextView!
    @IBOutlet weak var TitleLabel: UILabel!
    @IBOutlet weak var ContentLabel: UILabel!
    
    override func viewDidLoad() {
        super.viewDidLoad()
    }
    
    override func viewDidAppear(_ animated: Bool) {
        ContentTextView.layer.addTopBorder(color: UIColor(red: 255, green: 237, blue: 0, alpha: 1), thickness: 3)
        HeaderTextView.text = NSLocalizedString("BiometricWarningHeader", comment: "")
        ContentTextView.text = NSLocalizedString("BiometricWarningContent", comment: "")
        TitleLabel.text = NSLocalizedString("BiometricInfoMessage1", comment: "")
        ContentLabel.text = NSLocalizedString("BiometricInfoMessage2", comment: "")
        ContentTextView.resizeErrorTextView()
    }
}
