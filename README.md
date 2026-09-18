\# IT \& Lab Resource Booking System



A web-based resource booking system built with ASP.NET Core MVC to help users manage and book IT and laboratory resources through a centralized platform.



\## Overview



The IT \& Lab Resource Booking System provides a structured way to manage institutional resources and booking requests. It includes user authentication, resource management, booking workflows, administrative functions, and reporting.



The project was developed as a practical application of software engineering concepts including MVC architecture, Entity Framework Core, dependency injection, authentication and authorization, database management, validation, and asynchronous programming.



\## Features



\* User registration and login

\* User authentication and authorization

\* Resource management

\* Resource creation, editing, viewing, and deletion

\* Booking creation and management

\* View personal bookings

\* Booking details

\* Administrative dashboard

\* Booking approval management

\* User management

\* Reports

\* Access-denied handling

\* Form validation

\* Anti-forgery protection



\## Technologies Used



\* \*\*C#\*\*

\* \*\*ASP.NET Core MVC\*\*

\* \*\*Entity Framework Core\*\*

\* \*\*SQLite\*\*

\* \*\*ASP.NET Core Identity\*\*

\* \*\*Razor Views\*\*

\* \*\*HTML/CSS\*\*

\* \*\*Dependency Injection\*\*

\* \*\*LINQ\*\*



\## Architecture



The application follows the Model-View-Controller (MVC) architectural pattern.



```text

User

&#x20;│

&#x20;▼

Razor Views

&#x20;│

&#x20;▼

Controllers

&#x20;│

&#x20;├── Authentication \& Authorization

&#x20;├── Booking Management

&#x20;├── Resource Management

&#x20;└── Administration

&#x20;│

&#x20;▼

Entity Framework Core

&#x20;│

&#x20;▼

SQLite Database

```



\## Project Structure



```text

LabBookingSystem/

│

├── Controllers/

│   ├── AccountController.cs

│   ├── AdminController.cs

│   ├── BookingsController.cs

│   ├── HomeController.cs

│   └── ResourcesController.cs

│

├── Data/

│   ├── ApplicationDbContext.cs

│   └── DbSeeder.cs

│

├── Models/

│   ├── ApplicationUser.cs

│   ├── Booking.cs

│   ├── Resource.cs

│   └── ViewModels/

│

├── Views/

│   ├── Account/

│   ├── Admin/

│   ├── Bookings/

│   ├── Home/

│   ├── Resources/

│   └── Shared/

│

├── wwwroot/

│   └── css/

│

├── Program.cs

└── LabBookingSystem.csproj

```



\## Database



The application uses \*\*SQLite\*\* with Entity Framework Core.



SQLite was selected because it provides a lightweight relational database suitable for a self-contained application without requiring a separate database server during development.



The local database file is intentionally excluded from version control.



\## Security



The project includes several ASP.NET Core security features, including:



\* ASP.NET Core Identity

\* Authentication and authorization

\* Role-based access control

\* Anti-forgery protection

\* Access-denied handling

\* Server-side validation



\## Running the Project Locally



\### Prerequisites



\* .NET SDK

\* Visual Studio or Visual Studio Code

\* Git



\### Clone the repository



```bash

git clone https://github.com/KayyyJayy/IT-Lab-Resource-Booking-System.git

```



\### Navigate to the project



```bash

cd IT-Lab-Resource-Booking-System

```



\### Restore dependencies



```bash

dotnet restore

```



\### Build the application



```bash

dotnet build

```



\### Run the application



```bash

dotnet run

```



The application will provide a local URL in the terminal.



\## Development Concepts Demonstrated



This project was built to apply practical software engineering concepts, including:



\* MVC architecture

\* Object-oriented programming

\* Database-driven application development

\* Entity Framework Core

\* Dependency Injection

\* Authentication and authorization

\* CRUD operations

\* ViewModels

\* Model validation

\* Asynchronous programming

\* Secure form handling

\* Separation of application concerns



\## Future Improvements



Potential future improvements include:



\* Improved booking conflict detection

\* Email notifications

\* Calendar-based booking interface

\* Advanced reporting

\* Search and filtering

\* Automated testing

\* API integration

\* Improved deployment configuration

\* Cloud database support



\## Author



\*\*Osborn Adusei Amankwah\*\*



Computer Engineering graduate interested in software engineering, backend development, and network engineering.



GitHub: https://github.com/KayyyJayy



