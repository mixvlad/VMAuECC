# VMAuECC - MaxPatrol VM Standards and Data Collection Generator

A comprehensive web application for generating configuration files for MaxPatrol VM data collection standards and controls. The application provides an intuitive interface for creating security compliance checks and data collection configurations for both Unix and Windows environments.

🎯 Project Overview
VMAuECC (VM Audit and Compliance Configuration) is a full-stack application that simplifies the creation of MaxPatrol VM control configurations. It enables security professionals to generate standardized files for various security checks, file integrity monitoring, registry audits, and compliance controls across different operating systems.

🔒 Available Control Types

## Unix Controls
- **File/Directory Existence Check** - Verify presence of critical files and directories
- **File/Directory Permission Check** - Audit file and directory permissions
- **File Content Check** - Validate specific content within files
- **File Integrity Check** - Monitor file integrity using checksums
- **Directory Integrity Check** - Ensure directory structure integrity
- **Command Result Check** - Validate command execution outputs
- **Script Result Check** - Monitor custom script execution results
- **Directory Search Check** - Find files matching specific criteria

## Windows Controls
- **Registry Key Existence** - Verify registry key presence
- **Registry Permission Check** - Audit registry access permissions
- **Registry Value Content Check** - Validate registry value contents
- **Registry Value Existence** - Check for specific registry values
- **Windows File Info** - Collect detailed file information
- **Windows File Integrity Check** - Monitor Windows file integrity
- **Windows Directory Integrity Check** - Ensure Windows directory integrity
- **Windows Command Result Check** - Validate Windows command outputs
- **Group Membership Check** - Verify user group memberships
- **Share Access Check** - Audit network share access
- **WMI Query Check** - Execute and validate WMI queries
- **Script Result Check** - Monitor Windows script execution

🏗️ Project Structure
```
VMAuECC/
├── frontend/                           # Angular frontend application
│   └── yaml-generator-frontend/
│       ├── src/app/
│       │   ├── components/             # UI components
│       │   │   ├── control-type-selection/  # Control type chooser
│       │   │   ├── os-selection/            # OS selection interface
│       │   │   ├── yaml-form/               # Configuration form
│       │   │   └── header/                  # Navigation header
│       │   ├── models/                 # TypeScript interfaces
│       │   ├── services/               # API communication
│       │   └── assets/i18n/            # Internationalization
│       └── nginx/                      # Web server configuration
├── backend/                            # .NET Core backend application
│   ├── YamlGenerator.API/              # REST API controllers
│   ├── YamlGenerator.Core/             # Business logic and services
│   │   ├── Data/                       # Control type definitions
│   │   │   ├── ControlTypes/           # YAML templates
│   │   │   │   ├── unix/               # Unix control templates
│   │   │   │   └── windows/            # Windows control templates
│   │   │   └── Templates/              # Base templates
│   │   ├── Models/                     # Data models
│   │   └── Services/                   # Business services
│   └── YamlGenerator.Tests/            # Unit tests
└── docker-compose.yml                  # Container orchestration
```

🚀 Quick Start

## Prerequisites
- Docker
- Docker Compose

## Installation & Setup
```bash
# Clone the repository
git clone https://github.com/your-username/VMAuECC.git
cd VMAuECC

# Start the application
docker-compose up --build
```

## Access the Application
Open your browser and navigate to: http://localhost:8080

## Usage Workflow
1. **Select Operating System** - Choose between Unix or Windows controls
2. **Choose Control Type** - Select from available security checks
3. **Configure Parameters** - Fill in required and optional parameters
4. **Generate YAML** - Create the configuration file
5. **Download** - Save the generated YAML for use in MaxPatrol VM

🛠️ Technology Stack

## Frontend
- **Angular 17** - Modern web application framework
- **TypeScript** - Type-safe JavaScript development
- **SCSS** - Advanced CSS styling
- **Angular Material** - Material Design components
- **i18n** - Internationalization support (English/Russian)

