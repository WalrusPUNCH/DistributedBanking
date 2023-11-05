@ECHO OFF

docker-compose -f docker-compose-replica-set.yaml up -d
timeout /t 5
docker exec mongodb-one mongosh --file /scripts/replica-set-init.js

PAUSE
