{{- define "nhsapp.name" -}}
{{- default "nhsapp" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "nhsapp.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}
