# EfficientTaskManager

(https://roadmap.sh/projects/todo-list-api)

EfficientTaskManager is a comprehensive web application designed to help users manage their tasks effectively. It features secure authentication, user-friendly interfaces, and efficient data handling.

## Introduction Video

[![Watch the video](https://img.youtube.com/vi/6OEFkk3Gb_M/hqdefault.jpg)](https://youtu.be/6OEFkk3Gb_M)

...

## Features

- **JWT Authentication and Authorization**: Secure login and access control using JSON Web Tokens (JWT).
- **Logout Logic**: Blacklists tokens upon logout to prevent their reuse if compromised.
- **Google Single Sign-On**: Users can log in using their Google accounts for a streamlined experience.
- **API Pagination**: Efficient data retrieval by fetching a specific number of records at a time, optimizing bandwidth usage.
- **API Rate Limiting**: Implements a sliding window algorithm with limits of 100 requests per 60 seconds for general APIs and 50 requests per 60 seconds for login/register endpoints.
- **User-Friendly Error Handling**: Uses Fluent Results for more descriptive error messages and better handling of requests.
- **Frontend and Backend Integration**: Seamless connection between frontend and backend APIs using JavaScript.
- **Database Indexing**: Improved retrieval speed for TODOs by adding indexing to the database.

## Prerequisites

Ensure you have the following NuGet packages installed:

- **JWT Token**: For handling JSON Web Tokens.
- **Swashbuckle**: For Swagger documentation.
- **Identity**: For authentication and authorization.
- **.NET Core**: Ensure you are using .NET Core for the project.
