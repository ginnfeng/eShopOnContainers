{{/* Generate basic labels */}}
{{- define "mychart.labels" }}
  labels:
    generator: helm
    date: {{ now | htmlDate }}
{{- end }}

{{- define "mychart.labels2" }}
	image:
	  repository: acrsvr01.azurecr.io/apidemo/serviceorderingapi
	  tag: linux-latest
{{- end }}