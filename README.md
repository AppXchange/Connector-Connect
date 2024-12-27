# Trimble Connect App Xchange Connector

## Overview
The **Trimble Connect App Xchange Connector** integrates Trimble Connect's core functionalities with the App Xchange platform, enabling seamless file and project management through a set of API endpoints. This connector allows users to manage projects, upload files, and create folders within Trimble Connect while automating workflows in App Xchange.

### Connect API Documentation
- [SwaggerHub API Docs](https://app.swaggerhub.com/apis/Trimble-Connect/tcps/2.0)
- [Trimble Connect Core API Docs](https://developer.trimble.com/docs/connect/core-api)

## Endpoints Developed

### 1. List All Projects
- **Endpoint**: `GET /Projects`
- **Description**: This endpoint retrieves a list of all projects in Trimble Connect.
- **API Documentation**: [GET /projects](https://developer.trimble.com/docs/connect/core-api#get-/projects)
- **Usage**: Use this endpoint to obtain a list of projects in Trimble Connect, which can be used to manage or integrate project-related data in App Xchange.

### 2. Get File Structure
- **Endpoint**: `GET /files/fs/snapshot`
- **Description**: This endpoint retrieves the file structure of a project in Trimble Connect, providing a snapshot of the files and folders.
- **API Documentation**: [GET /files/fs/snapshot](https://developer.trimble.com/docs/connect/core-api#get-/files/fs/snapshot)
- **Usage**: Use this endpoint to view the structure of files within a specific project. This is useful for visualizing file organization and building custom file workflows in App Xchange.

### 3. Create a New Folder
- **Endpoint**: `POST /folders`
- **Description**: This endpoint allows the creation of a new folder within a project’s file structure in Trimble Connect.
- **API Documentation**: [POST /folders](https://developer.trimble.com/docs/connect/core-api#post-/folders)
- **Usage**: Use this endpoint to create new folders to organize files within a project in Trimble Connect. Folders can be structured based on project needs or workflow automation in App Xchange.

### 4. Package File Upload
- **Endpoint**: `POST /files/fs/upload`
- **Description**: This endpoint starts the process of uploading a file to Trimble Connect. It handles the initial upload request and prepares the system to receive file data.
- **API Documentation**: [POST /files/fs/upload](https://developer.trimble.com/docs/connect/core-api#post-/files/fs/upload)
- **Usage**: This endpoint should be called when starting a file upload to Trimble Connect. It supports multipart file uploads, making it suitable for large files that need to be transferred in parts.

### 5. Complete Multipart File Upload
- **Endpoint**: `POST /files/fs/upload/{uploadId}/complete`
- **Description**: This endpoint finalizes a multipart file upload by completing the upload process.
- **API Documentation**: [POST /files/fs/upload/{uploadId}/complete](https://developer.trimble.com/docs/connect/core-api#post-/files/fs/upload/-uploadId-/complete)
- **Usage**: Once a file is uploaded in parts using the `POST /files/fs/upload` endpoint, this endpoint must be used to complete the upload and ensure the file is fully saved in Trimble Connect.
