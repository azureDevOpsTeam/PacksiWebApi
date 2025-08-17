# راهنمای راه‌اندازی CI/CD و Deployment

## مرحله 1: ایجاد GitHub Repository

### روش 1: استفاده از GitHub CLI (توصیه شده)
```bash
# ورود به GitHub
gh auth login

# ایجاد repository
gh repo create PacksiWebApp --public --description "PacksiWebApp - A comprehensive web application with CI/CD"

# اضافه کردن remote origin
git remote add origin https://github.com/YOUR_USERNAME/PacksiWebApp.git

# Push کردن کد
git push -u origin main
```

### روش 2: ایجاد دستی از طریق GitHub Website
1. به [GitHub.com](https://github.com) بروید
2. روی "+" کلیک کنید و "New repository" را انتخاب کنید
3. نام repository: `PacksiWebApp`
4. توضیحات: `PacksiWebApp - A comprehensive web application with CI/CD`
5. Public یا Private انتخاب کنید
6. "Create repository" کلیک کنید
7. دستورات زیر را اجرا کنید:

```bash
git remote add origin https://github.com/YOUR_USERNAME/PacksiWebApp.git
git push -u origin main
```

## مرحله 2: تنظیم GitHub Secrets

برای فعال‌سازی CI/CD، باید Secrets زیر را در GitHub تنظیم کنید:

### مسیر تنظیم Secrets:
`Repository → Settings → Secrets and variables → Actions → New repository secret`

### Secrets مورد نیاز:

```bash
# Database
DATABASE_URL=postgresql://username:password@host:port/database
CONNECTION_STRING=Host=your-host;Port=5432;Database=PacksiWebApp;Username=your-user;Password=your-password;

# JWT Settings
JWT_SECRET=your-super-secret-jwt-key-at-least-32-characters
JWT_ISSUER=PacksiWebApp
JWT_AUDIENCE=PacksiWebApp

# Telegram Bot
TELEGRAM_BOT_TOKEN=your-telegram-bot-token

# Docker Registry (اختیاری)
DOCKER_USERNAME=your-docker-username
DOCKER_PASSWORD=your-docker-password

# Deployment Server (برای CD)
SERVER_HOST=your-server-ip
SERVER_USER=your-server-username
SERVER_SSH_KEY=your-private-ssh-key

# Monitoring (اختیاری)
GRAFANA_ADMIN_PASSWORD=your-grafana-password
PROMETHEUS_PASSWORD=your-prometheus-password

# Email Settings (اختیاری)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-email-password
```

## مرحله 3: راه‌اندازی Production Server

### پیش‌نیازهای سرور:
- Ubuntu 20.04+ یا CentOS 8+
- Docker و Docker Compose
- Nginx (اختیاری - در docker-compose موجود است)
- SSL Certificate

### نصب Docker:
```bash
# Ubuntu
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER

# نصب Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/download/v2.20.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
```

### ایجاد فایل .env در سرور:
```bash
# کپی کردن .env.example
cp .env.example .env

# ویرایش تنظیمات
nano .env
```

## مرحله 4: فعال‌سازی CI/CD

### CI Pipeline (خودکار):
هر بار که کد را push می‌کنید، موارد زیر اجرا می‌شود:
- ✅ Build پروژه
- ✅ اجرای تست‌ها
- ✅ Code Coverage
- ✅ Security Scan
- ✅ ساخت Docker Image
- ✅ Upload Artifacts

### CD Pipeline (خودکار):
هر بار که به branch `main` push می‌کنید:
- 🚀 Deploy به Staging Environment
- 🧪 اجرای Integration Tests
- ✅ Health Check
- 🎯 Deploy به Production (در صورت موفقیت)

## مرحله 5: مانیتورینگ و لاگ‌ها

### دسترسی به سرویس‌ها:
```bash
# وب اپلیکیشن
http://your-domain.com

# Grafana (مانیتورینگ)
http://your-domain.com:3000
Username: admin
Password: [GRAFANA_ADMIN_PASSWORD]

# Kibana (لاگ‌ها)
http://your-domain.com:5601

# RabbitMQ Management
http://your-domain.com:15672
Username: admin
Password: [RABBITMQ_PASSWORD]
```

### مشاهده لاگ‌ها:
```bash
# لاگ‌های اپلیکیشن
docker-compose logs -f web

# لاگ‌های دیتابیس
docker-compose logs -f postgres

# تمام لاگ‌ها
docker-compose logs -f
```

## مرحله 6: تست CI/CD

### تست اولیه:
```bash
# ایجاد تغییر کوچک
echo "# Test CI/CD" >> README.md
git add README.md
git commit -m "Test CI/CD pipeline"
git push origin main
```

### بررسی وضعیت:
1. به GitHub Actions بروید: `Repository → Actions`
2. وضعیت CI/CD را مشاهده کنید
3. در صورت خطا، لاگ‌ها را بررسی کنید

## عیب‌یابی رایج

### خطاهای CI:
```bash
# خطای Build
- بررسی dependencies در .csproj
- بررسی syntax errors

# خطای Test
- بررسی connection string تست
- بررسی database migrations

# خطای Docker
- بررسی Dockerfile syntax
- بررسی base image availability
```

### خطاهای CD:
```bash
# خطای SSH
- بررسی SSH key در GitHub Secrets
- بررسی دسترسی‌های سرور

# خطای Docker Compose
- بررسی .env file در سرور
- بررسی port conflicts
- بررسی disk space
```

## امنیت

### نکات امنیتی:
- 🔐 همیشه از HTTPS استفاده کنید
- 🔑 JWT secrets را قوی انتخاب کنید
- 🛡️ Firewall را تنظیم کنید
- 📊 لاگ‌های امنیتی را مانیتور کنید
- 🔄 به‌روزرسانی‌های امنیتی را اعمال کنید

### Firewall تنظیمات:
```bash
# Ubuntu UFW
sudo ufw allow 22/tcp    # SSH
sudo ufw allow 80/tcp    # HTTP
sudo ufw allow 443/tcp   # HTTPS
sudo ufw enable
```

## پشتیبان‌گیری

### پشتیبان‌گیری خودکار:
```bash
# اضافه کردن به crontab
0 2 * * * /path/to/backup-script.sh
```

### اسکریپت پشتیبان‌گیری:
```bash
#!/bin/bash
DATE=$(date +%Y%m%d_%H%M%S)
docker exec postgres pg_dump -U postgres PacksiWebApp > backup_$DATE.sql
aws s3 cp backup_$DATE.sql s3://your-backup-bucket/
```

---

## پشتیبانی

در صورت بروز مشکل:
1. لاگ‌های GitHub Actions را بررسی کنید
2. لاگ‌های سرور را چک کنید
3. مستندات Docker و .NET را مطالعه کنید
4. Issue در GitHub repository ایجاد کنید

**موفق باشید! 🚀**