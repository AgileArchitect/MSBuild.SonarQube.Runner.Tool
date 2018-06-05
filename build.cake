#addin "Cake.Git"

var target = Argument("target", "Default");
var package = "MSBuild.SonarQube.Runner.Tool";

var local = BuildSystem.IsLocalBuild;
var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var buildNumber = AppVeyor.Environment.Build.Number;


var branchName = isRunningOnAppVeyor ? EnvironmentVariable("APPVEYOR_REPO_BRANCH") : GitBranchCurrent(DirectoryPath.FromString(".")).FriendlyName;
var isMasterBranch = System.String.Equals("master", branchName, System.StringComparison.OrdinalIgnoreCase);

///////////////////////////////////////////////////////////////////////////////
// VERSION
///////////////////////////////////////////////////////////////////////////////

var version = "4.3.0";
var toolVersion = "4.3.0.1333";
var semVersion = local ? version : (version + string.Concat("+", buildNumber));

Task("Pack")
    .Does(() => {
	    CreateDirectory("nuget");
	    CleanDirectory("nuget");

	    var nuGetPackSettings = new NuGetPackSettings {
                            Id                      = package,
                            Version                 = version,
                            Title                   = package,
                            Authors                 = new[] {"Tom Staijen"},
                            Owners                  = new[] {"Tom Staijen", "cake-contrib"},
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

Task("Publish")
	.IsDependentOn("Pack")
    .WithCriteria(() => isRunningOnAppVeyor)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isMasterBranch)
	.Does(() => {
		
	    var apiKey = EnvironmentVariable("NUGET_API_KEY");

    	if(string.IsNullOrEmpty(apiKey))    
        	throw new InvalidOperationException("Could not resolve Nuget API key.");
		
		var p = "./nuget/" + package + "." + version + ".nupkg";
            
		// Push the package.
		NuGetPush(p, new NuGetPushSettings {
    		Source = "https://www.nuget.org/api/v2/package",
    		ApiKey = apiKey
		});
	});

Task("Update-AppVeyor-Build-Number")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion(semVersion);
});

Task("AppVeyor")
	.IsDependentOn("Update-AppVeyor-Build-Number")
	.IsDependentOn("Publish");

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);    
