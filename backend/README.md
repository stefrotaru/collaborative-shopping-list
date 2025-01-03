# Collaborative Shopping List Backend

## Description

The backend for Collaborative Shopping List App

## Built with

- .NET: a free, cross-platform, open-source developer platform for building many different types of applications.
- Entity Framework Core: a open-source and cross-platform object-relational mapper (ORM) for .NET. It enables developers to work with a database using .NET objects, eliminating the need for most of the data-access code that developers usually need to write. EF Core supports LINQ queries, change tracking, updates, and schema migrations.

## Design Patterns

1. Repository Pattern: a data access pattern that provides an abstraction layer between the database and the business logic of the application. It encapsulates the data access logic and provides a set of methods to perform CRUD (Create, Read, Update, Delete) operations on the database. 
    
    In this project I'm creating repository classes for each entity (e.g., UserRepository, GroupRepository, ShoppingListRepository) that will handle the data access operations. These repositories will be responsible for interacting with the database using Entity Framework Core.
    
    The main benefits of using the Repository pattern are: 
    
    - Separation of concerns: It separates the data access logic from the business logic, making the code more modular and easier to maintain.
    - Testability: Repositories can be easily mocked or replaced with fake implementations during unit testing, allowing for better testability of the business logic.
    - Flexibility: It provides flexibility to change the underlying data storage mechanism without affecting the rest of the application.
    
2. Dependency Injection (DI) Pattern: a design pattern that allows the dependencies of a class to be provided from outside, rather than the class creating or managing its own dependencies. It promotes loose coupling and makes the code more testable and maintainable.
    
    In this project, I'm using the built-in dependency injection container provided by ASP.NET Core. I'm registering the required dependencies (such as repositories, services, etc.) in the Startup class, and the container will handle the creation and lifetime management of these dependencies.
    
    The main advantages of using Dependency Injection are: 
    
    - Loose coupling: Classes are not tightly coupled to their dependencies, making the code more flexible and easier to modify.
    - Testability: Dependencies can be easily replaced with mock or fake implementations during unit testing, allowing for better testability of individual components.
    - Reusability: Components become more reusable as they are not tied to specific implementations of their dependencies.

3. Service Layer Pattern: an architectural pattern that defines a layer of services between the business logic and the presentation layer (in this case, the API controllers). It encapsulates the business logic and provides a set of methods that can be consumed by the presentation layer.
    
    In this project, I'm creating service classes (e.g., UserService, GroupService, ShoppingListService) that will contain the business logic for each feature. These services will depend on the repositories to access the data and will be injected into the API controllers using Dependency Injection.
    
    The main benefits of using the Service Layer pattern are:
    
    - Separation of concerns: It separates the business logic from the presentation layer, making the code more modular and easier to maintain.
    - Reusability: Business logic can be reused across different parts of the application or even in other applications.
    - Testability: Services can be easily tested in isolation, as they don't depend on the presentation layer or the database directly.

4. DTO (Data Transfer Object) Pattern: used to transfer data between different layers of the application. DTOs are simple objects that contain only the data needed for a specific operation, without any business logic or behavior.
    
    In this project, I'm creating DTO classes for each entity (e.g., UserDTO, GroupDTO, ShoppingListDTO) that will be used to transfer data between the API controllers and the services. DTOs help in reducing the coupling between layers and provide a way to shape the data according to the needs of the client.
    
    The main advantages of using the DTO pattern are: 
    
    - Data shaping: DTOs allow you to shape the data according to the needs of the client, exposing only the necessary properties and hiding the internal details of the domain entities.
    - Reduced coupling: DTOs decouple the presentation layer from the domain layer, allowing them to evolve independently.
    - Network efficiency: DTOs can be optimized for network transfer by including only the required data, reducing the amount of data sent over the network.