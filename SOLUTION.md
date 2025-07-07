# MercedesPartsTracker_Api

## Architecture Overview

The design of the `MercedesPartsTracker_Api` showcases **modularity**, **maintainability**, and **testability**.

### Layers

##### Web API Controller
Handles incoming HTTP requests, performs initial validation, and delegates business operations to the Service Layer.

#### Business Logic Layer
This was omited due to time contraints but should be implemented to handle any business rule requirements.

##### Service Layer
Utilizes Entity Framework Core to interact with the database. It maps C# objects (Entities) to database tables and handles CRUD operations.

#### PostgreSQL Database
Stores the application's data.

### Diagram (Mermaid)


```mermaid
    graph TD
    A[Controller] --> B[Service]
    B  
````

## Trade-offs

### EF Core
EF Core makes DB access simple, however sometimes struggles with complex apps which require complex queries. 

### Fargate *
Fargate is fully managed but can work out more expensive than deploying on EC2 or EKS

## Security & Monitoring Notes
### Security
**Connection Strings:** Should be secured using AWS Secrets Manager in production.

**Database:** Username and password should be secured using AWS Secrets Manager in production.

**CORS:** This should be changed from Allow All to restricted in prod.

### Monitoring
**Logs:** Logs should be implemented and should be viewed by tools like CloudWatch.

**Alerting tools:** Grafana/Prometheus or AWS X-Ray can be used for metric insights. Tools can also be used which which proactivly alerts devs of any issues or potential issue by using metrics.

## Cost strategies
Setup up scalling strategies for the application.
