
echo Creating nuget package...
set solutionDir=%~1
set nuget=%solutionDir%Tools\nuget.exe
set projDir=%solutionDir%NJsonApi\
set outputDir=%projDir%bin\Release
echo "%nuget%" pack "%projDir%NJsonApi.csproj" -IncludeReferencedProjects -OutputDirectory "%outputDir%" -Prop Configuration=Release -Verbosity detailed
"%nuget%" pack "%projDir%NJsonApi.csproj" -IncludeReferencedProjects -OutputDirectory "%outputDir%" -Prop Configuration=Release -Verbosity detailed