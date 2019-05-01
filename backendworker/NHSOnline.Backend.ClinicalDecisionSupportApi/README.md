# Clinical Decision Support Api (CDS Api)

<br>
<br>

## Setup
--------

### Update hosts

Add the following to `/etc/hosts`

```
127.0.0.1       clinicaldecisionsupportapi.local.bitraft.io
```

### Add run configuration to IDE

Create a run configuration to target the `NHSOnline.Backend.ClinicalDecisionSupportApi` project.

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