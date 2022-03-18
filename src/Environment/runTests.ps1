param(
    $url="",
    $payload="100",
    $waitTime=5
)

Write-Host "Starting Test"

$response = Invoke-RestMethod -UseBasicParsing -Method Post -ContentType "application/json" -Body $payload -Uri $url

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