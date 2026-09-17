# Salesforce Data Manager

A full-stack web application that allows users to manage Salesforce standard objects through a custom Angular UI and ASP.NET Core Web API.

The application integrates with Salesforce using OAuth 2.0 and Salesforce REST APIs.

## Features

- Salesforce OAuth 2.0 authentication
- Secure session-based authentication
- Dynamic Salesforce object selection
- Support for:
  - Account
  - Opportunity
  - Lead
  - Contact
  - Case
- View Salesforce records
- Create Salesforce records
- Edit Salesforce records
- Delete Salesforce records
- Dynamic fields based on selected object
- Required field validation
- Pagination with 20 records per page
- Responsive and clean user interface
- ASP.NET Core Web API backend
- Angular frontend
- Salesforce REST API integration

## Technology Stack

### Frontend

- Angular
- TypeScript
- HTML5
- CSS3

### Backend

- ASP.NET Core Web API
- C#
- HttpClient
- Session Management
- OAuth 2.0

### External API

- Salesforce REST API
- Salesforce OAuth 2.0

### Development Tools

- Visual Studio
- Visual Studio Code
- Git
- GitHub
- Postman

## Project Structure

```text
CloudVandana
│
├── CloudVandana.Api
│   ├── Configuration
│   ├── Controllers
│   ├── Models
│   ├── Services
│   ├── Program.cs
│   └── appsettings.json
│
├── CloudVandana.Client
│   ├── src
│   │   └── app
│   ├── angular.json
│   ├── package.json
│   └── README.md
│
└── CloudVandana.slnx