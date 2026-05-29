# Todo App - Full Stack Test Project

A modern, full-stack Todo application built with **ASP.NET Core Web API** (C#) backend and **React + Vite** frontend.

## Project Overview

This project demonstrates professional software development practices including:
- **Backend**: RESTful API with Entity Framework Core, dependency injection, and comprehensive unit testing
- **Frontend**: React with hooks, error handling, filtering, and search functionality
- **Testing**: 17 passing unit and integration tests with xUnit and Moq
- **Architecture**: Clean separation of concerns with service layer pattern

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+ with npm
- Visual Studio Code or similar editor

### Backend Setup

```bash
# Navigate to backend directory
cd TodoApi

# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run tests (17 tests)
dotnet test ../TodoApi.Tests

# Start API
dotnet run
```

### Frontend Setup

```bash
# Navigate to frontend directory
cd TodoUI

# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build
```

## 📝 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tasks` | Get all tasks (sorted by CreatedAt DESC) |
| POST | `/api/tasks` | Create new task |
| PUT | `/api/tasks/{id}` | Update task status/title |
| DELETE | `/api/tasks/{id}` | Delete task |

### Request/Response Examples

**Create Task:**
```json
POST /api/tasks
Content-Type: application/json

{
  "title": "Buy groceries"
}
```

**Update Task:**
```json
PUT /api/tasks/1
Content-Type: application/json

{
  "isCompleted": true,
  "title": "Optional new title"
}
```

## ✨ Features

### Backend
- ✅ RESTful API with proper HTTP status codes
- ✅ Input validation and error handling
- ✅ Dependency Injection pattern
- ✅ Service layer for business logic
- ✅ In-Memory database (Entity Framework Core)
- ✅ CORS enabled for React frontend
- ✅ Comprehensive unit/integration tests

### Frontend
- ✅ Add, update, delete tasks
- ✅ Mark tasks as complete/incomplete
- ✅ Search tasks by title
- ✅ Filter by status (All, Pending, Done)
- ✅ Task counters
- ✅ Error notifications
- ✅ Loading states
- ✅ Responsive mobile design
- ✅ Accessibility labels (ARIA)

## Testing

### Running Tests

```bash
cd TodoApi.Tests
dotnet test
```

### Test Coverage

**TasksControllerTests (9 tests)**
- ✅ GET empty list
- ✅ GET all tasks
- ✅ POST with valid title
- ✅ POST with empty title (validation)
- ✅ POST with null title (validation)
- ✅ DELETE existing task
- ✅ DELETE non-existent task
- ✅ PUT to update status
- ✅ PUT on non-existent task

**TaskServiceTests (8 tests)**
- ✅ Get tasks sorted by CreatedAt DESC
- ✅ Create task with trimming
- ✅ Update completion status
- ✅ Update title
- ✅ Update non-existent task
- ✅ Delete existing task
- ✅ Delete non-existent task
- ✅ Retrieve all created tasks

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core with In-Memory database
- **Testing**: xUnit, Moq
- **Architecture Pattern**: Service Layer with Dependency Injection

### Frontend
- **Library**: React 19
- **Build Tool**: Vite 8
- **Language**: JavaScript (JSX)
- **Styling**: CSS3 with CSS Variables
- **HTTP Client**: Fetch API