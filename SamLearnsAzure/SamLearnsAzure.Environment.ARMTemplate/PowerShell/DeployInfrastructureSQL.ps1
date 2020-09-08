﻿$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$timing = ""
$timing = -join($timing, "1. Deployment started: ", $stopwatch.Elapsed.TotalSeconds, "`n")
Write-Host "1. Deployment started: "$stopwatch.Elapsed.TotalSeconds

$appPrefix = "samsapp"
$environment = "${{parameters.environmentLowercase}}"
$resourceGroupName = "${{parameters.resourceGroupName}}"
$location = "${{parameters.resourceGroupLocation}}" 
$locationShort = "${{parameters.resourceGroupLocationShort}}"                
$keyVaultName = "$appPrefix-$environment-$locationShort-vault" #Must be <= 23 characters
$dataKeyVaultName = "${{parameters.keyVaultName}}"
$serviceAPIName = "$appPrefix-$environment-$locationShort-service"
$webSiteName = "$appPrefix-$environment-$locationShort-web"
$sqlServerName = "$appPrefix-$environment-$locationShort-sqlserver"
$webhostingName = "$appPrefix-$environment-$locationShort-hostingplan"
$storageAccountName = "$appPrefix$environment$($locationShort)storage" #Must be <= 24 lowercase letters and numbers.
$actionGroupName = "$appPrefix-$environment-$locationShort-actionGroup"
$actionGroupShortName = "$environment-actgrp"
$applicationInsightsName = "$appPrefix-$environment-$locationShort-appinsights"
$applicationInsightsAvailablityTestName = "Availability home page test-$applicationInsightsName"
$redisCacheName = "$appPrefix-$environment-$locationShort-redis"
$cdnName = "$appPrefix-$environment-$locationShort-cdn"   
$sqlDatabaseName = "${{parameters.databaseName}}" 
$sqlAdministratorLoginUser = "${{parameters.databaseLoginName}}"
$sqlAdministratorLoginPassword = "${{parameters.databaseLoginPassword}}" #The password is case-sensitive and must contain lower case, upper case, numbers and special characters. 
$administratorUserLogin = "c6193b13-08e7-4519-b7b4-e6b1875b15a8"
$administratorUserSid = "076f7430-ef4f-44e0-aaa7-d00c0f75b0b8"
$websiteDomainName = "$environment.samlearnsazure.com"
$letsEncryptAppServiceContributerClientSecret="RSRf?J_z+1t6W*EPpxkVhXTs9Szirku5"
$azureDevOpsPrincipalId = "e60b0582-1d81-4ab3-92db-fbdc53ddeb92"
$contactEmailAddress="samsmithnz@gmail.com"
$templatesLocation = "$(build.artifactstagingdirectory)\drop\EnvironmentARMTemplate\Templates"               
if ($keyVaultName.Length -gt 24)
{
    Write-Host "Key vault name must be 3-24 characters in length"
    Break
}
if ($storageAccountName.Length -gt 24)
{
    Write-Host "Storage account name must be 3-24 characters in length"
    Break
}
if ($letsEncryptAppServiceContributerClientSecret -eq $null)
{
    Write-Host "$letsEncryptAppServiceContributerClientSecret is null. Please set this secret before continuing"
    Break
}
$timing = -join($timing, "2. Variables created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "2. Variables created: "$stopwatch.Elapsed.TotalSeconds

#SQL
$sqlOutput = az deployment group create --resource-group $resourceGroupName --name $sqlServerName --template-file "$templatesLocation\SQL.json" --parameters sqlServerName=$sqlServerName databaseName=$sqlDatabaseName sqlAdministratorLogin=$sqlAdministratorLoginUser sqlAdministratorLoginPassword=$sqlAdministratorLoginPassword administratorUserLogin=$administratorUserLogin administratorUserSid=$administratorUserSid storageAccountName=$storageAccountName storageAccountAccessKey=$storageAccountAccessKey
$sqlJSON = $sqlOutput | ConvertFrom-Json
$sqlServerAddress = $sqlJSON.properties.outputs.sqlServerIPAddress.value
$sqlConnectionStringName = "ConnectionStrings--SamsAppConnectionString$Environment"
$sqlConnectionStringValue = "Server=tcp:$sqlServerName.database.windows.net,1433;Initial Catalog=$sqlDatabaseName;Persist Security Info=False;User ID=$sqlAdministratorLoginUser;Password=$sqlAdministratorLoginPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
Write-Host "Setting value $sqlConnectionStringValue for $sqlConnectionStringName to key vault"
az keyvault secret set --vault-name $dataKeyVaultName --name "$sqlConnectionStringName" --value $sqlConnectionStringValue #Upload the secret into the key vault
$timing = -join($timing, "8. SQL created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "8. SQL created: "$stopwatch.Elapsed.TotalSeconds

Write-Host "sqlServerIPAddress: "$sqlServerIPAddress
$timing = -join($timing, "14. All Done created: ", $stopwatch.Elapsed.TotalSeconds, "`n");
Write-Host "14. All Done: "$stopwatch.Elapsed.TotalSeconds
Write-Host "Timing: `n$timing"
Write-Host "Were there errors? (If the next line is blank, then no!) $error"