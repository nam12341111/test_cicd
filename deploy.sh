#!/bin/bash
# Deployment script for production server
# This script will be executed on the server via SSH

set -e  # Exit on error

echo "========================================="
echo "🚀 PTJ API Deployment Script"
echo "========================================="
echo ""

# Configuration
APP_DIR="/home/$(whoami)/app"
DOCKER_COMPOSE_FILE="$APP_DIR/docker-compose.yml"
IMAGE_NAME="ghcr.io/nam12341111/test_cicd:latest"

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Functions
log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is running
check_docker() {
    log_info "Checking Docker status..."
    if ! docker info > /dev/null 2>&1; then
        log_error "Docker is not running!"
        exit 1
    fi
    log_info "✓ Docker is running"
}

# Navigate to app directory
navigate_to_app() {
    log_info "Navigating to app directory: $APP_DIR"
    if [ ! -d "$APP_DIR" ]; then
        log_error "App directory does not exist: $APP_DIR"
        exit 1
    fi
    cd "$APP_DIR"
    log_info "✓ Current directory: $(pwd)"
}

# Backup current deployment
backup_deployment() {
    log_info "Creating backup..."
    BACKUP_DIR="$APP_DIR/backups/$(date +%Y%m%d_%H%M%S)"
    mkdir -p "$BACKUP_DIR"

    if docker-compose ps -q > /dev/null 2>&1; then
        docker-compose ps > "$BACKUP_DIR/containers.txt"
        log_info "✓ Backup created at: $BACKUP_DIR"
    else
        log_warn "No running containers to backup"
    fi
}

# Pull latest Docker image
pull_image() {
    log_info "Pulling latest Docker image: $IMAGE_NAME"
    if docker pull "$IMAGE_NAME"; then
        log_info "✓ Image pulled successfully"
    else
        log_error "Failed to pull Docker image"
        exit 1
    fi
}

# Stop old containers
stop_containers() {
    log_info "Stopping old containers..."
    if docker-compose down; then
        log_info "✓ Containers stopped"
    else
        log_warn "No containers to stop or error occurred"
    fi
}

# Start new containers
start_containers() {
    log_info "Starting new containers..."
    if docker-compose up -d; then
        log_info "✓ Containers started successfully"
    else
        log_error "Failed to start containers"
        rollback
        exit 1
    fi
}

# Health check
health_check() {
    log_info "Performing health check..."

    # Wait for API to start
    sleep 15

    MAX_RETRIES=10
    RETRY_COUNT=0

    while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
        if curl -f http://localhost:5000/health > /dev/null 2>&1 || \
           curl -f http://localhost:5000/swagger/index.html > /dev/null 2>&1; then
            log_info "✓ Health check passed!"
            return 0
        fi

        RETRY_COUNT=$((RETRY_COUNT + 1))
        log_warn "Health check failed, retry $RETRY_COUNT/$MAX_RETRIES..."
        sleep 5
    done

    log_error "Health check failed after $MAX_RETRIES attempts"
    return 1
}

# Rollback to previous version
rollback() {
    log_error "Starting rollback..."
    # Implement rollback logic here if needed
    docker-compose down
    log_info "Rollback completed. Please check logs and redeploy manually."
}

# Clean up old images
cleanup() {
    log_info "Cleaning up old Docker images..."
    docker image prune -af
    log_info "✓ Cleanup completed"
}

# Show deployment status
show_status() {
    echo ""
    echo "========================================="
    log_info "Deployment Status"
    echo "========================================="

    log_info "Running containers:"
    docker-compose ps

    echo ""
    log_info "Recent logs:"
    docker-compose logs --tail=20 api

    echo ""
    log_info "Disk usage:"
    df -h | grep -E '^Filesystem|/$'

    echo ""
    log_info "Docker disk usage:"
    docker system df
}

# Main deployment flow
main() {
    log_info "Starting deployment process..."
    echo ""

    # Execute deployment steps
    check_docker
    navigate_to_app
    backup_deployment
    pull_image
    stop_containers
    start_containers

    # Health check
    if health_check; then
        cleanup
        show_status

        echo ""
        echo "========================================="
        log_info "✅ Deployment completed successfully!"
        echo "========================================="
        echo ""
        log_info "API URL: http://$(hostname -I | awk '{print $1}'):5000"
        log_info "Swagger: http://$(hostname -I | awk '{print $1}'):5000/swagger"
    else
        log_error "Deployment failed during health check"
        rollback
        exit 1
    fi
}

# Run main function
main
