# graphql-mesh-microsoft-api
Example of difference in POST responses in GraphQL Mesh between a Microsoft-based REST API and a JS-based REST API

## Repository Components
- `todo-rest-api-asp-net`: Simple ToDo REST API service written in C# with ASP.NET
- `todo-rest-api-express`: Simple ToDo REST API service written in JavaScript with Express and Node.js
- `graphql-mesh-todo-service`: GraphQL Mesh service that serves both ToDo REST APIs

## Issue Description
The `post_aspnet_todoitems` mutation returns a `405 Method Not Allowed` error when invoked in the GraphQL Mesh service but is actually successful upstream (the ToDo item is still created in the ASP.NET-based API despite the error). This mutation calls the `POST /aspnet/todoitems` endpoint in the ASP.NET ToDo REST API. When this same POST call is issued outside of GraphQL Mesh, it successfully returns a `201 Created` response, so the issue is occurring exclusively within GraphQL. The Express-based ToDo API does not have this error and is included as a way to observe the correct/expected behavior.

## Run the Projects
The idea is to run both the ASP.NET and Express ToDo API services first. Then run the GraphQL Mesh service which is configured such that you can call both of these ToDo APIs via GraphQL Mesh.

Also included is a Postman collection which is a convenient way to try the two ToDo APIs on their own and see what the requests and responses look like outside of GraphQL Mesh.

### 1. Launch ASP.NET ToDo REST API (`/todo-rest-api-asp-net`)
*Requires: Visual Studio with .NET8 and ASP.NET workload installed*
- Open the `ToDoAPI.sln` file in Visual Studio
- Change the `https` run button to `http` and click it to launch the API service
- A browser should launch at `http://localhost:5166` which is where the API is served
*If the port is something other than 5166, please update that value in the `/graphql-mesh-todo-service/todo.openapi.aspnet.json` file at the `servers/url` location in the JSON.*

### 2. Launch Express ToDo REST API (`/todo-rest-api-express-js`)
*Requires: Node.js and npm*
- In a terminal, such as in Visual Studio Code, navigate to the `todo-rest-api-express-js` folder
- Run `npm install` to install package dependencies
- Run `node app.js` to launch the API service which should be available at http://localhost:3000

### 3. Launch GraphQL Mesh Service (`/graphql-mesh-todo-service`)
*Requires Node.js and npm*
- In a second terminal, such as in Visual Studio Code, navigate to the `graphql-mesh-todo-service` folder
- Run `npm install` to install package dependecies
- Run `npm run start` to launch the GraphQL service

## Reproduce the Issue
In a tool such as Postman or GraphQL Explorer, run the following mutation. This will attempt to create a new ToDo item in each of the upstream APIs. The Express API path will work and return the ToDo item back. The ASP.NET API path will fail with a `405 Method Not Allowed` error.

```graphql
mutation Post_aspnet_todoitems {
    post_aspnet_todoitems(
        input: { completed: false, title: "buy groceries (asp.net)", id: 1 }
    ) {
        id
        title
        completed
    }
    post_expressapp_todos(
        input: { completed: false, title: "buy groceries (express)", id: 1 }
    ) {
        id
        title
        completed
    }
}
```

Mutation response:
```json
{
    "errors": [
        {
            "message": "Upstream HTTP Error: 405, Could not invoke operation POST /aspnet/todoitems",
            "path": [
                "post_aspnet_todoitems"
            ],
            "extensions": {
                "code": "DOWNSTREAM_SERVICE_ERROR",
                "request": {
                    "url": "http://localhost:5166/aspnet/todoitems",
                    "method": "POST"
                },
                "response": {
                    "status": 405,
                    "statusText": "Method Not Allowed",
                    "headers": {
                        "content-length": "0",
                        "date": "Mon, 10 Feb 2025 20:06:20 GMT",
                        "server": "Kestrel",
                        "allow": "DELETE, GET, PUT"
                    },
                    "body": ""
                }
            }
        }
    ],
    "data": {
        "post_aspnet_todoitems": null,
        "post_expressapp_todos": {
            "id": 1,
            "title": "buy groceries (express)",
            "completed": false
        }
    }
}
```

Now, run the following query. Both upstream APIs will return the ToDo items that were created in each. The Express-based API behaves as expected. The ASP.NET API does not given the `405` error.
```graphql
query Aspnet_todoitems {
    aspnet_todoitems {
        id
        title
        completed
    }
    expressapp_todos {
        id
        title
        completed
    }
}
```

Query response:
```json
{
    "data": {
        "expressapp_todos": [
            {
                "id": 1,
                "title": "buy groceries (express)",
                "completed": false
            }
        ],
        "aspnet_todoitems": [
            {
                "id": 1,
                "title": "buy groceries (asp.net)",
                "completed": false
            }
        ]
    }
}
```