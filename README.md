# Smashblow - A Simple Web Server to Test its Host

This is a very simple web server that I wrote in order to test its host - k8s was my target.

This server is not doing anything useful in itself, it just allows you to test what happens on its host when it crashes or when it reaches a certain size in memory.

## "Useful" Endpoints

```
# Wait for 10 seconds before returning empty 200
GET /delay/10
```

```
# Shut down gracefully
POST /shutdown
```

```
# Crash
POST /crash
```

```
# Return 201 if 10 seconds have elapsed since the server started, 500 otherwise
GET /elapsed/10
```

```
# Increase the memory consumption to 10 MB (more or less)  then return 201
POST /size/10
```