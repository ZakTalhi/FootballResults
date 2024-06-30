The structure of My project allows for separation of concerns, making the codebase easier to manage, test, and maintain. 
ClassLibrary contains the business logic and models necessary for the application. 
It includes  services (Messages), data models (Results), and interfaces (IMessages). 
This separation ensures that my core application logic is decoupled from other parts of the application.
I used Dependency Injection, (ILogger<Messages> in the Messages class), it offers several benefits that align with modern software development practices.
it encourages adherence to the SOLID principles of object-oriented design, particularly the Dependency Inversion Principle.  

Like any software project, there's always room for improvement:
1-Improve Error Handling.
2-Optimize Data Structures, if the list of teams becomes large,searching for teams could become inefficient.

Before running the solution, please ensure you have created a folder at 'C:\YourFolderPathHere' to store the required text files.
