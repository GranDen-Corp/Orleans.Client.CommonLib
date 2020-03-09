# Orleans.Client.CommonLib

Orleans ([http://dotnet.github.io/orleans](http://dotnet.github.io/orleans)) Client stub creation and connection library,  
Using [Polly](http://www.thepollyproject.org) for compensate jitter connection situaiton.

**NOTE:** 
1. Be sure to add [Microsoft.Orleans.CodeGenerator.MSBuild](https://www.nuget.org/packages/Microsoft.Orleans.CodeGenerator.MSBuild/) or [Microsoft.Orleans.OrleansCodeGenerator.Build](https://www.nuget.org/packages/Microsoft.Orleans.OrleansCodeGenerator.Build/) or [Microsoft.Orleans.OrleansCodeGenerator](https://www.nuget.org/packages/Microsoft.Orleans.OrleansCodeGenerator/) nuget package manually when using in .NET Standard project.  
See [Orleans Code Generation](http://dotnet.github.io/orleans/Documentation/grains/code_generation.html) for detail.
2. If you use the so-called "[PublishTrimmed](https://aka.ms/dotnet-illink)" in .net core 3.0 or above, be sure to add associated 3rd party libs (e.q. [Orleans.Providers.MongoDB](https://www.nuget.org/packages/Orleans.Providers.MongoDB/), [Microsoft.Orleans.Clustering.AdoNet](https://www.nuget.org/packages/Microsoft.Orleans.Clustering.AdoNet/), [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/), [MySql.Data](https://www.nuget.org/packages/MySql.Data/)) to  `<TrimmerRootAssembly>` or update entries in `TrimmerRoots.xml`.

See [example](./example) projects for usage.
