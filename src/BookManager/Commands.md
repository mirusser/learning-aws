# Commands to use

resource: https://codewithmukesh.com/blog/deploy-aspnet-core-web-api-to-amazon-ecs/

```
docker build -t book-manager .
```


```
aws ecr get-login-password | docker login --username AWS --password-stdin ACCOUNT_ID.dkr.ecr.REGION.amazonaws.com
```

```
docker tag book-manager ACCOUNT_ID.dkr.ecr.REGION.amazonaws.com/REGISTRY_NAME
```

```
docker push ACCOUNT_ID.dkr.ecr.REGION.amazonaws.com/REGISTRY_NAME
```

```
docker run -p 8080:80 -e MONGODBCONFIG:CONNECTIONSTRING='mongodb://host.docker.internal:27017' -e ASPNETCORE_ENVIRONMENT=Development book-manager
```