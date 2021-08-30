$username = "administrator"
$password = "St@rwars3"
$encoded =  [System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes($username+":"+$password ))
$heders = @{
    'content-type'='application/json'
    'authorization'='Basic '+$encoded
} 
Invoke-RestMethod -Method PUT -Headers $heders -Body '{"password":"St@rwars3","tags":"administrator"}'  -Uri http://flxinflab01:15672/api/users/admin
Invoke-RestMethod -Method PUT -Headers $heders -Body '{"configure":".*","write":".*","read":".*"}' -Uri http://flxinflab01:15672/api/permissions/%2F/admin