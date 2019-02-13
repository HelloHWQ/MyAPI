# MyAPI
一个简单的restFul风格的API，基于dotnet core2.2，基于现有数据库的开发方式
## 开发步骤
+ 使用SQLserver2008及以上版本设计数据库
+ 打开vs2017，创建ASP.Net Core Web应用程序，选择API
## 反向工程
+ 通过数据库逆向生成Model
+ 在程序包管理器控制台执行```Scaffold-DbContext "Server=AFAAW-807092021\MSSQL2008R2;uid=sa;pwd=sa;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models```
+ 也可以通过-Tables Blog,Post 命令指定某些表
## 编码
+ 创建控制器
+ 在控制器中编写restFul风格的API
## 添加swagger插件完成接口说明页和调试页
+ 通过NuGet安装Swashbuckle.AspNetCore
+ 在Startup.ConfigureServices中添加配置
```services.AddSwaggerGen(c =>{ c.SwaggerDoc("v1", new Info { Title = "My API" Version = "v1" }); });```
## 在Startup.Configure中启用swagger UI 和json文档的服务
+ ```app.UseSwagger();```
+ ``` // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.```
  ```  app.UseSwaggerUI(c =>{c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");});```
## 在Startup.ConfigureServices中启用xml详细说明
+ ```var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";```

  ```var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);```

  ```c.IncludeXmlComments(xmlPath);```
## 在XXX.csproj配置文件中添加
```
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```
## 使用默认的Json序列化程序在序列化导航属性的时候会序列化不成功，这里配置Newtonsoft Json这替换全局的默认Json序列化
+ 通过NuGet安装 Newtonsoft Json程序包
+ 在Startup.中添加全局设置
```
services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
      //全局配置Json序列化处理
  .AddJsonOptions(options =>
  {
      //忽略循环引用
      options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
      //不使用驼峰样式的key
      options.SerializerSettings.ContractResolver = new DefaultContractResolver();
      //设置时间格式
      options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
  });
```
## IIS部署
+ 需安装对应版本的dotnet-hosting-2.2.1-win.exe
+ 应用程序池选择“无托管代码”和“集成模式”
+ 为对应的应用程序池添加处理程序映射
+ 添加模块映射：aspNetCore       *.        已启用    未指定    AspNetCoreModuleV2   本地
+ 重启站点