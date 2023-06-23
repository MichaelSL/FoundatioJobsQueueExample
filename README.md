# FoundatioJobsQueueExample
Minimal Foundatio Jobs Queue Example.

This example demonstrates content sharing in a tree using recurring HTTP requests and [Foundatio](https://github.com/FoundatioFx/Foundatio) Jobs Queue.

To share content with Jobs Queue use

```
POST http://localhost:5253/content/share/1
```

with Content array in JSON body:

```
[{
  "Id": "10010",
  "Title": "Hello",
  "IsComplete": false
},
{
  "Id": "10020",
  "Title": "Jobs",
  "IsComplete": false
}]
```

To share content with HTTP requests use same body but `PUT` verb:

```
PUT http://localhost:5253/content/share/1

[{
  "Id": "10010",
  "Title": "Hello",
  "IsComplete": false
},
{
  "Id": "10020",
  "Title": "HTTP",
  "IsComplete": false
}]
```

Tenants API allows you to track sharing progress: http://localhost:5253/tenants
