@description('The name of the workspace')
param logAnalyticsWorkspaceName string

@description('The location of the workspace')
param location string

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

output workspaceId string = logAnalytics.id
output customerKey string = logAnalytics.properties.customerId
output sharedKey string = logAnalytics.listKeys().primarySharedKey
