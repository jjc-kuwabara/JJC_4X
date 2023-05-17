$shellPath = Get-Location

$pathArray = @();
$pathArray += Join-Path $shellPath "\Assets\Scripts"
$pathArray += Join-Path $shellPath "\Assets\Scripts\ScriptableObject"
$pathArray += Join-Path $shellPath "\Assets\Materials"
$pathArray += Join-Path $shellPath "\Assets\Textures"
$pathArray += Join-Path $shellPath "\Assets\Prefabs"
$pathArray += Join-Path $shellPath "\Assets\Effects"
$pathArray += Join-Path $shellPath "\Assets\Fonts"
$pathArray += Join-Path $shellPath "\Assets\ProjectAssets"
$pathArray += Join-Path $shellPath "\Assets\Resources"

foreach($path in $pathArray){
    if( Test-Path $path ){

    }else{
        New-Item $path -ItemType Directory
    }
}
