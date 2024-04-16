#!/bin/bash
for file in ./Output/*; do
    dotnet nuget push $file --source https://www.nuget.org/api/v2/package -k $1
done