data "yandex_compute_image" "t_raider_image" {
  family = "container-optimized-image"
}

resource "yandex_compute_disk" "t_raider_disk" {
  name      = "t-raider-disk"
  folder_id = var.folder_id
  zone      = var.zone
  size      = 50
  type      = "network-hdd"
  image_id  = data.yandex_compute_image.t_raider_image.id
}

resource "yandex_compute_instance" "t_raider_vm" {
  name               = "t-raider-vm"
  folder_id          = var.folder_id
  zone               = var.zone
  platform_id        = "standard-v3"
  service_account_id = var.service_account_id

  resources {
    cores         = 2
    memory        = 4
    core_fraction = 100
  }

  boot_disk {
    disk_id = yandex_compute_disk.t_raider_disk.id
  }

  network_interface {
    subnet_id = var.subnet_id
    nat       = true
  }

  scheduling_policy {
    preemptible = false
  }

  metadata = {
    docker-compose = templatefile("t-raider-vm.docker-compose.yml", {
      postgres_password = var.postgres_password,
      gpt_api_key       = var.gpt_api_key,
      seq_password      = var.seq_password,
      seq_password_hash = var.seq_password_hash,
      seq_api_key       = var.seq_api_key,
      admin_bot_token   = var.admin_bot_token
    })
    user-data = templatefile("t-raider-vm.user-data.yml", {
      ssh_authorized_key = var.ssh_authorized_key
    })
    enable-oslogin = true
  }
}
