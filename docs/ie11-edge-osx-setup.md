# Local IE11 & Edge Setup on OSX

This guide will help you setup a local environment for testing the NHS App on Internet Explorer 11 / Edge using a virtual machine.

## Before you start

- Ensure you have around 20GB of free space (you can delete the zip and `ova` files after setup to reclaim space)
- Install VirtualBox from [here](https://www.virtualbox.org/)
- Install Oracle VM VirtualBox Extension Pack from [here](https://www.virtualbox.org/wiki/Downloads)
- Download the free Microsoft Edge VM (VirtualBox) from [here](https://az792536.vo.msecnd.net/vms/VMBuild_20190311/VirtualBox/MSEdge/MSEdge.Win10.VirtualBox.zip)
- Connect to your VPN
- Start the local NHS App environment (and/or local instances of web etc.)

## Virtual Box Setup

- Unzip file from download
- Open VirtualBox
    - Click `File` -> `Import Appliance...`
    - Select the `ova` file you unzipped
    - Accept defaults
- Start the new VM once import has finished
- Right click on the new VM in VirtualBox Manager
    - Click `Settings...`
    - Go to `Network` -> `Advanced` -> `Port Forwarding`
    - Click the `+` icon on the right and set:
        - `Name`: NHS App API
        - `Host IP`: 0.0.0.0
        - `Host Port`: 8089
        - `Guest Port`: 8089
    - Click the `+` icon on the right and set:
        - `Name`: NHS App Web
        - `Host IP`: 0.0.0.0
        - `Host Port`: 3000
        - `Guest Port`: 3000
    - Click `OK`
    - Click `OK`

## Accessing NHS App

- On your host, open your VPN client and copy the Client IP Address (v4)
    - In `Cisco AnyConnect`, click the tray icon and click `Show Statistics Window` to view this info
- In the VM enter `Notepad` into the start menu, right click the icon and select `Run as Administrator`
- Click `File` -> `Open` and select `C:\Windows\System32\drivers\etc\hosts` - *you may have to select `All Files (*.*)` from the file type dropdown to see it*
- Ensure the below lines are present and save the file
    ```
    # NHS App
    <IP Address> web.local.bitraft.io
    <IP Address> api.local.bitraft.io
    ```

    *Replace `<IP Address>` with the address you discovered at the start of this section*
- Open a browser inside the Windows VM and navigate to `http://web.local.bitraft.io:3000`
