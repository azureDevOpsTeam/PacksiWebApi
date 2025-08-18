# راه‌اندازی سرور برای CI/CD

## پیش‌نیازهای سرور

### 1. نصب .NET Runtime
```bash
# اضافه کردن Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# نصب .NET 9 Runtime
sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-9.0
```

### 2. ایجاد دایرکتوری‌های مورد نیاز
```bash
sudo mkdir -p /var/www/packsi/publish
sudo mkdir -p /var/www/packsi/backup
sudo chown -R www-data:www-data /var/www/packsi
```

### 3. تنظیم Systemd Service
```bash
# کپی فایل service به systemd
sudo cp packsi-api.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable packsi-api
```

### 4. تنظیم Nginx
فایل تنظیمات Nginx در `/etc/nginx/sites-available/packsi`:

```nginx
server {
    listen 80;
    server_name web.draton.io;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}
```

```bash
# فعال‌سازی سایت
sudo ln -s /etc/nginx/sites-available/packsi /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

## تنظیم GitHub Secrets

در تنظیمات repository خود در GitHub، Secrets زیر را اضافه کنید:

- `SERVER_HOST`: web.draton.io
- `SERVER_USER`: نام کاربری SSH
- `SERVER_SSH_KEY`: کلید خصوصی SSH
- `SERVER_PORT`: پورت SSH (پیش‌فرض: 22)

## نکات مهم

1. مطمئن شوید که کاربر SSH دسترسی sudo دارد
2. فایروال سرور باید پورت‌های 80، 443 و SSH را باز کند
3. پروژه روی پورت 5000 اجرا می‌شود
4. لاگ‌های اپلیکیشن در systemd قابل مشاهده است:
   ```bash
   sudo journalctl -u packsi-api -f
   ```

## عیب‌یابی

### بررسی وضعیت سرویس
```bash
sudo systemctl status packsi-api
```

### مشاهده لاگ‌ها
```bash
sudo journalctl -u packsi-api --since "1 hour ago"
```

### بررسی اتصال
```bash
curl http://localhost:5000
```