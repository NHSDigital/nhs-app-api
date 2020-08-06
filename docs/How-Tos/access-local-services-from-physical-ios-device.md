# Access local services from physical iOS device

To test local changes against a physical iOS device the device needs to be able to access the services running under docker on the local machine. To do this we run a proxy server in the docker network.

## Configure the iOS device to use the proxy server

### Mac

1. Ensure that your iphone is connected to your mac via the usb
2. Use `cmd + space` and search for `Network Utility`
3. The first tab has a drop down which should be defaulted to `Wifi` change this to `IPhone USB`
4. Make a note of the `IP Address` in this window.

### IPhone

1. Open device settings
2. Go to wifi settings and click the information icon beside your current network
3. Go to configure proxy and click `Manual`
4. Under `Server` add the IP Address you noted in the mac steps
5. Under `Port` add `3128`
6. Click `Save`

### Testing

Once this configuration has been done try going to safari and using the IP Address and port to see if you get a not found page from squid. If this is fine then run your make command and run the app.

## Start services with proxy

Add `PROXY=true` to your `make run...` command to start the service with the proxy

e.g.

```bash
make run PROXY=true
```

or

```bash
make run-dev-stubs PFSAPI=host PROXY=true
```
