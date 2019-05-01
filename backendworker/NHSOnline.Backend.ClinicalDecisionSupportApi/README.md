# Clinical Decision Support Api (CDS Api)

<br>
<br>

## Setup
--------

### Update hosts

Add the following to `/etc/hosts`

```
127.0.0.1       ems.cdss.stubs.local.bitraft.io
127.0.0.1       clinicaldecisionsupportapi.local.bitraft.io
```

### Add run configuration to IDE

Create a run configuration to target the `NHSOnline.Backend.ClinicalDecisionSupportApi` project.

### EMS CDSS Supplier Stub

Currently the CDS Api depends on the [EMS CDSS Supplier Stub Api](https://gitlab.com/ems-test-harness/ems-poc-public/cdss-supplier-stub). This is being used as a temporary CDSS supplier. Follow the instructions in the README.

<br>
<br>

## Run
--------

The CDS Api can be run using either Docker or your IDE.

### IDE

Simply run using the run configuration created in _*Setup*_ above.

### Docker

The CDS Api can be run as part of the standard `docker-compose` file: 

```
cd /path/to/nhsonline-app/backendworker
docker-compose up --build
```

or as a standalone service

```
cd /path/to/nhsonline-app/backendworker
docker-compose -f docker-compose.clinicaldecisionsupport.yml up --build
```