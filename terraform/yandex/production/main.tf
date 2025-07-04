provider "yandex" {
  cloud_id                 = var.cloud_id
  folder_id                = var.folder_id
  service_account_key_file = "authorized_key.json"
}

terraform {
  backend "s3" {
    endpoints = {
      s3 = "https://storage.yandexcloud.net"
    }

    endpoint   = "https://storage.yandexcloud.net" 
    bucket     = "t-raider"
    region     = "ru-central1"
    key        = "t-raider-production/state.tfstate"

    skip_region_validation      = true
    skip_credentials_validation = true
    skip_requesting_account_id  = true
    skip_s3_checksum            = true
  }
}
