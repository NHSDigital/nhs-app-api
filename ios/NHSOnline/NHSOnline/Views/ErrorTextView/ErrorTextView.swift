import UIKit

class ErrorTextView: UITextView {
    required init?(coder aDecoder: NSCoder) {
        super.init(coder: aDecoder)
    }
    
    override init(frame: CGRect, textContainer: NSTextContainer?) {
        super.init(frame: frame, textContainer: textContainer)
    }

    func setServiceError(title:String, message:String? = nil) {
        let attributedServiceError = toHeaderAttributedText(headerText: title)
        
        if let attributedInfo = message {
            attributedServiceError.append(toAttributedText(text: attributedInfo , anyNewLines: 1))
        }
        
        self.attributedText = attributedServiceError
        self.linkTextAttributes = [
            NSAttributedString.Key.foregroundColor: UIColor.blue,
            NSAttributedString.Key.underlineColor: UIColor.blue,
            NSAttributedString.Key.underlineStyle: NSUnderlineStyle.single.rawValue
        ]
        resizeErrorTextView()
    }
    
    func resizeErrorTextView() {
        self.textContainerInset = UIEdgeInsets.init(top: 30, left: 12, bottom: 10, right: 12)
        let size = CGSize(width: self.bounds.width, height: .infinity)
        let estimatedSize = self.sizeThatFits(size)
        self.constraints.forEach{(constraint) in
            if constraint.firstAttribute == .height {
                constraint.constant = estimatedSize.height
            }
        }
    }
    
    private func toHeaderAttributedText(headerText:String)-> NSMutableAttributedString {
        let fontAttribute = [NSAttributedString.Key.font: UIFont.systemFont(ofSize: 17)]
        return NSMutableAttributedString(string: headerText, attributes: fontAttribute)
    }
    
    private func toAttributedText(text:String, anyNewLines:Int = 1)-> NSMutableAttributedString {
        var newLines = ""
        
        if anyNewLines > 0 {
            for _ in 0 ..< anyNewLines {
                newLines += "\n"
            }
        }
        
        let textAttribute = [NSAttributedString.Key.font: UIFont.systemFont(ofSize: 17), NSAttributedString.Key.paragraphStyle : NSMutableParagraphStyle()]
        
        let attributedText = NSMutableAttributedString(string: "\(newLines)\(text)", attributes: textAttribute)
        
        configureLinksInText(attributedText: attributedText)
        return attributedText
    }
    
    private func configureLinksInText(attributedText: NSMutableAttributedString) {
        
        for (textToFind, urlString) in config().ErrorTextViewUrls {
            let linkRange = attributedText.mutableString.range(of: textToFind)
            attributedText.addAttribute(NSAttributedString.Key.link, value: urlString, range: linkRange)
        }
    }
}
