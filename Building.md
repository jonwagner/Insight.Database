Notes for Building Insight
===============================================

Now with Insight 6.0, we DO support .NET Core and no longer support databases that require funky driver installs.
It makes it a lot easier.


# Docker for Test Databases #

You can run all of the supported databases through docker:

	cd insight-docker
	.\docker-env.bat # windows
	source ./docker-env.bat # non-windows
	docker-compose up

It takes a while the first time, so wait until all of the databases boot up fully.

NOTE: `docker-env` will set the `INSIGHT_TEST_HOST` and `INSIGHT_TEST_PASSWORD` environment variables. You'll need them
for any process that is running the tests and needs to connect to the database.

# Command-Line Builds #

On Windows, you can use the powershell scripts in the root folder:

	./build.ps1 build
	./build.ps1 test
	./build.ps1 package

On non-Windows, you can use the `dotnet build` and `dotnet test` command for anything you want to build and run.
You'll want to specify the framework with `-f <framework>` so you don't try to build frameworks that you don't have.

	dotnet build Insight.Database/Insight.Database.csproj -f netstandard2.0
	dotnet test Insight.Tests/Insight.Tests.csproj -f netcoreapp2.0

# Tagging a Version #

1. Update the version number in SharedConfiguration.csproj.
2. Use `git tag <version>` to mark the changeset.
3. `build package` to make the build.
4. `git push --tags` to push the tags to the server.
