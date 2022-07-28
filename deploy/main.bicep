@description('The location to deploy our application to. Default is location of resource group')
param location string = resourceGroup().location

@description('Name of our application.')
param applicationName string = uniqueString(resourceGroup().id)

@description('Image used for the Todo API')
param booksApiImage string = 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'

@description('Image used for the web app')
param booksWebImage string = 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'

@description('The name of the publisher for the APIM instance')
param apimPublisherName string

@description('The email of the publisher for the APIM instance')
param apimPublisherEmail string

var containerRegistryName = '${applicationName}acr'
var logAnalyticsWorkspaceName = '${applicationName}law'
var appInsightsName = '${applicationName}ai'
var containerAppEnvironmentName = '${applicationName}env'
var apimName = '${applicationName}apim'
var cosmosAccountName = '${applicationName}db'
var databaseName = 'BookstoreDB'
var collectionName = 'books'
var autoscaleMaxThroughput = 4000
var bookApiName = 'book-api'
var bookWebName = 'book-web'
var targetPort = 80

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2022-05-15' = {
  name: cosmosAccountName
  location: location
  kind: 'MongoDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        isZoneRedundant: true
        failoverPriority: 0
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    apiProperties: {
      serverVersion: '4.2'
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource bookDatabase 'Microsoft.DocumentDB/databaseAccounts/mongodbDatabases@2022-05-15' = {
  name: databaseName
  parent: cosmosAccount
  properties: {
    resource: {
      id: databaseName
    }
  }
}

resource booksCollection 'Microsoft.DocumentDB/databaseAccounts/mongodbDatabases/collections@2022-05-15' = {
  name: collectionName
  parent: bookDatabase
  properties: {
    options: {
      autoscaleSettings: {
        maxThroughput: autoscaleMaxThroughput
      }
    }
    resource: {
      id: collectionName
      shardKey: {
        _id: 'Hash'
      }
      indexes: [
        {
          key: {
            keys: [
              '_id'
            ]
          }
        }
        {
          key: {
            keys: [
              '$**'
            ]
          }
        }
      ]
    }
  }
}

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2021-12-01-preview' = {
  name: containerRegistryName
  location: location 
  sku: {
    name: 'Basic'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    adminUserEnabled: true
  }
}

module logAnalytics 'modules/logAnalytics.bicep' = {
  name: 'logAnalytics'
  params: {
    location: location
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
  }
}

module appInsights 'modules/appInsights.bicep' = {
  name: 'appInsights'
  params: {
    appInsightsName: appInsightsName 
    location: location
    logAnalyticsWorkspaceId: logAnalytics.outputs.workspaceId 
  }
}

resource apim 'Microsoft.ApiManagement/service@2021-12-01-preview' = {
  name: apimName
  location: location
  sku: {
    capacity: 1
    name: 'Developer'
  }
  properties: {
    publisherEmail: apimPublisherEmail
    publisherName: apimPublisherName
  }
}

resource environment 'Microsoft.App/managedEnvironments@2022-03-01' = {
  name: containerAppEnvironmentName
  location: location
  properties: {
   appLogsConfiguration: {
    destination: 'log-analytics'
    logAnalyticsConfiguration: {
      customerId: logAnalytics.outputs.customerKey
      sharedKey: logAnalytics.outputs.sharedKey
    }
   } 
  }
}

resource bookApiApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: bookApiName
  location: location
  properties: {
   managedEnvironmentId: environment.id
   configuration: {
    activeRevisionsMode: 'Multiple'
    secrets: [
      {
        name: 'container-registry-password'
        value: containerRegistry.listCredentials().passwords[0].value
      }
    ]
    registries: [
      {
        server: '${containerRegistry.name}.azurecr.io'
        username: containerRegistry.listCredentials().username
        passwordSecretRef: 'container-registry-password'
      }
    ]
    ingress: {
      external: true
      targetPort: targetPort
      transport: 'http'
      allowInsecure: false
    }
   } 
   template: {
    containers: [
      {
        name: bookApiName
        image: booksApiImage
        env: [
          
        ]
        resources: {
          cpu: json('0.5')
          memory: '1.0Gi'
        }
      }
    ]
    scale: {
      minReplicas: 0
      maxReplicas: 30
    }
   }
  }
}

resource bookWebApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: bookWebName
  location: location
  properties: {
   managedEnvironmentId: environment.id
   configuration: {
    activeRevisionsMode: 'Multiple'
    secrets: [
      {
        name: 'container-registry-password'
        value: containerRegistry.listCredentials().passwords[0].value
      }
    ]
    registries: [
      {
        server: '${containerRegistry.name}.azurecr.io'
        username: containerRegistry.listCredentials().username
        passwordSecretRef: 'container-registry-password'
      }
    ]
    ingress: {
      external: true
      targetPort: targetPort
      transport: 'http'
      allowInsecure: false
    }
   }
   template: {
    containers: [
      {
        name: bookWebName
        image: booksWebImage
        env: [
          
        ]
        resources: {
          cpu: json('0.5')
          memory: '1.0Gi'
        }
      }
    ]
    scale: {
      minReplicas: 0
      maxReplicas: 30
    }
   } 
  }
}
