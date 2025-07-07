# MercedesPartsTracker_Api

An ASP.NET Core 8 Web API for managing Mercedes car parts inventory, bundled with infrastructure as code (Terraform), ,Dockerfile, and Docker Compose file.

---

## Included in This Repo

- **MercedesPartsTracker API** – ASP.NET Core REST API
- **Terraform Files** – Provision AWS RDS PostgreSQL & ECS Fargate (plan-only)
- **Dockerfile** – Build and package the API into a container
- **docker-compose.yml** – Spin up the PostgreSQL DB and API containers

---

## Setup Instructions

### Running Locally (Dev)

**Prerequisite**:
Uncomment out the first part of the docker-compose.yaml file and comment out the send part. This will spin up the Postgres DB locally.

1. **Clone the repository**
   ```bash
   git clone https://github.com/mohammedalli123/MercedesPartsTracker_Api.git
   cd MercedesPartsTracker_Api

2. **Run the solution**  
Run within Visual Studio
    
### Running as a container (Docker)  
1. **Run the below  in the soltuion root to create a docker image for the API and and spin up both it's container as well as Postgres**  
    ```bash
    docker compose up --build  


```mermaid
    graph TD
    A[Controller] --> B[Service]
    B --> C[PostgreSQL DB]  

