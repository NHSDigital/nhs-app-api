{{- define "nhsapp.name" -}}
{{- default "nhsapp" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "nhsapp.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "nhsapp.preStopHook" }}  
preStop:
  exec:
    command: ["/bin/sh", "-c", "echo 'PreStop hook invoked, sleeping for 10 seconds'; sleep 10; echo 'PreStop hook completed'"]
{{- end }}