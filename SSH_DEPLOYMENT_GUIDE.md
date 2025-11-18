# 🔐 SSH Deployment Guide - CI/CD tự động deploy

Hướng dẫn setup CI/CD với SSH key để tự động deploy lên production server.

---

## 📋 Mục lục

1. [Tổng quan](#tổng-quan)
2. [Chuẩn bị Server](#chuẩn-bị-server)
3. [Tạo SSH Key](#tạo-ssh-key)
4. [Cấu hình GitHub Secrets](#cấu-hình-github-secrets)
5. [Setup Server](#setup-server)
6. [Test Deployment](#test-deployment)
7. [Troubleshooting](#troubleshooting)

---

## 🎯 Tổng quan

### Workflow CI/CD với SSH:

```
Developer Push Code
       ↓
GitHub Actions Triggered
       ↓
Build & Test (Job 1)
       ↓
Build Docker Image (Job 2)
       ↓
Push to GHCR
       ↓
SSH vào Production Server (Job 3)
       ↓
Pull Docker Image
       ↓
docker-compose up -d
       ↓
✅ Deployment Complete
```

### Yêu cầu:

- ✅ Production server (Linux VPS, Ubuntu khuyến nghị)
- ✅ Docker & Docker Compose đã cài trên server
- ✅ SSH access vào server
- ✅ GitHub repository

---

## 🖥️ Chuẩn bị Server

### 1. SSH vào server của bạn:

```bash
ssh user@your-server-ip
```

### 2. Cài Docker & Docker Compose (nếu chưa có):

```bash
# Update packages
sudo apt update

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Add user to docker group (không cần sudo mỗi lần)
sudo usermod -aG docker $USER

# Logout và login lại để apply changes
exit
# SSH lại vào server

# Verify Docker installation
docker --version
docker-compose --version
```

### 3. Tạo app directory:

```bash
mkdir -p ~/app
cd ~/app
```

### 4. Tạo docker-compose.yml trên server:

```bash
nano ~/app/docker-compose.yml
```

Paste nội dung sau:

```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: ptj-sqlserver
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Password123
      - MSSQL_PID=Express
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql
    networks:
      - ptj-network
    restart: always

  api:
    image: ghcr.io/nam12341111/test_cicd:latest
    container_name: ptj-api
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__Default=Server=sqlserver;Database=PartTimeJobs;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;MultipleActiveResultSets=true
      - JWT__Key=YourSuperSecretKeyForJWTTokenGenerationMinimum32CharactersLong
      - JWT__Issuer=PTJ.API
      - JWT__Audience=PTJ.Client
    ports:
      - "5000:8080"
    volumes:
      - uploads-data:/app/Uploads
    networks:
      - ptj-network
    depends_on:
      - sqlserver
    restart: always

volumes:
  sqlserver-data:
  uploads-data:

networks:
  ptj-network:
```

**Lưu file:** `Ctrl+O`, Enter, `Ctrl+X`

---

## 🔑 Tạo SSH Key

### Option 1: Tạo SSH key trên máy local (Windows)

#### **PowerShell:**

```powershell
# Generate SSH key pair
ssh-keygen -t rsa -b 4096 -C "github-actions-deploy" -f $HOME\.ssh\github_deploy

# Tạo 2 files:
# - github_deploy (private key)
# - github_deploy.pub (public key)
```

#### **View keys:**

```powershell
# Private key (dùng cho GitHub Secrets)
Get-Content $HOME\.ssh\github_deploy

# Public key (dùng cho server)
Get-Content $HOME\.ssh\github_deploy.pub
```

---

### Option 2: Tạo SSH key trên Linux/Mac

```bash
# Generate SSH key pair
ssh-keygen -t rsa -b 4096 -C "github-actions-deploy" -f ~/.ssh/github_deploy

# View private key (dùng cho GitHub Secrets)
cat ~/.ssh/github_deploy

# View public key (dùng cho server)
cat ~/.ssh/github_deploy.pub
```

---

## 📤 Copy Public Key lên Server

### Method 1: ssh-copy-id (Linux/Mac)

```bash
ssh-copy-id -i ~/.ssh/github_deploy.pub user@your-server-ip
```

### Method 2: Manual copy (Windows)

```powershell
# Copy public key content
Get-Content $HOME\.ssh\github_deploy.pub | Set-Clipboard

# SSH vào server
ssh user@your-server-ip

# Paste vào authorized_keys
mkdir -p ~/.ssh
nano ~/.ssh/authorized_keys
# Paste public key vào file này (Ctrl+Shift+V)
# Save: Ctrl+O, Enter, Ctrl+X

# Set permissions
chmod 700 ~/.ssh
chmod 600 ~/.ssh/authorized_keys
```

---

## 🔐 Cấu hình GitHub Secrets

### 1. Vào GitHub Repository:

```
https://github.com/nam12341111/test_cicd/settings/secrets/actions
```

### 2. Click "New repository secret"

### 3. Thêm các secrets sau:

| Secret Name | Value | Mô tả |
|------------|-------|-------|
| `SSH_HOST` | `your-server-ip` | IP hoặc domain của server |
| `SSH_USERNAME` | `your-username` | Username SSH (vd: ubuntu, root) |
| `SSH_PORT` | `22` | SSH port (thường là 22) |
| `SSH_PRIVATE_KEY` | `[nội dung private key]` | Toàn bộ nội dung file github_deploy |

#### **Cách lấy SSH_PRIVATE_KEY:**

**Windows PowerShell:**
```powershell
Get-Content $HOME\.ssh\github_deploy | clip
# Đã copy vào clipboard, paste vào GitHub Secret
```

**Linux/Mac:**
```bash
cat ~/.ssh/github_deploy
# Copy output, paste vào GitHub Secret
```

**Format đúng của private key:**
```
-----BEGIN OPENSSH PRIVATE KEY-----
b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAACFwAAAA
... (nhiều dòng)
... (nhiều dòng)
-----END OPENSSH PRIVATE KEY-----
```

### 4. Verify secrets:

Sau khi thêm xong, bạn sẽ thấy 4 secrets:
- ✅ SSH_HOST
- ✅ SSH_USERNAME
- ✅ SSH_PORT
- ✅ SSH_PRIVATE_KEY

---

## ⚙️ Setup Server

### 1. SSH vào server:

```bash
ssh user@your-server-ip
```

### 2. Copy deployment script lên server:

```bash
cd ~/app
nano deploy.sh
```

Paste nội dung từ file `deploy.sh` (hoặc download từ repo)

```bash
# Make executable
chmod +x deploy.sh
```

### 3. Login vào GitHub Container Registry trên server:

```bash
# Tạo GitHub Personal Access Token:
# https://github.com/settings/tokens
# Permissions: read:packages

# Login
echo YOUR_GITHUB_TOKEN | docker login ghcr.io -u YOUR_GITHUB_USERNAME --password-stdin
```

### 4. Test pull image thủ công:

```bash
docker pull ghcr.io/nam12341111/test_cicd:latest
```

---

## 🧪 Test Deployment

### Method 1: Manual test trên server

```bash
cd ~/app
./deploy.sh
```

**Kết quả mong đợi:**
```
✅ Docker is running
✅ Image pulled successfully
✅ Containers started successfully
✅ Health check passed!
✅ Deployment completed successfully!
```

---

### Method 2: Test qua GitHub Actions

#### **A. Push code để trigger workflow:**

```bash
# Local machine
git add .
git commit -m "Test SSH deployment"
git push origin main
```

#### **B. Xem workflow execution:**

```
https://github.com/nam12341111/test_cicd/actions
```

**Jobs sẽ chạy:**
1. ✅ Build and Test
2. ✅ Build and Push Docker Image
3. ✅ Deploy to Production Server (SSH)

#### **C. Check logs trên server:**

```bash
# SSH vào server
ssh user@your-server-ip

# Xem logs
cd ~/app
docker-compose logs -f api
```

---

## 🔍 Verify Deployment

### 1. Check containers đang chạy:

```bash
docker-compose ps
```

**Output:**
```
NAME              IMAGE                                    STATUS
ptj-api           ghcr.io/nam12341111/test_cicd:latest    Up
ptj-sqlserver     mcr.microsoft.com/mssql/server:2022     Up
```

### 2. Test API:

```bash
# Health check
curl http://localhost:5000/health

# Swagger UI
curl http://localhost:5000/swagger/index.html
```

### 3. Test từ browser:

```
http://YOUR_SERVER_IP:5000/swagger
```

---

## 🔥 Troubleshooting

### Problem 1: SSH connection refused

**Error:**
```
ssh: connect to host x.x.x.x port 22: Connection refused
```

**Solutions:**
```bash
# Check if SSH service is running
sudo systemctl status sshd

# Start SSH service
sudo systemctl start sshd

# Enable SSH on boot
sudo systemctl enable sshd

# Check firewall
sudo ufw status
sudo ufw allow 22/tcp
```

---

### Problem 2: Permission denied (publickey)

**Error:**
```
Permission denied (publickey)
```

**Solutions:**

1. **Check public key on server:**
```bash
cat ~/.ssh/authorized_keys
# Should contain your public key
```

2. **Check permissions:**
```bash
chmod 700 ~/.ssh
chmod 600 ~/.ssh/authorized_keys
```

3. **Test SSH key:**
```bash
# From local machine
ssh -i ~/.ssh/github_deploy user@server-ip
```

---

### Problem 3: Docker permission denied

**Error:**
```
Got permission denied while trying to connect to the Docker daemon socket
```

**Solutions:**
```bash
# Add user to docker group
sudo usermod -aG docker $USER

# Logout and login again
exit
# SSH lại vào server

# Verify
docker ps
```

---

### Problem 4: Image pull failed

**Error:**
```
Error response from daemon: pull access denied
```

**Solutions:**

1. **Login to GHCR:**
```bash
echo YOUR_TOKEN | docker login ghcr.io -u YOUR_USERNAME --password-stdin
```

2. **Make package public:**
   - Go to: https://github.com/users/nam12341111/packages
   - Click on package → Settings → Change visibility to Public

---

### Problem 5: Health check failed

**Error:**
```
Health check failed after 10 attempts
```

**Solutions:**

1. **Check API logs:**
```bash
docker-compose logs api
```

2. **Check container status:**
```bash
docker-compose ps
```

3. **Check database connection:**
```bash
docker-compose logs sqlserver
```

4. **Restart services:**
```bash
docker-compose restart
```

---

## 🎯 Best Practices

### 1. Security

✅ **DO:**
- Sử dụng SSH keys thay vì passwords
- Rotate SSH keys định kỳ (3-6 tháng)
- Sử dụng GitHub Secrets cho sensitive data
- Enable firewall và chỉ mở ports cần thiết
- Sử dụng strong passwords cho database

❌ **DON'T:**
- Commit SSH keys vào Git
- Hardcode passwords trong code
- Sử dụng root user cho deployment
- Expose unnecessary ports

---

### 2. Deployment

✅ **DO:**
- Test deployment trên staging trước
- Backup trước khi deploy
- Monitor logs sau deployment
- Setup health checks
- Have rollback plan

❌ **DON'T:**
- Deploy trực tiếp lên production không test
- Deploy vào giờ cao điểm
- Skip health checks
- Ignore error logs

---

### 3. Monitoring

```bash
# Setup monitoring với cron job
crontab -e

# Add health check mỗi 5 phút
*/5 * * * * curl -f http://localhost:5000/health || echo "API Down!" | mail -s "Alert" your@email.com
```

---

## 📊 Deployment Flow Summary

```
┌──────────────────┐
│  Push to GitHub  │
└────────┬─────────┘
         ↓
┌──────────────────┐
│  GitHub Actions  │
│  - Build & Test  │
│  - Build Docker  │
└────────┬─────────┘
         ↓
┌──────────────────┐
│   Push to GHCR   │
└────────┬─────────┘
         ↓
┌──────────────────┐
│   SSH to Server  │
└────────┬─────────┘
         ↓
┌──────────────────┐
│  Pull Image      │
│  docker-compose  │
│  up -d           │
└────────┬─────────┘
         ↓
┌──────────────────┐
│  Health Check    │
└────────┬─────────┘
         ↓
┌──────────────────┐
│   ✅ Success!    │
│  API Live @ :5000│
└──────────────────┘
```

---

## 🎓 Next Steps

1. **Setup Nginx reverse proxy** - Expose API qua domain và HTTPS
2. **Setup SSL/TLS** - Sử dụng Let's Encrypt
3. **Setup monitoring** - Prometheus + Grafana
4. **Setup backup** - Automated database backups
5. **Setup alerts** - Email/Slack notifications

---

## 📚 Tài liệu tham khảo

- [GitHub Actions SSH Action](https://github.com/appleboy/ssh-action)
- [Docker Deployment Guide](https://docs.docker.com/engine/install/)
- [GitHub Secrets Documentation](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [SSH Key Management](https://www.ssh.com/academy/ssh/keygen)

---

**🎉 Chúc mừng! Bạn đã setup xong CI/CD với SSH deployment!** 🚀

Giờ mỗi lần push code lên `main` branch, API sẽ tự động deploy lên production server! 🎯
