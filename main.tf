provider "aws" {
  region = "us-west-1"
}

# 1. RDS PostgreSQL
resource "aws_db_instance" "postgres" {
  identifier          = "parts-db"
  engine              = "postgres"
  instance_class      = "db.t3.micro"
  allocated_storage   = 10
  username            = var.db_username
  password            = var.db_password
  skip_final_snapshot = true

}

# 2. ECS Task Definition for Fargate
resource "aws_ecs_task_definition" "app" {
  family                   = "parts-tracker-task"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = "256"
  memory                   = "512"

  container_definitions = jsonencode([
    {
      name      = "parts-api"
      image     = var.api_image
      essential = true
      portMappings = [
        {
          containerPort = 80
          hostPort      = 80
          protocol      = "tcp"
        }
      ]
    }
  ])
}