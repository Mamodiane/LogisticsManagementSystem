# Logistics Management System

## Overview

Logistics Management System is a professional logistics and shipment management platform built using ASP.NET Core Web API, Entity Framework Core, and SQL Server.

The system is designed to manage the complete shipment lifecycle from creation to delivery while providing tracking, status management, driver assignment, and operational visibility.

---

## Features

### Shipment Management

* Create Shipments
* View Shipments
* Update Shipments
* Delete Shipments
* Track Shipments by Tracking Number

### Shipment Workflow

* Pending
* Assigned
* Collected
* In Transit
* Delivered
* Failed Delivery
* Returned
* Cancelled

### Shipment Properties

* Sender Information
* Receiver Information
* Pickup Address
* Delivery Address
* Weight
* Shipment Type
* Delivery Priority
* Tracking Number

### Tracking

* Search shipment using tracking number
* View shipment details
* View shipment status

---

## Technology Stack

### Backend

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* Repository Pattern
* Swagger / OpenAPI

### Development Tools

* Visual Studio 2026
* Git
* GitHub

---

## Project Structure

```text
LogisticsManagementSystem
│
├── Controllers
├── Data
├── Enums
├── Migrations
├── Models
├── Repositories
├── Properties
├── Program.cs
├── appsettings.json
└── README.md
```

---

## Database

### Main Tables

#### Shipments

Stores shipment information including:

* Tracking Number
* Sender Details
* Receiver Details
* Pickup Address
* Delivery Address
* Weight
* Status
* Priority
* Type

---

## API Endpoints

### Shipments

| Method | Endpoint                              | Description        |
| ------ | ------------------------------------- | ------------------ |
| GET    | /api/Shipments                        | Get all shipments  |
| GET    | /api/Shipments/{id}                   | Get shipment by ID |
| GET    | /api/Shipments/track/{trackingNumber} | Track shipment     |
| POST   | /api/Shipments                        | Create shipment    |
| PUT    | /api/Shipments/{id}                   | Update shipment    |
| DELETE | /api/Shipments/{id}                   | Delete shipment    |

---

## Future Enhancements

### Phase 1

* Shipment Status History (Audit Trail)
* Driver Management
* Vehicle Management

### Phase 2

* Driver Assignment
* Driver Portal
* Client Portal

### Phase 3

* Authentication & Authorization (JWT)
* Role-Based Access Control

### Phase 4

* Delivery Proof
* Failed Delivery Workflow
* Address Change Requests

### Phase 5

* Reporting & Analytics Dashboard
* Notifications (Email/SMS)

---

## Learning Objectives

This project demonstrates:

* ASP.NET Core Web API Development
* Entity Framework Core
* SQL Server Database Design
* Repository Pattern
* RESTful API Design
* Professional Software Architecture

---

## Author

Pilato Mmatshipyane

Full-Stack Software Developer
