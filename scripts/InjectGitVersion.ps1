# InjectGitVersion.ps1
#
# Set the version in the projects AssemblyInfo.cs file
#

# Get version info from Git, only tags that begin with v[0-9] will be considered
$gitDescribe = git describe --tags --first-parent --match 'v[0-9]*';

# Parse Git version info into semantic pieces
if ($gitDescribe -match '^v([\d+][\.\d+]*)([\-\w][\-\w\d]*)?-(\d+)-g(\w+)$') {
	$gitVersion = $Matches[1];
	$gitLabel = $Matches[2];
	$gitCount = $Matches[3];
	$gitSHA1 = $Matches[4];
	echo "InjectGitVersion.ps1: $gitDescribe [version='$gitVersion' label='$gitLabel' count='$gitCount' commit='$gitSHA1']";
} else {
	if ($gitDescribe -match '^v([\d+][\.\d+]*)([\-\w][\-\w\d]*)?$') {
		$gitVersion = $Matches[1];
		$gitLabel = $Matches[2];
		echo "InjectGitVersion.ps1: $gitDescribe [version='$gitVersion' label='$gitLabel']";
	} else {
		echo "InjectGitVersion.ps1: PARSING FAILED: '$gitDescribe'";
		exit 1;
	}
}

# Define file variables
$currentYear = (Get-Date).year;
$assemblyFile = $args[0] + "\Properties\AssemblyInfo.cs";
$templateFile =  $args[0] + "\Properties\AssemblyInfo_template.cs";

# Read template file, overwrite place holders with git version info
$newAssemblyContent = Get-Content $templateFile |
    %{$_ -replace '\$CURRENTYEAR\$', ($currentYear) } |
    %{$_ -replace '\$FILEVERSION\$', ($gitVersion) } |
    %{$_ -replace '\$INFOVERSION\$', ($gitDescribe) };

# Write AssemblyInfo.cs file only if there are changes
If (-not (Test-Path $assemblyFile) -or ((Compare-Object (Get-Content $assemblyFile) $newAssemblyContent))) {
    echo "InjectGitVersion.ps1: Creating new AssemblyInfo.cs";
    $newAssemblyContent > $assemblyFile;
} else {
	echo "InjectGitVersion.ps1: AssemblyInfo.cs already up-to-date";
}