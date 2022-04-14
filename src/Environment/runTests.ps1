param(
    $url="",
    $tests=10000,
    $min=1,
    $max=20,
    $waitTime=5,
    $resourceGroupName,
    $serviceBusName
)

Write-Host "Starting Test"

$payload = @{
    numberOfTests = $tests
    minMessageSize = $min
    maxMessageSize = $max
}

$response = Invoke-RestMethod -UseBasicParsing -Method Post -ContentType "application/json" -Body ($payload | ConvertTo-Json) -Uri $url

Write-Host $response

Start-Sleep -Seconds $waitTime

Write-Host "Checking Status"

$statusResponse = Invoke-RestMethod -Method Get -Uri $response.statusQueryGetUri -UseBasicParsing
$counter = 0

while($statusResponse.runtimeStatus -ne "Completed") {
    Start-Sleep -Seconds $waitTime
    $statusResponse = Invoke-RestMethod -Method Get -Uri $response.statusQueryGetUri -UseBasicParsing
    Write-Host "." -NoNewline
    $counter++
    if (($counter % 5) -eq 0) {
        Write-Host (az servicebus namespace show -g $resourceGroupName -n $serviceBusName --query "sku.capacity") -NoNewline
    }
}

Write-Host
Write-Host "Test Completed:"
Write-Host $statusResponse
Write-Host

$capacityUnits = az servicebus namespace show -g $resourceGroupName -n $serviceBusName --query "sku.capacity"
Write-host "Service Bus Capacity Units: $capacityUnits"
