#!/bin/bash

kubectl apply \
    -f ./secrets.yaml \
    -f ./sqldata.yaml \
    -f ./rabbitmq.yaml \
    -f ./apigateway.yaml \
    -f ./components/pubsub-rabbitmq.yaml \
    -f ./facesapi.yaml \
    -f ./ordersapi.yaml \
    -f ./faces.webmvc.yaml \
    -f ./notificationservice.yaml
