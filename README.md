# PacksiWebApp 📦

A comprehensive web application built with ASP.NET Core for package delivery and logistics management, featuring Telegram Mini App integration.

## 🚀 Features

- **User Management**: Complete user registration, authentication, and profile management
- **Request Management**: Create, track, and manage delivery requests
- **Telegram Integration**: Telegram Mini App support with secure authentication
- **Real-time Updates**: Live tracking and notifications
- **Admin Dashboard**: Comprehensive management interface
- **Multi-language Support**: Persian and English language support
- **Secure API**: JWT-based authentication with refresh tokens
- **File Upload**: Support for request attachments and user avatars

## 🏗️ Architecture

The application follows Clean Architecture principles with the following layers:

```
├── PresentationApp/          # Web API Controllers
├── ApplicationLayer/         # Business Logic & CQRS
│   ├── CQRS/                # Commands & Queries
│   ├── BusinessLogic/       # Services & Interfaces
│   ├── DTOs/                # Data Transfer Objects
│   └── Extensions/          # Helper Extensions
├── DomainLayer/             # Domain Entities & Business Rules
└── InfrastructureLayer/     # Data Access & External Services
```

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0
- **Database**: PostgreSQL with Entity Framework Core
- **Caching**: Redis
- **Authentication**: JWT with Refresh Tokens
- **Message Queue**: RabbitMQ
- **Monitoring**: Prometheus + Grafana
- **Logging**: Elasticsearch + Kibana
- **Containerization**: Docker & Docker Compose
- **CI/CD**: GitHub Actions

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (for containerized deployment)
- [PostgreSQL](https://www.postgresql.org/download/) (for local development)
- [Redis](https://redis.io/download) (for caching)

## 🚀 Quick Start

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/PacksiWebApp.git
   cd PacksiWebApp
   ```

2. **Setup Database**
   ```bash
   # Create PostgreSQL database
   createdb PacksiWebApp
   ```

3. **Configure Application Settings**
   ```bash
   # Copy and modify appsettings
   cp PresentationApp/appsettings.json PresentationApp/appsettings.Development.json
   ```

   Update connection strings and other settings in `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=PacksiWebApp;Username=your_user;Password=your_password;"
     },
     "JWT": {
       "SecretKey": "your-super-secret-key-here",
       "Issuer": "PacksiWebApp",
       "Audience": "PacksiWebApp",
       "ExpiryInMinutes": 60
     },
     "Telegram": {
       "BotToken": "your-telegram-bot-token",
       "WebhookUrl": "https://your-domain.com/api/telegram/webhook"
     }
   }
   ```

4. **Run Database Migrations**
   ```bash
   dotnet ef database update --project InfrastructureLayer --startup-project PresentationApp
   ```

5. **Start the Application**
   ```bash
   dotnet run --project PresentationApp
   ```

   The API will be available at `https://localhost:7000` and `http://localhost:5000`

### Docker Deployment

1. **Environment Setup**
   ```bash
   # Create .env file
   cp .env.example .env
   ```

   Configure environment variables in `.env`:
   ```env
   JWT_SECRET_KEY=your-super-secret-jwt-key
   TELEGRAM_BOT_TOKEN=your-telegram-bot-token
   TELEGRAM_WEBHOOK_URL=https://your-domain.com/api/telegram/webhook
   REDIS_PASSWORD=your-redis-password
   GRAFANA_PASSWORD=your-grafana-password
   RABBITMQ_PASSWORD=your-rabbitmq-password
   ```

2. **Start Services**
   ```bash
   # Start all services
   docker-compose up -d
   
   # View logs
   docker-compose logs -f web
   
   # Stop services
   docker-compose down
   ```

3. **Access Services**
   - **Web API**: http://localhost:8080
   - **Database**: localhost:5432
   - **Redis**: localhost:6379
   - **Grafana**: http://localhost:3000 (admin/your-password)
   - **Prometheus**: http://localhost:9090
   - **Kibana**: http://localhost:5601
   - **RabbitMQ Management**: http://localhost:15672

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test ApplicationLayer.Tests
```

## 📚 API Documentation

Once the application is running, you can access:

- **Swagger UI**: `https://localhost:7000/swagger`
- **API Endpoints**: `https://localhost:7000/api`

### Key Endpoints

- `POST /api/identity/register` - User registration
- `POST /api/identity/login` - User login
- `GET /api/requests` - Get user requests
- `POST /api/requests` - Create new request
- `POST /api/miniapp/validate` - Telegram Mini App validation

## 🔧 Configuration

### Database Configuration

The application uses Entity Framework Core with PostgreSQL. Connection strings are configured in `appsettings.json`.

### Telegram Mini App Setup

1. Create a Telegram Bot via [@BotFather](https://t.me/botfather)
2. Set up Mini App in Telegram
3. Configure bot token in application settings
4. Set webhook URL for receiving updates

### JWT Configuration

Configure JWT settings in `appsettings.json`:

```json
{
  "JWT": {
    "SecretKey": "your-256-bit-secret-key",
    "Issuer": "PacksiWebApp",
    "Audience": "PacksiWebApp",
    "ExpiryInMinutes": 60
  }
}
```

## 🚀 Deployment

### GitHub Actions CI/CD

The project includes automated CI/CD pipelines:

- **CI Pipeline** (`.github/workflows/ci.yml`):
  - Builds and tests the application
  - Runs security scans
  - Creates Docker images
  - Uploads artifacts

- **CD Pipeline** (`.github/workflows/cd.yml`):
  - Deploys to staging environment
  - Runs integration tests
  - Deploys to production (on release)
  - Includes rollback capabilities

### Manual Deployment

1. **Build Docker Image**
   ```bash
   docker build -t packsi-web-app:latest .
   ```

2. **Push to Registry**
   ```bash
   docker tag packsi-web-app:latest your-registry/packsi-web-app:latest
   docker push your-registry/packsi-web-app:latest
   ```

3. **Deploy with Docker Compose**
   ```bash
   docker-compose -f docker-compose.prod.yml up -d
   ```

## 📊 Monitoring

The application includes comprehensive monitoring:

- **Application Metrics**: Prometheus + Grafana
- **Logging**: Elasticsearch + Kibana
- **Health Checks**: Built-in ASP.NET Core health checks
- **Performance**: Application Insights integration

## 🔒 Security

- JWT-based authentication with refresh tokens
- Input validation and sanitization
- SQL injection prevention via Entity Framework
- CORS configuration
- Rate limiting
- Security headers

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines

- Follow Clean Architecture principles
- Write unit tests for new features
- Use meaningful commit messages
- Update documentation as needed
- Follow C# coding conventions

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For support and questions:

- Create an issue on GitHub
- Contact the development team
- Check the documentation

## 🙏 Acknowledgments

- ASP.NET Core team for the excellent framework
- Entity Framework Core for data access
- Telegram for Mini App platform
- All contributors and supporters

---

**Made with ❤️ by the Packsi Team**