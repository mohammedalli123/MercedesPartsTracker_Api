variable "db_password" {
  type      = string
  sensitive = true
}

variable "db_username" {
  type      = string
  sensitive = true
}

variable "api_image" {
  type      = string
  sensitive = false
}
