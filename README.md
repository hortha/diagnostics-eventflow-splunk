# diagnostics-eventflow-splunk

## Introduction
Extensions to Microsoft Diagnostics EventFlow to output to Splunk.

### Build Status
[![Build Status](https://dev.azure.com/hortha/Public/_apis/build/status/hortha.diagnostics-eventflow-splunk)](https://dev.azure.com/hortha/Public/_build/latest?definitionId=1) 
![NuGet](https://img.shields.io/nuget/v/Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.svg)


**Outputs**
- [Splunk (via HTTP Event Collector)](#splunk)

### Outputs

#### Splunk
*Nuget Package*: [**Spotsoft.Diagnostics.EventFlow.Outputs.Splunk**](https://www.nuget.org/packages/Spotsoft.Diagnostics.EventFlow.Outputs.Splunk/)

This output writes data to a [Splunk HTTP Event Collector (HEC)](http://docs.splunk.com/Documentation/Splunk/latest/Data/AboutHEC). Here is an example showing all possible settings:
```json
{
  "inputs": [
    {
      "type": "Microsoft.Extensions.Logging"
    }
  ],
  "filters": [
  ],
  "outputs": [
    {
      "type": "SplunkOutput",
      "serviceBaseAddress": "https://hec.mysplunkserver.com:8088",
      "authenticationToken": "B5A79AAD-D822-46CC-80D1-819F80D7BFB0",
      "host": "localhost",
      "index": "main",
      "source": "my source",
      "sourceType": "_json",
      "ignoreSslCertificateErrors" : "true",
      "maxRetryAttempts" : "5"
    }
  ],
  "schemaVersion": "2016-08-11",

  "extensions": [
    {
      "category": "outputFactory",
      "type": "SplunkOutput",
      "qualifiedTypeName": "Spotsoft.Diagnostics.EventFlow.Outputs.Splunk.SplunkOutputFactory, Spotsoft.Diagnostics.EventFlow.Outputs.Splunk"
    }
  ]
}
```
| Field | Values/Types | Required | Description |
| :---- | :-------------- | :------: | :---------- |
| `type` | "SplunkOutput" | Yes | Specifies the output type. For this output, it must be "SplunkOutput". |
| `serviceBaseAddress` | string | Yes | Base address for the Splunk HTTP Event Collector (HEC) (excluding the API URI e.g. services/collector/event/1.0). |
| `authenticationToken` | string | Yes | Defines the [HEC token](http://docs.splunk.com/Documentation/Splunk/latest/Data/UsetheHTTPEventCollector#About_Event_Collector_tokens) as configured in Splunk. This token can be used to configure the default index, source and sourcetype associated with all events which use it. |
| `host` | string | No | The [host](http://docs.splunk.com/Splexicon:Host) associated with the events. If left blank this will default to the name of the server which is executing the process using EventFlow. |
| `index` | string | No | The Splunk [index](http://docs.splunk.com/Splexicon:Index) where the event will be stored. Leave blank unless you specifically want to override the default index associated with the HEC token. |
| `source` | string | No | The Splunk [source](https://docs.splunk.com/Splexicon:Source) associated with the event. Leave blank unless you specifically want to override the default source associated with the HEC token. |
| `sourcetype` | string | No | The Splunk [source type](https://docs.splunk.com/Splexicon:Sourcetype) associated with the event. Leave blank unless you specifically want to override the default source type associated with the HEC token. |
| `ignoreSslCertificateErrors` | boolean | No | When set to true can be used against a Splunk HEC with a self-signed SSL certificate (only recommended for testing purposes). |
| `maxRetryAttempts` | integer | No | Maximum number of attempts to call the Splunk HEC API if it returns a server error HTTP Status Code (i.e. >= 500). This uses an exponential backoff algorithm. |

## Splunk Dashboard

The events might end up looking something like this on the Splunk dashboard:

![Splunk Dashboard Example](https://github.com/hortha/diagnostics-eventflow-splunk/blob/master/src/SplunkOutputNetCoreConsoleTest/SplunkOutputScreenshot.png)
