Notes for Building Insight
===============================================

# Docker for Test Databases #

You can run all of the supported databases through docker:

	cd insight-docker
	.\docker-env.bat # windows
	source ./docker-env.sh # non-windows
	docker-compose up

It takes a while the first time, so wait until all of the databases boot up fully.

NOTE: `docker-env` will set the `INSIGHT_TEST_HOST` and `INSIGHT_TEST_PASSWORD` environment variables. You'll need them
for any process that is running the tests and needs to connect to the database.

# Command-Line Builds #

Use: ./build.sh

	[default] builds everything
	-t        tests everything
	-p        packages everything

# Tagging a Version #

1. Update the version number in SharedConfiguration.csproj.
2. Use `git tag <version>` to mark the changeset.
3. `build package` to make the build.
4. `git push --tags` to push the tags to the server.
