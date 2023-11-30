#####################
# Build stage
#####################
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

WORKDIR /build

# The following arguments are set locally
ARG primaryBuildDirectory=./Util-JsonApiSerializer.Common.NetCore
ARG primaryLinkedCodeDirectory=./Util-JsonApiSerializer
ARG primaryProjectName=Util-JsonApiSerializer.Common.NetCore.csproj
ARG primaryCoreProject=$primaryBuildDirectory/$primaryProjectName





ARG releaseType=ReleaseCore

#The following arguments are passed in from the build script
ARG branchName=master

# Copy the project File and code
COPY $primaryBuildDirectory $primaryBuildDirectory
# because there are linked files we need to copy these as well
COPY $primaryLinkedCodeDirectory $primaryLinkedCodeDirectory

#Log the branch name
Run echo "Branch name is $branchName"

# Bring either the nuget from the docker run context or the passed in arg
# This will bust the build cache of the file is different b/c its the ADD command and it's smart.
# Setting the variable to this default will allow the dockerfile to work
# locally within FL network as it always has
ARG nugetconfig=NuGet.config
ADD $nugetconfig NuGet.config


# Restore the packages w/ the appropriate NuGet.config
RUN dotnet restore "$primaryCoreProject"


# build without a restore

RUN dotnet build "$primaryCoreProject" -c $releaseType --no-restore



#####################
# do this for secondary project Consul
#####################
ARG secondaryBuildDirectory=./source/FL.ServiceDiscovery.Consul.NetCore
ARG secondaryLinkedCodeDirectory=./source/FL.ServiceDiscovery.Consul
ARG secondaryProjectName=FL.ServiceDiscovery.Consul.NetCore.csproj
ARG secondaryCoreProject=$secondaryBuildDirectory/$secondaryProjectName



# Copy the project File and code
COPY $secondaryBuildDirectory $secondaryBuildDirectory
# because there are linked files we need to copy these as well
COPY $secondaryLinkedCodeDirectory $secondaryLinkedCodeDirectory


# Restore the packages w/ the appropriate NuGet.config
RUN dotnet restore "$secondaryCoreProject"


# build without a restore
Run dotnet build "$secondaryCoreProject" -c $releaseType --no-restore

# Always delete the NuGet.config file, no matter where it came from.
# This prevents it from appearing in the runtime stage build, thus
# protecting the passed-in secret
RUN rm -f NuGet.config


# Pack
ARG nugetVersion
RUN dotnet pack  "$primaryCoreProject" --output ./nupkgs /p:Version=$nugetVersion -c $releaseType --no-build --no-restore; 

##Pack Secondary
RUN dotnet pack  "$secondaryCoreProject" --output ./nupkgs /p:Version=$nugetVersion -c $releaseType --no-build --no-restore;


# Upload
ARG nugetUri
ARG nugetApiKey
RUN dotnet nuget push ./nupkgs/*.nupkg --source $nugetUri --api-key $nugetApiKey


