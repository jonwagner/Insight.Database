#!/bin/bash

INSIGHT_FOLDER=$(dirname "$0")
OUTPUT_FOLDER=${INSIGHT_FOLDER}/Build/Output
INSIGHT_CONFIGURATION="${INSIGHT_CONFIGURATION:-Release}"
INSIGHT_NOBUILD=

build_insight () {
  dotnet build -c $INSIGHT_CONFIGURATION ./Insight.sln
}

while getopts "cdobtp" opt; do
  case $opt in
    c)
      for folder in ./Insight.Database*; do
        rm -rf $folder/bin
        rm -rf $folder/obj
        dotnet clean $folder/*.csproj || break
      done
      for folder in ./Insight.Tests*; do
        rm -rf $folder/bin
        rm -rf $folder/obj
        dotnet clean $folder/*.csproj || break
      done
      ;;
    d)
      echo "Building DEBUG Mode" >&2
      INSIGHT_CONFIGURATION=Debug
      ;;
    o)
      echo "Running only-mode --no-build" >&2
      INSIGHT_NOBUILD=--no-build
      ;;
    b)
      build_insight
      ;;
    t)
      build_insight
      for folder in ./Insight.Tests*; do
        dotnet test -c $INSIGHT_CONFIGURATION ${INSIGHT_NOBUILD} $folder/*.csproj || break
      done
      ;;
    p)
      echo "Running Package" >&2
      rm -rf ${OUTPUT_FOLDER}
      mkdir ${OUTPUT_FOLDER}
      for folder in ./Insight.Database*; do
        dotnet pack -c $INSIGHT_CONFIGURATION ${INSIGHT_NOBUILD} $folder/*.csproj || break
        cp $folder/bin/Release/*.nupkg ${OUTPUT_FOLDER}
      done
      ;;
    \?)
      echo "Invalid option: -$OPTARG" >&2
      exit 1
      ;;
  esac
done

if (( $OPTIND == 1 )); then
   build_insight
fi