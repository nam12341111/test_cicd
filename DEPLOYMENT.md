# Hướng Dẫn Triển Khai Part-Time Job API

## 📋 Mục Lục
1. [Yêu Cầu Hệ Thống](#yêu-cầu-hệ-thống)
2. [Triển Khai Local với Docker Compose](#triển-khai-local-với-docker-compose)
3. [Build Docker Image Thủ Công](#build-docker-image-thủ-công)
4. [CI/CD với GitHub Actions](#cicd-với-github-actions)
5. [Triển Khai Production](#triển-khai-production)
6. [Biến Môi Trường](#biến-môi-trường)
7. [Troubleshooting](#troubleshooting)

---

## 🔧 Yêu Cầu Hệ Thống

### Phát Triển Local
- Docker Desktop 20.10+
- Docker Compose 2.0+
- .NET 9.0 SDK (nếu build không dùng Docker)
- Git

### Production
- Docker Engine 20.10+
- Docker Compose 2.0+ hoặc Kubernetes
- Minimum 2GB RAM
- 10GB disk space

---

## 🚀 Triển Khai Local với Docker Compose

### Bước 1: Clone Repository
```bash
git clone <your-repository-url>
cd server
```

### Bước 2: Cấu Hình Môi Trường
Tạo file `.env` trong thư mục root (tùy chọn):
```env
SA_PASSWORD=YourStrong@Password123
JWT_SECRET_KEY=YourSuperSecretKeyForJWTTokenGenerationMinimum32CharactersLong
```

### Bước 3: Khởi Động Các Services
```bash
# Khởi động tất cả services (API + SQL Server)
docker-compose up -d

# Xem logs
docker-compose logs -f api

# Kiểm tra trạng thái
docker-compose ps
```

### Bước 4: Truy Cập API
- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433 (User: sa, Password: YourStrong@Password123)

### Bước 5: Dừng Services
```bash
# Dừng services
docker-compose down

# Dừng và xóa volumes (database data sẽ bị xóa)
docker-compose down -v
```

---

## 🐳 Build Docker Image Thủ Công

### Build Image
```bash
# Build image
docker build -t ptj-api:latest .

# Build với specific tag
docker build -t ptj-api:v1.0.0 .
```

### Run Container Đơn Lẻ
```bash
# Run API container (cần SQL Server riêng)
docker run -d \
  --name ptj-api \
  -p 5000:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__Default="Server=your-sql-server;Database=PartTimeJobs;User Id=sa;Password=YourPassword;TrustServerCertificate=True" \
  -v ptj-uploads:/app/Uploads \
  ptj-api:latest
```

### Kiểm Tra Container
```bash
# Xem logs
docker logs ptj-api

# Truy cập vào container
docker exec -it ptj-api bash

# Kiểm tra health
docker inspect ptj-api
```

---

## 🔄 CI/CD với GitHub Actions

### Cấu Hình Workflow

Workflow được cấu hình tự động chạy khi:
- Push code lên branch `main` hoặc `develop`
- Tạo Pull Request vào `main` hoặc `develop`

### Các Bước Trong Pipeline

1. **Build and Test**
   - Restore dependencies
   - Build solution
   - Run unit tests
   - Publish artifacts

2. **Docker Build & Push**
   - Build Docker image
   - Push lên GitHub Container Registry (GHCR)
   - Tag với commit SHA và branch name

3. **Security Scan**
   - Quét vulnerabilities với Trivy
   - Upload kết quả lên GitHub Security

### Sử Dụng Docker Images từ GHCR

```bash
# Login vào GitHub Container Registry
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin

# Pull image
docker pull ghcr.io/YOUR-USERNAME/server:main

# Run image
docker run -d -p 5000:8080 ghcr.io/YOUR-USERNAME/server:main
```

### Cấu Hình GitHub Secrets

Không cần thêm secrets cho workflow cơ bản. Nếu deploy lên cloud platforms, thêm:
- `DOCKER_USERNAME`: Docker Hub username (nếu dùng Docker Hub)
- `DOCKER_PASSWORD`: Docker Hub password
- `AZURE_CREDENTIALS`: Azure service principal (nếu deploy lên Azure)
- `AWS_ACCESS_KEY_ID`: AWS access key (nếu deploy lên AWS)
- `AWS_SECRET_ACCESS_KEY`: AWS secret key

---

## 🌐 Triển Khai Production

### Option 1: Docker Compose (VPS/VM)

1. **SSH vào server**
```bash
ssh user@your-server-ip
```

2. **Clone repository và setup**
```bash
git clone <your-repo>
cd server

# Tạo file .env với production values
nano .env
```

3. **Chỉnh sửa docker-compose.yml cho production**
```yaml
services:
  api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__Default=Server=sqlserver;Database=PartTimeJobs;User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True
      - JWT__Key=${JWT_SECRET_KEY}
    restart: always
```

4. **Khởi động services**
```bash
docker-compose -f docker-compose.yml up -d
```

5. **Setup reverse proxy (Nginx)**
```bash
# Install Nginx
sudo apt install nginx

# Configure Nginx
sudo nano /etc/nginx/sites-available/ptj-api
```

Nginx config:
```nginx
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### Option 2: Azure Container Instances

```bash
# Login to Azure
az login

# Create resource group
az group create --name ptj-rg --location eastus

# Create container
az container create \
  --resource-group ptj-rg \
  --name ptj-api \
  --image ghcr.io/YOUR-USERNAME/server:main \
  --dns-name-label ptj-api \
  --ports 8080 \
  --environment-variables \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__Default="<your-connection-string>"
```

### Option 3: AWS ECS/Fargate

```bash
# Push image to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin <account-id>.dkr.ecr.us-east-1.amazonaws.com

docker tag ptj-api:latest <account-id>.dkr.ecr.us-east-1.amazonaws.com/ptj-api:latest
docker push <account-id>.dkr.ecr.us-east-1.amazonaws.com/ptj-api:latest

# Deploy với ECS (sử dụng AWS Console hoặc Terraform)
```

### Option 4: Kubernetes

```yaml
# deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: ptj-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: ptj-api
  template:
    metadata:
      labels:
        app: ptj-api
    spec:
      containers:
      - name: api
        image: ghcr.io/YOUR-USERNAME/server:main
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__Default
          valueFrom:
            secretKeyRef:
              name: ptj-secrets
              key: connection-string
```

Deploy:
```bash
kubectl apply -f deployment.yaml
kubectl apply -f service.yaml
```

---

## 🔐 Biến Môi Trường

### Required Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Production`, `Development` |
| `ASPNETCORE_URLS` | URLs to listen on | `http://+:8080` |
| `ConnectionStrings__Default` | Database connection | `Server=sqlserver;Database=PartTimeJobs;...` |
| `JWT__Key` | JWT secret key (min 32 chars) | `YourSuperSecretKey...` |
| `JWT__Issuer` | JWT issuer | `PTJ.API` |
| `JWT__Audience` | JWT audience | `PTJ.Client` |

### Optional Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `JWT__AccessTokenExpirationMinutes` | Access token lifetime | `60` |
| `JWT__RefreshTokenExpirationDays` | Refresh token lifetime | `7` |
| `FileStorage__UploadPath` | Upload directory | `/app/Uploads` |
| `FileStorage__MaxFileSize` | Max file size (bytes) | `10485760` (10MB) |

### Cách Thiết Lập

**Docker Compose:**
```yaml
environment:
  - ConnectionStrings__Default=${DB_CONNECTION}
  - JWT__Key=${JWT_SECRET}
```

**Docker Run:**
```bash
docker run -e ConnectionStrings__Default="..." -e JWT__Key="..." ptj-api
```

**Kubernetes:**
```yaml
env:
  - name: JWT__Key
    valueFrom:
      secretKeyRef:
        name: ptj-secrets
        key: jwt-key
```

---

## 🐛 Troubleshooting

### API không kết nối được SQL Server

**Triệu chứng:**
```
Connection refused or timeout
```

**Giải pháp:**
1. Kiểm tra SQL Server đã khởi động:
```bash
docker-compose ps
docker logs ptj-sqlserver
```

2. Kiểm tra connection string đúng format
3. Đảm bảo network được tạo:
```bash
docker network ls
docker network inspect server_ptj-network
```

### Migration không chạy tự động

**Giải pháp:**
```bash
# Vào container và chạy migration
docker exec -it ptj-api bash
dotnet ef database update
```

Hoặc thêm vào Program.cs:
```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
```

### File uploads không persist

**Triệu chứng:**
Files bị mất khi restart container

**Giải pháp:**
Đảm bảo volume được mount:
```yaml
volumes:
  - uploads-data:/app/Uploads
```

### Port đã được sử dụng

**Triệu chứng:**
```
Error: bind: address already in use
```

**Giải pháp:**
```bash
# Tìm process đang dùng port
netstat -ano | findstr :5000  # Windows
lsof -i :5000                 # Linux/Mac

# Đổi port trong docker-compose.yml
ports:
  - "5001:8080"
```

### Performance Issues

**Giải pháp:**
1. Tăng resources cho container:
```yaml
deploy:
  resources:
    limits:
      cpus: '2'
      memory: 2G
```

2. Enable caching
3. Optimize database queries
4. Add Redis for caching

### Container exit ngay lập tức

**Kiểm tra logs:**
```bash
docker logs ptj-api
docker-compose logs api
```

**Nguyên nhân thường gặp:**
- Connection string sai
- Environment variables thiếu
- Migration lỗi
- Port conflict

---

## 📊 Monitoring & Logging

### Xem Logs
```bash
# Docker Compose
docker-compose logs -f api

# Single container
docker logs -f ptj-api

# Last 100 lines
docker logs --tail 100 ptj-api
```

### Health Checks
```bash
# Check API health
curl http://localhost:5000/health

# Check container health
docker inspect --format='{{.State.Health.Status}}' ptj-api
```

### Production Monitoring
Khuyến nghị sử dụng:
- **Application Insights** (Azure)
- **CloudWatch** (AWS)
- **Prometheus + Grafana**
- **ELK Stack** (Elasticsearch, Logstash, Kibana)

---

## 🔒 Security Best Practices

1. **Không hard-code secrets** trong source code
2. **Sử dụng environment variables** cho sensitive data
3. **Thường xuyên update** base images
4. **Scan vulnerabilities** với Trivy/Snyk
5. **Sử dụng HTTPS** trong production
6. **Limit container permissions**
7. **Network segmentation** với Docker networks
8. **Regular backups** cho database

---

## 📚 Tài Liệu Tham Khảo

- [Docker Documentation](https://docs.docker.com/)
- [ASP.NET Core Docker](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)
- [GitHub Actions](https://docs.github.com/en/actions)
- [Docker Compose](https://docs.docker.com/compose/)
- [.NET 9 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9)

---

## 🤝 Support

Nếu gặp vấn đề, vui lòng:
1. Kiểm tra [Troubleshooting](#troubleshooting) section
2. Xem logs: `docker-compose logs -f`
3. Tạo issue trên GitHub repository
