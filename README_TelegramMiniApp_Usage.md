# Telegram MiniApp Integration - Usage Guide

This guide shows how to integrate Telegram MiniApp validation with the existing MiniApp controller.

## تغییرات انجام شده

### 1. CreateRequestTMADto.cs
- حذف property `InitData` از request body
- حالا `initData` باید در header ارسال شود

### 2. IMiniAppServices.cs
- تغییر return type متدها از `ServiceResult` به `Result Pattern`
- اضافه شدن `botToken` به عنوان parameter در `ValidateTelegramMiniAppUserAsync`

### 3. MiniAppServices.cs
- پیاده‌سازی `Result Pattern` در تمام متدها
- بهبود error handling و استفاده از typed results
- حذف وابستگی به `ServiceResult`

### 4. CreateRequestTMAHandler.cs
- اضافه شدن `IHttpContextAccessor` به constructor
- خواندن `initData` از header `X-Telegram-Init-Data`
- اضافه شدن `IConfiguration` برای خواندن `botToken`
- اضافه شدن `ILogger` برای logging
- بهبود error handling و validation
- استفاده از `Result Pattern` جدید

### 5. UserValicationHandler.cs
- به‌روزرسانی برای سازگاری با `Result Pattern`
- اضافه شدن `IConfiguration` و `ILogger`
- بهبود error handling

## Client-Side Usage Example

### JavaScript Example (Sending Request with initData in Header)

```javascript
// Example 1: Sending request without file
const sendRequestWithoutFile = async () => {
    try {
        // Get initData from Telegram WebApp
        const initData = window.Telegram?.WebApp?.initData || '';
        
        const requestData = {
            originCityId: 1,
            destinationCityId: 2,
            // ... other fields
        };

        const response = await fetch('/api/MiniApp/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-Telegram-Init-Data': initData  // Send initData in header
            },
            body: JSON.stringify(requestData)
        });

        const result = await response.json();
        console.log('Success:', result);
    } catch (error) {
        console.error('Error:', error);
    }
};

// Example 2: Sending request with file
const sendRequestWithFile = async (file) => {
    try {
        // Get initData from Telegram WebApp
        const initData = window.Telegram?.WebApp?.initData || '';
        
        const formData = new FormData();
        formData.append('file', file);
        formData.append('originCityId', '1');
        formData.append('destinationCityId', '2');
        // ... append other fields

        const response = await fetch('/api/MiniApp/Create', {
            method: 'POST',
            headers: {
                'X-Telegram-Init-Data': initData  // Send initData in header
            },
            body: formData  // No Content-Type header needed for FormData
        });

        const result = await response.json();
        console.log('Success:', result);
    } catch (error) {
        console.error('Error:', error);
    }
};
```

### React/TypeScript Example

```typescript
interface TelegramWebApp {
    initData: string;
    // ... other Telegram WebApp properties
}

declare global {
    interface Window {
        Telegram?: {
            WebApp?: TelegramWebApp;
        };
    }
}

const createRequest = async (requestData: any) => {
    const initData = window.Telegram?.WebApp?.initData;
    
    if (!initData) {
        throw new Error('Telegram initData not available');
    }

    const response = await fetch('/api/MiniApp/Create', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-Telegram-Init-Data': initData
        },
        body: JSON.stringify(requestData)
    });

    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.json();
};
```

## Required Configuration

### appsettings.json
```json
{
  "Telegram": {
    "BotToken": "YOUR_TELEGRAM_BOT_TOKEN_HERE"
  }
}
```

### appsettings.Development.json
```json
{
  "Telegram": {
    "BotToken": "YOUR_DEVELOPMENT_BOT_TOKEN_HERE"
  }
}
```

## Error Messages

The handler can return the following error messages:

1. **Missing initData**: "داده‌های اعتبارسنجی Telegram در header الزامی است (X-Telegram-Init-Data)"
2. **Missing Bot Token**: "خطای پیکربندی سرور"
3. **Validation Failed**: Various messages from the validation service

## Important Notes

### About initData
- `initData` is automatically provided by Telegram WebApp when your mini app is opened
- It contains user information and authentication data
- It must be sent in the `X-Telegram-Init-Data` header for security reasons
- The data is URL-encoded and contains parameters like `user`, `auth_date`, `hash`, etc.

### Security
- The handler validates the `initData` using HMAC-SHA256 signature verification
- Only requests from legitimate Telegram users will be accepted
- The bot token is securely stored in configuration and not exposed to clients

### Testing
- For testing outside of Telegram, you can manually set the header with valid initData
- Use Telegram's test environment for development

## Integration Flow

1. User opens your Telegram MiniApp
2. Telegram provides `initData` via `window.Telegram.WebApp.initData`
3. Client sends request to `/api/MiniApp/Create` with `initData` in `X-Telegram-Init-Data` header
4. Handler extracts `initData` from header
5. Handler validates user using Telegram MiniApp validation service
6. If validation succeeds, the request is processed normally
7. If validation fails, an error response is returned

This approach ensures that only legitimate Telegram users can access your MiniApp endpoints while maintaining clean separation between authentication data (header) and business data (body).