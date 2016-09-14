@echo off
echo Creating nuget package...
set solutionDir=%1
set nuget=%solutionDir%Tools\nuget.exe
set projDir=%solutionDir%UtilJsonApiSerializer\
set outputDir=%projDir%bin\Release

%nuget% pack "%projDir%UtilJsonApiSerializer.csproj" -IncludeReferencedProjects -OutputDirectory "%outputDir%" -Prop Configuration=Release -Verbosity detailed
