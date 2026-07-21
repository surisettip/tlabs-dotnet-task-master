
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
