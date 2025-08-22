# راهنمای Deploy خودکار

این پروژه از GitHub Actions برای deploy خودکار استفاده می‌کند.

## تنظیمات مورد نیاز

برای فعال‌سازی CI/CD، باید متغیرهای زیر را در GitHub Repository Secrets تنظیم کنید:

### Environment Variables (GitHub Secrets)

1. **SERVER_HOST**: آدرس IP یا دامنه سرور
   ```
   مثال: 192.168.1.100 یا example.com
   ```

2. **SERVER_PORT**: پورت SSH سرور (معمولاً 22)
   ```
   مثال: 22
   ```

3. **SERVER_USER**: نام کاربری برای اتصال SSH
   ```
   مثال: ubuntu یا root
   ```

4. **SERVER_SSH_KEY**: کلید خصوصی SSH برای اتصال به سرور
   ```
   محتوای فایل private key (معمولاً ~/.ssh/id_rsa)
   ```

5. **APP_URL**: آدرس نهایی اپلیکیشن (اختیاری)
   ```
   مثال: https://api.example.com
   ```

## نحوه تنظیم GitHub Secrets

1. به repository خود در GitHub بروید
2. روی **Settings** کلیک کنید
3. از منوی سمت چپ **Secrets and variables** > **Actions** را انتخاب کنید
4. روی **New repository secret** کلیک کنید
5. نام و مقدار secret را وارد کنید

## فرآیند Deploy

هنگامی که کد روی branch `main` یا `master` push شود، فرآیند زیر اجرا می‌شود:

1. **Checkout**: دریافت کد از repository
2. **Setup .NET**: نصب .NET SDK
3. **Restore**: بازیابی وابستگی‌ها
4. **Build**: کامپایل پروژه
5. **Publish**: تولید فایل‌های قابل اجرا
6. **Deploy**: انتقال فایل‌ها به سرور
7. **Service Management**: راه‌اندازی مجدد سرویس

## تنظیمات سرور

قبل از اولین deploy، مطمئن شوید که:

1. پوشه `/var/www/api-app` وجود دارد
2. سرویس `api` در systemd تعریف شده است
3. کاربر SSH دسترسی sudo دارد
4. .NET Runtime روی سرور نصب است

## مثال فایل سرویس systemd

```ini
[Unit]
Description=Packsi Web API
After=network.target

[Service]
Type=notify
ExecStart=/var/www/api-app/PresentationApp
Restart=always
RestartSec=5
KillSignal=SIGINT
SyslogIdentifier=packsi-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
WorkingDirectory=/var/www/api-app

[Install]
WantedBy=multi-user.target
```

این فایل باید در مسیر `/etc/systemd/system/api.service` قرار گیرد.

## عیب‌یابی

اگر deploy با مشکل مواجه شد:

1. لاگ‌های GitHub Actions را بررسی کنید
2. اتصال SSH به سرور را تست کنید
3. وضعیت سرویس را بررسی کنید: `sudo systemctl status api`
4. لاگ‌های سرویس را مشاهده کنید: `sudo journalctl -u api -f`
