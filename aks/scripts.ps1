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
vault secrets enable -path='projects-api/rabbitmq' rabbitmq
vault write rabbitmq/config/connection \
    connection_uri="http://rabbitmq:15672" \
    username="guest" \
    password="guest" 

vault write rabbitmq/config/lease \
    ttl=60 \
    max_ttl=360000 

# create my-role
vault write rabbitmq/roles/my-role \
    vhosts='{"/":{"configure": ".*", "write": ".*", "read": ".*"}}' \
    token_ttl=1h \
	token_max_ttl=2h 

# get a token for RabbitMQ
vault read rabbitmq/creds/my-role

# enable userpass auth method
vault auth enable userpass

# add user with userpass
vault write auth/userpass/users/app-vaultrabbit-dev password=starwars
vault login -method=userpass username=app-vaultrabbit-dev

# add rabbitmq policy
vault policy write rabbitmq ../rabbitmq-role-policy.hcl 

vault write auth/userpass/users/app-vaultrabbit-dev \
    token_policies="rabbitmq" \
    token_ttl=1h \
	token_max_ttl=2h 