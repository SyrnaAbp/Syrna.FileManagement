# Syrna.FileManagement
File Management module for ABP framework.

[![ABP version](https://img.shields.io/badge/dynamic/xml?style=flat-square&color=yellow&label=abp&query=%2F%2FProject%2FPropertyGroup%2FVoloAbpPackageVersion&url=https%3A%2F%2Fraw.githubusercontent.com%2FSyrnaAbp%2FSyrna.FileManagement%2Fmaster%2FDirectory.Packages.props)](https://abp.io)
![build and test](https://img.shields.io/github/actions/workflow/status/SyrnaAbp/Syrna.FileManagement/build-all.yml?branch=dev&style=flat-square)
[![NuGet Download](https://img.shields.io/nuget/dt/Syrna.FileManagement.Application.svg?style=flat-square)](https://www.nuget.org/packages/Syrna.FileManagement.Application)
[![NuGet (with prereleases)](https://img.shields.io/nuget/vpre/Syrna.FileManagement.Application.svg?style=flat-square)](https://www.nuget.org/packages/Syrna.FileManagement.Application) 

An abp application module that allows manage blobs.

## Installation

1. Install the following NuGet packages. ([see how](https://github.com/SyrnaAbp/SyrnaAbpGuide/blob/master/docs/How-To.md#add-nuget-packages))

    * Syrna.FileManagement.Application
    * Syrna.FileManagement.Application.Contracts
    * Syrna.FileManagement.Domain
    * Syrna.FileManagement.Domain.Shared
    * Syrna.FileManagement.EntityFrameworkCore
    * Syrna.FileManagement.HttpApi
    * Syrna.FileManagement.HttpApi.Client
    * Syrna.FileManagement.Web
    * Syrna.FileManagement.Blazor
    * Syrna.FileManagement.Blazor.Server
    * Syrna.FileManagement.Blazor.WebAssembly

1. Add `DependsOn(typeof(FileManagementXxxModule))` attribute to configure the module dependencies. ([see how](https://github.com/SyrnaAbp/SyrnaAbpGuide/blob/master/docs/How-To.md#add-module-dependencies))

1. Add `builder.ConfigureFileManagement();` to the `OnModelCreating()` method in **MyProjectMigrationsDbContext.cs**.

1. Add EF Core migrations and update your database. See: [ABP document](https://docs.abp.io/en/abp/latest/Tutorials/Part-1?UI=MVC&DB=EF#add-database-migration).

## Usage

![File List](docs/images/file_list.png)

![Create Directory](docs/images/create_directory.png)

![Rename Directory](docs/images/rename_directory.png)

![File Upload Error](docs/images/file_upload_error.png)

![File İmage Viewer](docs/images/file_image_viewer.png)

![File Detail](docs/images/file_detail.png)

![File Rename](docs/images/file_rename.png)

![File Move](docs/images/file_move.png)

## Reference

### This project based on [EasyAbp FileManagement](https://github.com/EasyAbp/FileManagement)

### Differences

1. Demo project created for OpenIddict
2. Demo project extended modules added
3. Razor pages implemented
4. Added Image Viewer to blazor UI
5. Added Text Viewer to blazor UI
6. Exception improvments 
7. Localization improvments 
8. Added Turkish localization
9. Uploader improments for blazor UI


