# DevOps Engineer - Technical Assignment

## 📌 Objective

This project showcases a production-grade deployment workflow for a containerized .NET API using AWS services, entirely defined through AWS CDK in C#. It includes infrastructure provisioning, application deployment, and environment-specific CI/CD automation.

---

## 🏗️ Architecture Summary

The infrastructure is built around a simple and scalable design that follows AWS best practices:

- **VPC** with two public subnets spread across availability zones
- **ECS Fargate Cluster** with a public **Application Load Balancer**
- **Dockerized .NET API**, containerized and pushed to **Amazon ECR**
- **Environment-specific CDK stacks** with dynamic region/account assignment
- Fully automated deployment using **GitHub Actions** with support for:
  - **Staging** (auto-deploy)
  - **Production** (manual approval after staging)

---

## 📁 Project Structure

```
ProductInfra/
├── src/ProductInfra/
│   ├── Program.cs                # Entrypoint - defines stack execution
│   ├── NetworkStack.cs           # VPC and networking setup
│   ├── EcsStack.cs               # ECS + ALB + Fargate config
│   ├── EcsStackProps.cs          # Custom stack props (VPC injection)
│   └── ProductInfra.csproj       # CDK project file
├── ProductApp/
│   ├── Program.cs                # .NET Web API returning a simple message
│   ├── ProductApp.csproj         # .NET 8.0 project file
│   └── Dockerfile                # Container definition
└── .github/workflows/deploy.yml # CI/CD automation via GitHub Actions
```

---

## 🚀 How to Deploy

### 🧰 Prerequisites

- AWS account and IAM user with sufficient permissions
- AWS CLI configured (`aws configure`)
- Node.js and npm installed
- AWS CDK CLI installed:
  ```bash
  npm install -g aws-cdk
  ```
- .NET 8 SDK installed
- Docker installed and running

---

### 🔐 GitHub Setup

#### 🔸 Repository Secrets (used in all jobs):

| Key                    | Value                            |
|------------------------|----------------------------------|
| `AWS_ACCESS_KEY_ID`    | Your AWS access key              |
| `AWS_SECRET_ACCESS_KEY`| Your AWS secret access key       |
| `AWS_ACCOUNT_ID`       | Your 12-digit AWS account number |

#### 🔸 Environment Secrets:

In **Settings → Environments → staging** and **production**, add:

| Key         | Example Value     |
|-------------|-------------------|
| `AWS_REGION`| `us-east-1` (staging), `us-west-2` (prod) |

Also assign manual reviewers for the `production` environment.

---

### 🧱 CDK Workflow

1. Clone the repository:
   ```bash
   git clone https://github.com/your-user/ProductInfra.git
   cd ProductInfra
   ```

2. Build and push your Docker image (one-time or as needed):
   ```bash
   docker build -t product-app ./ProductApp
   docker tag product-app:latest <account-id>.dkr.ecr.<region>.amazonaws.com/product-app:latest
   docker push <account-id>.dkr.ecr.<region>.amazonaws.com/product-app:latest
   ```

3. Bootstrap your AWS environment (per region):
   ```bash
   cdk bootstrap aws://<account-id>/<region>
   ```

4. Deploy with CDK:
   ```bash
   cdk deploy --all
   ```

Or use the GitHub Actions pipeline to automate this on push.

---

## 🔄 CI/CD Pipeline

GitHub Actions handles deployment for both environments via `.github/workflows/deploy.yml`.

- **Staging** is auto-deployed on push to `main`
- **Production** waits for **manual approval** in the Actions tab

Each job:
- Builds and pushes the Docker image to the appropriate region
- Bootstraps the region (if needed)
- Deploys all CDK stacks using the environment-specific AWS region and credentials

---

## 🌐 Live Output

After deployment, the final load balancer DNS will be printed in the CLI (or GitHub Actions log):

```
EcsStack.LoadBalancerDNS = http://your-alb-name.elb.amazonaws.com
```

Paste it in a browser — you should see:
```
Hello from Product Management API!
```

---

## ⚠️ Known Limitations

- No database integration (by design)
- HTTPS/ACM not configured
- All infra and app are stateless
- Monitoring (CloudWatch/X-Ray) not implemented
- GitHub Actions requires Docker layer caching for faster builds

---

## 📈 What I Would Do With More Time

- Add Route53 + SSL/TLS support via ACM
- Setup centralized logging with CloudWatch and metrics alarms
- Implement Blue/Green ECS deployments
- Add auto-scaling policies based on CPU usage
- Move Docker builds to CodeBuild for faster deploys
- Integrate with SSM Parameter Store or AWS Secrets Manager for secure config

---

## ✅ Summary

This repository presents a clean, fully reproducible infrastructure stack using AWS CDK in C#, and deploys a Docker-based .NET API to ECS Fargate. It includes real-world CI/CD automation with proper environment isolation, ECR-based image delivery, and load-balanced, region-aware deployments — making it easy to extend, secure, and scale.

```bash
Everything deploys cleanly with just:
> git push origin main
```

---

Let me know if you’d like a PNG diagram for the architecture diagram as well 🎯