## Backend
- **.NET 8** - High-performance web framework
- **ASP.NET Core** - RESTful API development
- **YamlDotNet** - YAML serialization/deserialization
- **Dependency Injection** - Modular architecture

## Infrastructure
- **Docker** - Containerized deployment
- **Docker Compose** - Multi-container orchestration
- **Nginx** - Reverse proxy and static file serving

🔧 API Endpoints

### Control Types
- `GET /api/controltypes/{os}` - Get available control types for OS
- `GET /api/controltypes/{os}/{controlTypeId}` - Get specific control type details

### YAML Generation
- `POST /api/yaml/generate` - Generate YAML configuration
  - Request: JSON configuration object
  - Response: Generated YAML content

### Localization
- `GET /api/localization/{language}` - Get localized strings

📊 Features

## Security Controls
- **Comprehensive Coverage** - Support for 20+ security check types
- **OS-Specific Templates** - Optimized configurations for Unix and Windows
- **Parameter Validation** - Built-in validation for all control parameters
- **Default Values** - Pre-configured sensible defaults

## User Experience
- **Intuitive Interface** - Step-by-step configuration wizard
- **Real-time Preview** - Live YAML preview during configuration
- **Multi-language Support** - English and Russian interfaces
- **Responsive Design** - Works on desktop and mobile devices

## Configuration Management
- **Template System** - Reusable configuration templates
- **Parameter Inheritance** - Smart parameter handling
- **Validation Rules** - Automatic parameter validation
- **Export Options** - Direct download of generated files

🧪 Testing

## Unit Tests
```bash
# Run backend tests
cd backend
dotnet test

# Run frontend tests
cd frontend/yaml-generator-frontend
npm test
```

## Integration Tests
```bash
# Test the full application stack
docker-compose up --build
# Navigate to http://localhost:8080 and test functionality
```

🔧 Configuration

## Environment Variables
```bash
# Frontend configuration
ANGULAR_ENVIRONMENT=development
API_BASE_URL=http://localhost:5000

# Backend configuration
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:5000
```

## Docker Configuration
```yaml
# docker-compose.yml
version: '3.8'
services:
  frontend:
    build: ./frontend
    ports:
      - "8080:80"
    depends_on:
      - backend
  
  backend:
    build: ./backend
    ports:
      - "5000:5000"
```

📈 Performance

## Optimization Features
- **Lazy Loading** - Components load on demand
- **Caching** - Static assets and API responses cached
- **Compression** - Gzip compression for faster loading
- **CDN Ready** - Optimized for content delivery networks

## Monitoring
- **Health Checks** - Application health monitoring
- **Error Tracking** - Comprehensive error logging
- **Performance Metrics** - Response time monitoring

🤝 Contributing

## Development Workflow
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Code Standards
- Follow Angular style guide for frontend
- Use .NET coding conventions for backend
- Write unit tests for new features
- Update documentation as needed
- Use TypeScript strict mode

## Adding New Control Types
1. Create YAML template in appropriate OS directory
2. Add localization strings
3. Update control type definitions
4. Add unit tests
5. Update documentation

📝 License
This project is licensed under the MIT License - see the LICENSE file for details.

🙏 Acknowledgments
- **MaxPatrol VM** - For the security platform integration
- **Angular Team** - For the excellent frontend framework
- **Microsoft .NET Team** - For the robust backend framework
- **Open Source Community** - For the amazing tools and libraries

📞 Support
If you have questions or need help:

- Create an Issue on GitHub
- Check the Wiki for detailed documentation
- Contact the development team
- Review the API documentation

⭐ If this project helps you manage MaxPatrol VM configurations, please give it a star!

---

**About VMAuECC**
A comprehensive web application for generating YAML configuration files for MaxPatrol VM data collection standards and controls. The application provides an intuitive interface for creating security compliance checks and data collection configurations for both Unix and Windows environments. 