# LotusGoIM后端

## 所用技术

使用Asp.Net Core Web Api (.Net 6) + SingalR + Redis + EF Core完成，单服务
数据库使用SQL Sever

> 运行前请配置Redis、Sql Server，若要使用自带上传文件功能必须使用Linux服务器并配置好Nginx，注册的第一个用户拥有管理员权限

> 记得修改appsettings.json配置

## 迁移数据库

1. 删除Migrations文件夹

2. 程序包管理器运行

``` add-migration ```
``` update-database ```

## 发布应用

> 虽然写得很水但是前端更水