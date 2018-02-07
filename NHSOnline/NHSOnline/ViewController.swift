//
//  ViewController.swift
//  NHSOnline
//
//  Created by Karma Tsering on 05/02/2018.
//  Copyright © 2018 NHS Digital. All rights reserved.
//

import UIKit
import WebKit

class ViewController: UIViewController, UITabBarDelegate {
    @IBOutlet weak var webView: WKWebView!
    @IBOutlet weak var tabBar: UITabBar!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        tabBar.delegate = self
        let urlRequest = URLRequest(url: URL(string: "https://111.nhs.uk")!)
        webView.load(urlRequest)
        // Do any additional setup after loading the view, typically from a nib.
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
    
    public func tabBar(_ tabBar: UITabBar, didSelect item: UITabBarItem){
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
}

