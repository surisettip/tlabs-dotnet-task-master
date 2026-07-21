
# Developer Test Assessment C# (.Net)

Please read this document before starting the assessment.

## Context

This app is a simple ToDo List App. However, there are a few errors left in the code. The functionality of the app is very limited.

## Notes

We expect that you spend about 4-8 hours on this test.

We do not expect you to complete all the tasks. You can focus on the tasks that you find most important or interesting for such an app.

You can focus on backend tasks if you are not familiar with Vue.js or frontend.

You can also share your thoughts on how can you improve the app, what can be done better.

## Starting an app

**Start Backend**. You need to have .NET 8 SDK installed.

`cd Server/src/TestApp.Server`

`dotnet run`

Swagger (API docs) will be available under http://localhost:5000/swagger/index.html

**Start Frontend.**

`cd Server/rsc/VueFrontend`

`npm run dev`

The Frontend will be available under http://localhost:5173/

# Tasks

## Backend

* There are a few problems with the app. Update/delete functionality does not work properly - Done
* Implement Tags for the ToDo Item. Allow adding/removing tags for each item - Done
* Implement common error handling for the application - Done
* Implement filter and sort functionality for the ToDo items - Done
* (Bonus task) Implement caching - Done
* (Bonus task) Implement authentication for the application - Done
* (Bonus task) Implement pagination for the ToDo items - Done


## Frontend

* Fix update/delete functionality - Done
* Add filter/sort functionality - Done
* (Bonus) Tags create/edit/delete functionality - Done
* (Bonus) Add a UI components library (e.g. Quasar) to the project 
* (Bonus) Use pagination/infinite scroll for the elements - Done
* (Bonus) Authentication - Done
* (Bonus) Do not reload the list on every update of an element

## Submition

Please share your git repo with us after completing the task, e.g., by sharing your GitHub repo.


Todo Management Application – Implementation Summary
Project Overview

Enhanced the initial Todo application by implementing complete CRUD functionality, authentication, task organization, pagination, improved error handling, and documentation.

Features Implemented

🛠️ Task Management
Added Create, Read, Update, and Delete (CRUD) operations for tasks
Implemented task filtering and search functionality
Added task status management (Pending / Completed)

🏷️ Task Tagging
Added support for creating and updating tags
Enabled assigning tags to todo items for better organization

⚠️ Centralized Error Handling
Implemented common API response handling
Added consistent frontend error management
Improved user feedback for failed operations

🔐 Authentication & Authorization
Backend
-Implemented JWT-based authentication
-Added secure login APIs
-Protected API

Frontend
-Added Login UI
-Integrated JWT authentication flow
-Stored authentication token
-Protected routes for authenticated users

📄 Pagination
Added server-side pagination
Implemented paginated task responses
Updated frontend to support page navigation

⚡ Performance Improvements
Added caching to reduce unnecessary API requests
Improved application responsiveness


Outcome

The project evolved from a basic repository into a fully functional Todo Management System featuring secure authentication, complete task management, efficient data handling, enhanced user experience, and comprehensive documentation.