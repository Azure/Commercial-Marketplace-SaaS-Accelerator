# Use a Linux base image
FROM mcr.microsoft.com/dotnet/sdk:6.0

# Add Microsoft repository for PowerShell
RUN apt-get update && \
    apt-get install -y wget apt-transport-https software-properties-common && \
    wget -q "https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb" -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    apt-get update

# Install PowerShell
RUN apt-get install -y powershell

# Install .NET tools
RUN curl -o /tmp/dotnet-install.sh https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh && \
    chmod +x /tmp/dotnet-install.sh && \
    /tmp/dotnet-install.sh -version 6.0.417 && \
    dotnet tool install --global dotnet-ef --version 6.0.1

# Clone the specific release of the SaaS Accelerator
RUN git clone https://github.com/Azure/Commercial-Marketplace-SaaS-Accelerator.git -b 7.6.1 --depth 1 /app

# Set the working directory
WORKDIR /app/deployment

# Expose volume for local development
VOLUME /app
