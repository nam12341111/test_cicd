# CI/CD Test

This file is created to test the CI/CD pipeline.

## Test Information

- **Date:** 2025-11-18
- **Purpose:** Verify CI/CD workflow execution
- **Expected Result:**
  - ✅ Job 1: Build and Test - PASS
  - ✅ Job 2: Build and Push Docker Image - PASS
  - ⏭️ Job 3: Deploy to Server - SKIPPED (disabled)

## Workflow Status

Check the latest workflow run at:
https://github.com/nam12341111/test_cicd/actions

## Docker Image

After successful workflow, the Docker image will be available at:
```
ghcr.io/nam12341111/test_cicd:latest
ghcr.io/nam12341111/test_cicd:main
ghcr.io/nam12341111/test_cicd:main-{commit-sha}
```

## Test Commands

```bash
# Pull the latest image
docker pull ghcr.io/nam12341111/test_cicd:latest

# Run locally
docker-compose up -d

# Access API
http://localhost:5000/swagger
```

---

**Test Status:** In Progress 🔄
