# Repository Architecture

This repository is designed as a **multi-cloud, multi-CI platform** with strict separation of concerns and symmetry across AWS, Azure, and GCP.

## Structure

```

Infrastructure/
â”œâ”€â”€ AzureDevOps-Bicep/     # Azure infra defined in Bicep
â”œâ”€â”€ Terraform-AWS/         # AWS infra with Terraform
â”œâ”€â”€ Terraform-GCP/         # GCP infra with Terraform

pipelines/                  # Azure DevOps YAML jobs
.github/workflows/          # GitHub Actions workflows
.gitlab/ci/                 # GitLab CI/CD pipelines

````

## CI/CD Flow

- **Build & Test** â†’ restore, build, test .NET code
- **Docker Build & Push** â†’ builds images, pushes to cloud registry
- **Infra Deploy** â†’ provisions infra via Bicep/Terraform
- **Service Deploy** â†’ deploys to Azure, GCP, or AWS targets (AKS, GKE, EKS, Cloud Run, ECS)

## Architecture Diagram

```mermaid
flowchart TD

  subgraph CI-CD["CI/CD Platforms"]
    ADO["Azure DevOps"]
    GH["GitHub Actions"]
    GL["GitLab CI"]
  end

  subgraph Infra["Infrastructure as Code"]
    AZB["Azure Bicep"]
    TFAWS["Terraform AWS"]
    TFGCP["Terraform GCP"]
  end

  subgraph App["Application Layer"]
    FRZ["Orbita Framework"]
    SRV["Microservices"]
  end

  ADO --> AZB
  GH --> TFAWS
  GL --> TFGCP

  Infra --> App
```
## Multi-Cloud Deployment Architecture

```mermaid
flowchart TD

  subgraph CI-CD["CI/CD Platforms"]
    ADO["Azure DevOps"]
    GH["GitHub Actions"]
    GL["GitLab CI"]
  end

  subgraph Infra["Infrastructure as Code"]
    subgraph Azure
      AZB["Bicep Templates"]
      AKS["Deploy â†’ AKS"]
    end

    subgraph AWS
      TFAWS["Terraform AWS"]
      ECS["Deploy â†’ ECS"]
      EKS["Deploy â†’ EKS"]
    end

    subgraph GCP
      TFGCP["Terraform GCP"]
      GKE["Deploy â†’ GKE"]
      CR["Deploy â†’ Cloud Run"]
    end
  end

  subgraph App["Application Layer"]
    FRZ["Orbita Framework"]
    SRV["Microservices"]
  end

  %% CI/CD to Infra
  ADO --> AZB
  GH --> TFAWS
  GL --> TFGCP

  %% Infra to Deploy Targets
  AZB --> AKS
  TFAWS --> ECS
  TFAWS --> EKS
  TFGCP --> GKE
  TFGCP --> CR

  %% Infra to Application
  Infra --> App
```

