var target = Argument("Target", "Pack");
var package = "MSBuild.SonarQube.Runner.Tool";
var version = "2.2.0.24";

Task("Pack")
    .Does(() => {

	    CreateDirectory("nuget");
	    CleanDirectory("nuget");

	    var nuGetPackSettings = new NuGetPackSettings {
                            Id                      = package,
                            Version                 = version,
                            Title                   = package,
                            Authors                 = new[] {"Isatis Health"},
                            Owners                  = new[] {"Isatis Health"},
                            Description             = "Nuget tool package for MSBUild.SonarQube.Runner",
                            Summary                 = "Because the other people forgot to make it",
                            ProjectUrl              = new Uri("http://t.staijen@git.ncontrol.local:8080/scm/tool/msbuild.sonarqube.runner.tool.git"),
                            RequireLicenseAcceptance= false,
                            Symbols                 = false,
                            NoPackageAnalysis       = true,
                            Files                   = new [] 
							{
								new NuSpecContent {Source = string.Format(@"**", package), Target = "tools"}
                            },
                            BasePath                = "./runner",
                            OutputDirectory         = "./nuget"
                        };

    	NuGetPack(nuGetPackSettings);

    });

Task("Push")
    .Does(() => {
        var package = "./nuget/MSBuild.SonarQube.Runner.Tool." + version + ".nupkg";
        
        NuGetPush(package, new NuGetPushSettings {
            Source = "http://teamcity.ncontrol.local:8222/nuget",
            ApiKey = "abcdef"
        });
    });

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);    