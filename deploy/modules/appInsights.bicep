@description('The name of the App Insights workspace')
param appInsightsName string

@description('The location of the App Insights workspace')
param location string

@description('The Log Analytics Workspace Id to associate this App Insights workspace to')
param logAnalyticsWorkspaceId string

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspaceId
  }
}
