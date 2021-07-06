# Enable dashboard
kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.2.0/aio/deploy/recommended.yaml
kubectl proxy


#send command to vault
kubectl exec vault-56bccf455f-9c8wj -it -- vault version

#bash start vault: use wsl!
export VAULT_ADDR='http://127.0.0.1:8200'
export VAULT_TOKEN='some-root-token'

vault auth enable approle

vault secrets enable -path='secrets' -version=2 kv

#Setup rabbitmq engine
vault secrets enable rabbitmq
#with path
vault secrets enable -path='projects-api/rabbitmq' rabbitmq
#configure connection settings
vault write rabbitmq/config/connection \
    connection_uri="http://rabbitmq:15672" \
    username="guest" \
    password="guest" 
#configure lease
vault write rabbitmq/config/lease \
    ttl=60 \
    max_ttl=360000 

# create my-role that provide policy in RabbitMQ 
vault write rabbitmq/roles/my-role \
    vhosts='{"/":{"configure": ".*", "write": ".*", "read": ".*"}}' 

# get a token for RabbitMQ with my-role
vault read rabbitmq/creds/my-role

# add rabbitmq policy
vault policy write rabbitmq ../rabbitmq-role-policy.hcl 

#---Userpass
# enable userpass auth method
vault auth enable userpass

# add user with userpass
vault write auth/userpass/users/app-vaultrabbit-dev password=starwars
vault login -method=userpass username=app-vaultrabbit-dev

# add policy to userpass token
vault write auth/userpass/users/app-vaultrabbit-dev \
    token_policies="rabbitmq" \
    token_ttl=1h \
	token_max_ttl=2h

#---Approle
# create approle for app-vaultrabbit-dev
vault write auth/approle/role/app-vaultrabbit-dev \
	  role_id="app-vaultrabbit-dev" \
		token_policies="rabbitmq" \
		token_ttl=1h \
		token_max_ttl=2h \
		secret_id_num_uses=5
# get secret-id
vault write -f auth/approle/role/app-vaultrabbit-dev/secret-id
