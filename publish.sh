set -e

dotnet pack -o .
dotnet nuget push $(ls | grep nupkg) --api-key $NUGET_API_KEY --source https://api.nuget.org/v3/index.json
ls | grep nupkg | xargs rm -f
