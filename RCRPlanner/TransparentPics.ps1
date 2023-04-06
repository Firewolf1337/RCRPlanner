function Check-Command($cmdname)
{
    return [bool](Get-Command -Name $cmdname -ErrorAction SilentlyContinue)
}

if(Check-Command magick)
{
    Write-Host "Making car backgrounds transparent" -ForegroundColor Green
    Write-Host "`n" -ForegroundColor Green
    $path = "./static/cars/"
    $files = Get-Childitem -Name -Path $path -Filter *.png | Where {$_.PSChildName -notlike "*logo*"}
    $count = 0

    foreach($filename in $files){
        $count++
        if((magick identify -format '%[channels]' $path$filename) -eq "srgb")
        {
            Write-Host "Processing car picture $filename" -ForegroundColor Yellow
            $filenameout = $path + $filename
            magick $path$filename `( `+clone -fx "p{0,0}" `) -compose Difference  -composite  -modulate 100,0  -alpha off  difference.png
            magick difference.png -bordercolor black -border 5 -threshold 8%  -blur 0x3  halo_mask.png
            magick $path$filename -bordercolor white -border 5 halo_mask.png -alpha Off -compose CopyOpacity -composite $filenameout
        }
        else
        {
            Write-Host "Skipping car picture $filename" -ForegroundColor Cyan
        }
    }
    Write-Host "`n" -ForegroundColor Green
    Write-Host "Making track backgrounds transparent" -ForegroundColor Green
    Write-Host "`n" -ForegroundColor Green
    $path = "./static/tracks/"
    $files = Get-Childitem -Name -Path $path -Filter *.png

    foreach($filename in $files)
    {  
        if((magick identify -format '%[channels]' $path$filename) -eq "srgb")
        {
            Write-Host "Processing track picture $filename" -ForegroundColor Yellow
            $filenameout = $path + $filename
            magick $path$filename `( `+clone -fx "p{0,0}" `) -compose Difference  -composite  -modulate 100,0  -alpha off  difference.png
            magick difference.png -bordercolor black -border 5 -threshold 8% halo_mask.png
            magick $path$filename -bordercolor white -border 5 halo_mask.png -alpha Off -compose CopyOpacity -composite $filenameout
        }
                else
        {
            Write-Host "Skipping track picture $filename" -ForegroundColor Cyan
        }
    }
    Remove-item halo_mask.png -ErrorAction SilentlyContinue
    Remove-item difference.png -ErrorAction SilentlyContinue
    Read-Host -Promt "Done. Press enter to close."
}
else
{
    Write-Host "Please install ImageMagick first." -ForegroundColor White -BackgroundColor Red
    Write-Host "https://imagemagick.org/" -ForegroundColor White -BackgroundColor Red
    Read-Host
}
