This folder contains scripts that are useful during development.

The best way to use this folder is to add it into your PATH environment variable.


Scripts
=======

# build_all.sh
This script will fully rebuild all your containers; including updating the Service Journey Rules volume
To use it; please run it from the ROOT of the repo (e.g. nhsonline-app)
`utils/build_all.sh`

# docker-port-forward.sh
This script reads all of the ports exposed by containers currently running in docker and then configures port forwarding from localhost through to the docker virtual machine. Effectively this makes Docker Toolbox behave like Docker for Windows/Mac. 

Run from the docker quickstart terminal.  The script will block and, when killed with ctrl-c, will terminate the port forwarding.
