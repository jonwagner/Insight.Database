@ECHO OFF

REM set the local environment variables
FOR /f "tokens=*" %%i IN ('docker-machine env default') DO @%%i

REM these variables are read in BaseTest.cs to override localhost and passwords
SET INSIGHT_TEST_HOST=%DOCKER_HOST%
IF "%INSIGHT_TEST_HOST%"=="" SET INSIGHT_TEST_HOST=127.0.0.1
SET INSIGHT_TEST_PASSWORD=Insight!!!Test

ECHO Insight docker environment variables set
