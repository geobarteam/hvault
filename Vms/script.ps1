

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

#set vault addrr
$env:VAULT_ADDR = "http://127.0.0.1:8200"
vault operator init
Vault operator unseal #seal key here repeat 3x

#root keys
vault operator generate-root -init
vault operator generate-root
vault operator generate-root -decode=K2ZNBgZrQ1BCKDEtMFk1OFxRXlkoIQVqAwY -otp=XH5l18q9rdEzfiyp162ilgP8aj

#set vault token
$env:VAULT_TOKEN = 's.xj7S2i0LtWV0LHmgl0DFURbl'

#enable app role auth
vault auth enable approle

#Setup rabbitmq engine
vault secrets enable -path='[TenantName]/rabbitmq' rabbitmq

##configure connection settings
vault write rabbitmq/config/connection connection_uri="http://flxinflab01:15672" username="administrator" password="St@rwars3"

#create a role for rabbitmq
#configure lease
vault write rabbitmq/config/lease ttl=60 max_ttl=360000 

# create flbe-application that provide policy in RabbitMQ 
vault write rabbitmq/roles/flbe-application vhosts='{\"/\":{\"configure\": \".*\", \"write\": \".*\", \"read\": \".*\"}}'




