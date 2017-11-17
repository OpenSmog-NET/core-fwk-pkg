/**
 * File: build.cake
 * Desc: CAKE build system
 * Author: mmisztal1980
 */

#load "build/settings.build.cake"

const string SolutionFile = "OS.Core.sln";

var packages = getProjectsDirs(new string[] {
    //"OS.Core.Queues
    //"OS.Core.ServiceBus"
});

Task(Restore).Does(() => {
    DotNetCoreRestore(SolutionFile, getDotNetCoreRestoreSettings());
}); // Restore

Task(Build)
    .IsDependentOn(Restore)
    .Does(() => {
    Information($"Starting build({configuration}, {platform})");
    DotNetCoreBuild(SolutionFile, getDotNetCoreBuildSettings());
}); // Build

Task(UnitTests)
    .Does(() => {
    forEachPath(unitTests, null, (test) => {
        DotNetCoreTest(test, getDotNetCoreTestSettings(test, UnitTests));
    });
}); // UnitTests

Task(IntegrationTests)
    .Does(() => {
    forEachPath(integrationTests, null, (test) => {
        DotNetCoreTest(test, getDotNetCoreTestSettings(test, IntegrationTests));
    });
}); // IntegrationTests

Task(Pack)
    .WithCriteria(canEmitArtifacts(@branch))
    .Does(() => {
    forEachPath(packages, null, (package) => {
        DotNetCorePack(package, getDotNetCorePackSettings(package, @branch, @buildNumber));
    });
}); // Pack

RunTarget(@target);