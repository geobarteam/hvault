# Enable dashboard
kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.2.0/aio/deploy/recommended.yaml
kubectl proxy


#send command to vault
kubectl exec vault-56bccf455f-9c8wj -it -- vault version

#bash start vault: use wsl!
export VAULT_ADDR='http://127.0.0.1:8200'
export VAULT_TOKEN='some-root-token'

vault auth enable approle

#Setup rabbitmq engine
vault secrets enable rabbitmq
vault write rabbitmq/config/connection \
    connection_uri="http://rabbitmq:15672" \
    username="guest" \
    password="guest"

#Get a token
vault write rabbitmq/roles/my-role \
    vhosts='{"/":{"write": ".*", "read": ".*"}}'