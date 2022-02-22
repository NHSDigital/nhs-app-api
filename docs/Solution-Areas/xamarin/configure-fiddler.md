# Configure Fiddler
Fiddler is a network proxy allowing you to inspect and modify network traffic.

<b>Note: Ensure to toggle the capture switch off and close Fiddler when finished as this can mess with some applications.</b>

[[_TOC_]]

## Fiddler setup

1. Install Fiddler from here: https://www.telerik.com/fiddler/fiddler-everywhere (this is a trial, you will need to obtain a license to keep using. Those in Kainos can reach out to systems to obtain this)

2. Once setup, in Fiddler go to Settings > HTTPS > Advanced Settings > Export root certificate

 ![Export certificate](Images/ExportCertificate.png)

3. In Fiddler settings go to Connections and check <b>Allow remote computers to connect.</b>

## On an iOS simulator:
1. Drag the certificate onto the iOS simulator.

2. Ensure the certificate is trusted on the iOS simulator, by going (on the device) to Settings > General > About > Certificate Trust Settings and toggling it:
  ![IOS Certificate](Images/iOSCertificate.png)

## On an Android emulator:
1. Install the Fiddler certificate on the emulator

2. On the emulator go to Settings -> Network and click the Wifi network. Click advanced and obtain the gateway address.

  ![Network details - Android](Images/Details.png)

3. Add the proxy onto the Wifi network specifying the gateway address from step 2 as the host, and using the default Fiddler port of `8866`.

4. To verify this is setup correctly go to http://ipv4.fiddler:8866 in the browser on the emulator.

5. Add the following into `network_security_config.xml`: 
```
<?xml version="1.0" encoding="utf-8"?>
<network-security-config>
    <base-config>
        <trust-anchors>
            <!-- Trust preinstalled CAs -->
            <certificates src="system" />
            <!-- HERE: Additionaly trust user added CAs -->
            <certificates src="user"/>
        </trust-anchors>
    </base-config>
</network-security-config>
```

6. Rebuild and deploy the app, you should now be able to capture traffic.

## Capturing traffic
1. In Fiddler toggle the capturing switch on and you should start to see traffic in the dashboard.

2. You can add a filter to only capture traffic from a certain URL or path which will cut down the amount of data.

3. To ensure it is working on your device, launch the app.
 ![Traffic capture](Images/Capture.png)

4. You can also add rules within Fiddler to modify the reponses to particular requests. eg. For all paths ending with .js you could respond with a 404 to see how the application behaves.


