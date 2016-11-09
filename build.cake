var target = Argument("Target", "Pack");
var package = "MSBuild.SonarQube.Runner.Tool";
var version = "1.0.0";
var toolVersion = "2.2.0.24";

Task("Pack")
    .Does(() => {

	    CreateDirectory("nuget");
	    CleanDirectory("nuget");

	    var nuGetPackSettings = new NuGetPackSettings {
                            Id                      = package,
                            Version                 = version,
                            Title                   = package,
                            Authors                 = new[] {"Tom Staijen"},
                            Owners                  = new[] {"Tom Staijen"},
                            Description             = "Nuget tool package for MSBUild.SonarQube.Runner",
                            Summary                 = "Contains the runner with version " + toolVersion,
                            ProjectUrl              = new Uri("https://github.com/AgileArchitect/MSBuild.SonarQube.Runner.Tool"),
                            LicenseUrl              = new Uri("https://github.com/AgileArchitect/MSBuild.SonarQube.Runner.Tool/blob/master/LICENCE"),
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
        
        var source = Argument("source", "");
        var apikey = Argument("apikey", "");

        NuGetPush(package, new NuGetPushSettings {
            Source = source,
            ApiKey = apikey
        });
    });

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);    