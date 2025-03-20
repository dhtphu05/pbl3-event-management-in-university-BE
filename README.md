# 🔍 Phân Tích Tech Stack  

## 1️⃣ Tổng Quan  
Dự án sử dụng **C# với ASP.NET Core** để phát triển backend nhằm đảm bảo hiệu suất, bảo mật và khả năng mở rộng.  

## 2️⃣ Công Nghệ Được Sử Dụng  

| Thành phần            | Công nghệ                     | Lý do lựa chọn |
|-----------------------|------------------------------|---------------|
| **Ngôn ngữ**         | `C#`                          | Hiệu suất cao, mạnh về xử lý backend, dễ bảo trì. |
| **Framework**        | `ASP.NET Core`                | Nhẹ, hỗ trợ cross-platform, tích hợp sẵn nhiều tính năng mạnh mẽ. |
| **Cơ sở dữ liệu**    | `SQL server`| Phù hợp với dữ liệu dạng `structured`, hỗ trợ truy vấn mạnh mẽ. |
| **ORM**             | `Entity Framework Core` | Giúp truy vấn dữ liệu dễ dàng, tối ưu hiệu suất. |
| **Authentication**  | `JWT (Bearer Token)`          | Bảo mật, stateless, phù hợp cho API RESTful. |
| **Caching**        | `In-Memory Caching`    | Tăng tốc độ truy xuất dữ liệu, giảm tải cho DB. |
| **Deployment**     | `Local` | Dự án PBL3 đơn giản nên chỉ host local |

## 3️⃣ Các Gói NuGet Được Cài Đặt  

Dưới đây là danh sách các gói **NuGet** quan trọng được sử dụng trong dự án:  

| Gói NuGet                     | Mô tả |
|--------------------------------|----------------------------|
| `Microsoft.AspNetCore.Mvc`      | Hỗ trợ xây dựng API RESTful. |
| `Microsoft.EntityFrameworkCore` | ORM(Object Relational Mapping) giúp thao tác cơ sở dữ liệu dễ dàng. |
| `Microsoft.EntityFrameworkCore.SqlServer` | Provider cho SQL Server. |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | Provider cho PostgreSQL. |
| `System.IdentityModel.Tokens.Jwt` | Hỗ trợ xác thực JWT token. |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | Middleware để xử lý JWT. |
| `FluentValidation`              | Hỗ trợ validate dữ liệu dễ dàng. |
| `Swashbuckle.AspNetCore`        | Tạo tài liệu API Swagger. |

Cài đặt tất cả gói NuGet sau khi clone bằng lệnh:  

```sh
dotnet restore
