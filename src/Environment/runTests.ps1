param(
    $url="",
    $tests=10000,
    $min=1,
    $max=20,
    $waitTime=5
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

while($statusResponse.runtimeStatus -ne "Completed") {
    Start-Sleep -Seconds $waitTime
    $statusResponse = Invoke-RestMethod -Method Get -Uri $response.statusQueryGetUri -UseBasicParsing
    Write-Host "." -NoNewline
}

Write-Host $statusResponse