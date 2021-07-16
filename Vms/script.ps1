

#install chocolatey
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

#install git
choco install git

#install consul
choco install consul

#clone project
mkdir C:\Project
cd c:\Project
git clone https://github.com/geobarteam/hvault.git
cp C:\Project\hvault\Vms\consul\flxsrvpoc01.json C:\ProgramData\consul\config
restart-service consul

#validate consul state
consul members

#start vault
vault server -config C:\Projects\hvault\Vms\vault\flxsrvpoc01.hcl -log-level=trace