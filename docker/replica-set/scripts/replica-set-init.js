﻿var config = {
    "_id": "banking_set",
    "version": 1,
    "writeConcernMajorityJournalDefault": false,
    "members": [
        {
            "_id": 0,
            "host": "mongodb-one:27027",
            "priority": 1
        },
        {
            "_id": 1,
            "host": "mongodb-two:27028",
            "priority": 0.5
        },
        {
            "_id": 2,
            "host": "mongodb-three:27029",
            "priority": 0.5
        }
    ]
};

printjson(rs.initiate(config, { force: true }));

console.log('Waiting for replica set initiation...')
sleep(15000);

printjson(rs.status());

//"settings": {
//    "chainingAllowed": false
//}