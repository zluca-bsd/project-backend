# project-backend
Example appsettings.json:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  },
  "JwtSettings": {
    "Secret": "jgH0&//<L>8,XH-)4|j7`U,'ola@.p9MlXpY>#c+783?V!f1t7",
    "Issuer": "testdomain.com",
    "Audience": "testdomain.com"
  },
  "AiSettings": {
    "Model": "qwen/qwen3-4b-2507",
    "Temperature": 0,
    "MaxTokens": -1,
    "Stream": false,
    "Endpoint": "http://localhost:1234/v1/completions"
  }
}
```
