FROM nhsapp.azurecr.io/nhsonline-azure-func-cli:sdk-6.0-func-4

COPY NHSOnline.MetricLogFunctionApp/. /src/NHSOnline.MetricLogFunctionApp/
COPY NHSOnline.MetricLogFunctionApp.IntegrationTests/inttests.settings.json /src/NHSOnline.MetricLogFunctionApp/local.settings.json

WORKDIR /src/NHSOnline.MetricLogFunctionApp

CMD func start
