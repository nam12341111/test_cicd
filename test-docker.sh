#!/bin/bash
# Bash script to test Docker setup
# Run this script after Docker Desktop is running

echo -e "\033[1;36m=== Docker Test Script ===\033[0m"
echo ""

# Step 1: Check Docker is running
echo -e "\033[1;33m[1/6] Checking Docker status...\033[0m"
if docker info > /dev/null 2>&1; then
    echo -e "\033[1;32m✓ Docker is running!\033[0m"
else
    echo -e "\033[1;31m✗ Docker is not running. Please start Docker Desktop first.\033[0m"
    exit 1
fi
echo ""

# Step 2: Build Docker image
echo -e "\033[1;33m[2/6] Building Docker image...\033[0m"
echo -e "\033[0;37mThis may take 3-5 minutes on first build...\033[0m"
if docker build -t ptj-api:test .; then
    echo -e "\033[1;32m✓ Docker image built successfully!\033[0m"
else
    echo -e "\033[1;31m✗ Failed to build Docker image\033[0m"
    exit 1
fi
echo ""

# Step 3: Check if containers are already running
echo -e "\033[1;33m[3/6] Checking for existing containers...\033[0m"
if [ "$(docker ps --filter 'name=ptj-' --format '{{.Names}}')" ]; then
    echo -e "\033[0;33mFound running containers. Stopping them...\033[0m"
    docker-compose down
fi
echo -e "\033[1;32m✓ Ready to start fresh\033[0m"
echo ""

# Step 4: Start services with docker-compose
echo -e "\033[1;33m[4/6] Starting services with docker-compose...\033[0m"
echo -e "\033[0;37mStarting: API + SQL Server\033[0m"
if docker-compose up -d; then
    echo -e "\033[1;32m✓ Services started!\033[0m"
else
    echo -e "\033[1;31m✗ Failed to start services\033[0m"
    exit 1
fi
echo ""

# Step 5: Wait for services to be ready
echo -e "\033[1;33m[5/6] Waiting for services to be ready...\033[0m"
echo -e "\033[0;37mWaiting 30 seconds for SQL Server and API to initialize...\033[0m"
sleep 30

# Check API health
echo -e "\033[0;37mChecking API health...\033[0m"
for i in {1..10}; do
    if curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/swagger/index.html | grep -q "200"; then
        echo -e "\033[1;32m✓ API is responding!\033[0m"
        break
    else
        if [ $i -eq 10 ]; then
            echo -e "\033[0;33m⚠ API not responding yet, but may need more time\033[0m"
        else
            echo -e "\033[0;37m  Attempt $i/10... waiting...\033[0m"
            sleep 3
        fi
    fi
done
echo ""

# Step 6: Show results
echo -e "\033[1;33m[6/6] Test Results\033[0m"
echo ""
echo -e "\033[1;36m=== Container Status ===\033[0m"
docker-compose ps
echo ""

echo -e "\033[1;36m=== Access URLs ===\033[0m"
echo -e "\033[1;37mAPI:         http://localhost:5000\033[0m"
echo -e "\033[1;37mSwagger UI:  http://localhost:5000/swagger\033[0m"
echo -e "\033[1;37mSQL Server:  localhost:1433\033[0m"
echo -e "\033[0;37m  Username:  sa\033[0m"
echo -e "\033[0;37m  Password:  YourStrong@Password123\033[0m"
echo ""

echo -e "\033[1;36m=== Container Logs ===\033[0m"
echo -e "\033[1;37mView API logs:      docker-compose logs -f api\033[0m"
echo -e "\033[1;37mView SQL logs:      docker-compose logs -f sqlserver\033[0m"
echo -e "\033[1;37mView all logs:      docker-compose logs -f\033[0m"
echo ""

echo -e "\033[1;36m=== Useful Commands ===\033[0m"
echo -e "\033[1;37mStop services:      docker-compose down\033[0m"
echo -e "\033[1;37mRestart services:   docker-compose restart\033[0m"
echo -e "\033[1;37mView containers:    docker ps\033[0m"
echo -e "\033[1;37mEnter API shell:    docker exec -it ptj-api bash\033[0m"
echo ""

echo -e "\033[1;32m✓ Docker test complete!\033[0m"
echo ""
echo -e "To stop all services, run: \033[1;33mdocker-compose down\033[0m"
