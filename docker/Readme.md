`docker-compose -f docker-compose-replica-set.yaml up`

`docker-compose -f docker-compose-replica-set.yaml down`


<br/>

`docker exec -it mongodb-one mongosh --eval "rs.initiate({_id:'banking_set', members: [{_id:0, host: 'mongodb-one', 'priority': 1},{_id:1, host: 'mongodb-two', 'priority': 0.5},{_id:2, host: 'mongodb-three', 'priority': 0.5}]})"`


`docker exec -it mongodb-one mongosh --port 27027 --eval "rs.status()"`



<br/>


`mongodb://mongodb-one:27027,mongodb-two:27028,mongodb-three:27029/?replicaSet=banking_set`