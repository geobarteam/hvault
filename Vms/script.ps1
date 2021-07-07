

#install chocolatey
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

#install git
choco install git

#install consul
choco install consul

#clone project
mkdir Project
cd c:\Project
git clone https://github.com/geobarteam/hvault.git

