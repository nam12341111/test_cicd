# PowerShell script to test Docker setup
# Run this script after Docker Desktop is running

Write-Host "=== Docker Test Script ===" -ForegroundColor Cyan
Write-Host ""

# Step 1: Check Docker is running
Write-Host "[1/6] Checking Docker status..." -ForegroundColor Yellow
try {
    docker info | Out-Null
    Write-Host "✓ Docker is running!" -ForegroundColor Green
} catch {
    Write-Host "✗ Docker is not running. Please start Docker Desktop first." -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 2: Build Docker image
Write-Host "[2/6] Building Docker image..." -ForegroundColor Yellow
Write-Host "This may take 3-5 minutes on first build..." -ForegroundColor Gray
docker build -t ptj-api:test .
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Docker image built successfully!" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to build Docker image" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 3: Check if containers are already running
Write-Host "[3/6] Checking for existing containers..." -ForegroundColor Yellow
$runningContainers = docker ps --filter "name=ptj-" --format "{{.Names}}"
if ($runningContainers) {
    Write-Host "Found running containers. Stopping them..." -ForegroundColor Yellow
    docker-compose down
}
Write-Host "✓ Ready to start fresh" -ForegroundColor Green
Write-Host ""

# Step 4: Start services with docker-compose
Write-Host "[4/6] Starting services with docker-compose..." -ForegroundColor Yellow
Write-Host "Starting: API + SQL Server" -ForegroundColor Gray
docker-compose up -d
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Services started!" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to start services" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 5: Wait for services to be ready
Write-Host "[5/6] Waiting for services to be ready..." -ForegroundColor Yellow
Write-Host "Waiting 30 seconds for SQL Server and API to initialize..." -ForegroundColor Gray
Start-Sleep -Seconds 30

# Check API health
Write-Host "Checking API health..." -ForegroundColor Gray
for ($i = 1; $i -le 10; $i++) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/swagger/index.html" -TimeoutSec 5 -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Host "✓ API is responding!" -ForegroundColor Green
            break
        }
    } catch {
        if ($i -eq 10) {
            Write-Host "⚠ API not responding yet, but may need more time" -ForegroundColor Yellow
        } else {
            Write-Host "  Attempt $i/10... waiting..." -ForegroundColor Gray
            Start-Sleep -Seconds 3
        }
    }
}
Write-Host ""

# Step 6: Show results
Write-Host "[6/6] Test Results" -ForegroundColor Yellow
Write-Host ""
Write-Host "=== Container Status ===" -ForegroundColor Cyan
docker-compose ps
Write-Host ""

Write-Host "=== Access URLs ===" -ForegroundColor Cyan
Write-Host "API:         http://localhost:5000" -ForegroundColor White
Write-Host "Swagger UI:  http://localhost:5000/swagger" -ForegroundColor White
Write-Host "SQL Server:  localhost:1433" -ForegroundColor White
Write-Host "  Username:  sa" -ForegroundColor Gray
Write-Host "  Password:  YourStrong@Password123" -ForegroundColor Gray
Write-Host ""

Write-Host "=== Container Logs ===" -ForegroundColor Cyan
Write-Host "View API logs:      docker-compose logs -f api" -ForegroundColor White
Write-Host "View SQL logs:      docker-compose logs -f sqlserver" -ForegroundColor White
Write-Host "View all logs:      docker-compose logs -f" -ForegroundColor White
Write-Host ""

Write-Host "=== Useful Commands ===" -ForegroundColor Cyan
Write-Host "Stop services:      docker-compose down" -ForegroundColor White
Write-Host "Restart services:   docker-compose restart" -ForegroundColor White
Write-Host "View containers:    docker ps" -ForegroundColor White
Write-Host "Enter API shell:    docker exec -it ptj-api bash" -ForegroundColor White
Write-Host ""

Write-Host "=== Quick Test ===" -ForegroundColor Cyan
Write-Host "Opening Swagger in browser..." -ForegroundColor Yellow
Start-Process "http://localhost:5000/swagger"
Write-Host ""

Write-Host "✓ Docker test complete!" -ForegroundColor Green
Write-Host ""
Write-Host "To stop all services, run: " -NoNewline
Write-Host "docker-compose down" -ForegroundColor Yellow
