<#
.SYNOPSIS
  Composites Mod/About/ModIcon.png as a corner badge over Mod/About/Preview.png, CSS-badge style.
.DESCRIPTION
  1. Copies the icon into a 32bppArgb bitmap (LockBits silently drops alpha writes on a 24bpp source)
     and flood-fills its near-black background to transparent, starting only from the border pixels
     (a color-key match anywhere would also blank matching pixels inside the artwork).
  2. Crops to the bounding box of pixels above -AlphaThreshold (trims the dead transparent margin the
     flood fill leaves, same idea as a browser-side trimAlpha()).
  3. Composites it onto a copy of Preview.png as a CSS-style badge: -Width fixes the badge box width,
     height follows the icon's own aspect ratio; -Corner places the box flush in that corner; -TranslatePercent
     is a fraction of the box's OWN size (not the preview's), matching CSS `transform: translate(%, %)`;
     -RotateDegrees rotates around the box's center, after the translation (CSS transform-origin: center).
.EXAMPLE
  ./Make-PreviewBadge.ps1 -Corner BottomLeft -Width 220 -TranslatePercent @(-0.125,0.125) -RotateDegrees 15 `
    -OutFile Preview_with_ModIcon_badge_left.png
#>
param(
    [string]$AboutDir = (Join-Path (Split-Path $PSScriptRoot -Parent) 'Mod/About'),
    [string]$PreviewFile = 'Preview.png',
    [string]$IconFile = 'ModIcon.png',
    [ValidateSet('BottomLeft','BottomRight')][string]$Corner = 'BottomRight',
    [double]$Width = 200,
    [double[]]$TranslatePercent = @(0.30, 0.30),   # x, y — fraction of the badge box's own size, CSS translate()
    [double]$RotateDegrees = 15,
    [int]$AlphaThreshold = 20,
    [int]$BgTolerance = 26,
    [string]$OutFile = 'Preview_with_ModIcon_badge.png',
    [string]$SaveTrimmedIconTo   # if set, writes the background-removed, alpha-trimmed icon here and exits
                                  # (no compositing); this is the asset render-preview.cjs's .icon-badge <img> uses.
)

Add-Type -AssemblyName System.Drawing

function Remove-BorderBackground {
    param([System.Drawing.Bitmap]$Src, [int]$Tolerance)
    $w = $Src.Width; $h = $Src.Height
    $refR = $Src.GetPixel(0,0).R; $refG = $Src.GetPixel(0,0).G; $refB = $Src.GetPixel(0,0).B

    # LockBits with a target format that differs from the source's own (e.g. 24bpp) writes back nothing:
    # always work on a fresh 32bppArgb copy.
    $copy = New-Object System.Drawing.Bitmap($w, $h, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $gg = [System.Drawing.Graphics]::FromImage($copy)
    $gg.DrawImage($Src, 0, 0, $w, $h)
    $gg.Dispose()

    $rect = New-Object System.Drawing.Rectangle(0,0,$w,$h)
    $data = $copy.LockBits($rect, [System.Drawing.Imaging.ImageLockMode]::ReadWrite, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $stride = [int]$data.Stride
    $bytes = New-Object byte[] ($stride*$h)
    [System.Runtime.InteropServices.Marshal]::Copy($data.Scan0, $bytes, 0, $bytes.Length)

    $visited = New-Object bool[] ($w*$h)
    $stack = New-Object System.Collections.Generic.Stack[int]
    for ($x=0; $x -lt $w; $x++) { [void]$stack.Push([int]$x); [void]$stack.Push([int]($x + ($h-1)*$w)) }
    for ($y=0; $y -lt $h; $y++) { [void]$stack.Push([int]($y*$w)); [void]$stack.Push([int](($w-1) + $y*$w)) }

    while ($stack.Count -gt 0) {
        $idx = [int]$stack.Pop()
        if ($idx -lt 0 -or $idx -ge ($w*$h) -or $visited[$idx]) { continue }
        $px = [int]($idx % $w); $py = [int]([Math]::Floor($idx / $w))
        $off = [int]($py*$stride + $px*4)
        if ($off -lt 0 -or ($off+3) -ge $bytes.Length) { continue }
        $b = [int]$bytes[$off]; $g = [int]$bytes[$off+1]; $r = [int]$bytes[$off+2]
        if ([Math]::Abs($r-[int]$refR) -gt $Tolerance -or [Math]::Abs($g-[int]$refG) -gt $Tolerance -or [Math]::Abs($b-[int]$refB) -gt $Tolerance) { continue }
        $visited[$idx] = $true
        $bytes[$off+3] = 0
        if ($px -gt 0) { [void]$stack.Push([int]($idx-1)) }
        if ($px -lt $w-1) { [void]$stack.Push([int]($idx+1)) }
        if ($py -gt 0) { [void]$stack.Push([int]($idx-$w)) }
        if ($py -lt $h-1) { [void]$stack.Push([int]($idx+$w)) }
    }
    [System.Runtime.InteropServices.Marshal]::Copy($bytes, 0, $data.Scan0, $bytes.Length)
    $copy.UnlockBits($data)
    return $copy
}

function Get-TrimmedByAlpha {
    param([System.Drawing.Bitmap]$Src, [int]$Threshold)
    $w = $Src.Width; $h = $Src.Height
    $rect = New-Object System.Drawing.Rectangle(0,0,$w,$h)
    $data = $Src.LockBits($rect, [System.Drawing.Imaging.ImageLockMode]::ReadOnly, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $stride = $data.Stride
    $bytes = New-Object byte[] ($stride*$h)
    [System.Runtime.InteropServices.Marshal]::Copy($data.Scan0, $bytes, 0, $bytes.Length)
    $Src.UnlockBits($data)

    $minX=$w; $minY=$h; $maxX=-1; $maxY=-1
    for ($y=0; $y -lt $h; $y++) {
        for ($x=0; $x -lt $w; $x++) {
            if ($bytes[$y*$stride + $x*4 + 3] -gt $Threshold) {
                if ($x -lt $minX) { $minX = $x }
                if ($y -lt $minY) { $minY = $y }
                if ($x -gt $maxX) { $maxX = $x }
                if ($y -gt $maxY) { $maxY = $y }
            }
        }
    }
    if ($maxX -lt $minX -or $maxY -lt $minY) { return $Src }
    $cw = $maxX - $minX + 1; $ch = $maxY - $minY + 1
    $cropped = New-Object System.Drawing.Bitmap($cw, $ch, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $gc = [System.Drawing.Graphics]::FromImage($cropped)
    $gc.DrawImage($Src, (New-Object System.Drawing.Rectangle(0,0,$cw,$ch)), (New-Object System.Drawing.Rectangle($minX,$minY,$cw,$ch)), [System.Drawing.GraphicsUnit]::Pixel)
    $gc.Dispose()
    return $cropped
}

$previewPath = Join-Path $AboutDir $PreviewFile
$iconPath = Join-Path $AboutDir $IconFile
$outPath = Join-Path $AboutDir $OutFile

$rawIcon = New-Object System.Drawing.Bitmap($iconPath)
$transparent = Remove-BorderBackground -Src $rawIcon -Tolerance $BgTolerance
$rawIcon.Dispose()
$icon = Get-TrimmedByAlpha -Src $transparent -Threshold $AlphaThreshold
if (-not [object]::ReferenceEquals($icon, $transparent)) { $transparent.Dispose() }

if ($SaveTrimmedIconTo) {
    $icon.Save($SaveTrimmedIconTo, [System.Drawing.Imaging.ImageFormat]::Png)
    Write-Output "wrote $SaveTrimmedIconTo ($($icon.Width) x $($icon.Height), trimmed, transparent background)"
    $icon.Dispose()
    return
}

$preview = New-Object System.Drawing.Bitmap($previewPath)
$pw = $preview.Width; $ph = $preview.Height

$badgeW = [double]$Width
$badgeH = $badgeW * $icon.Height / $icon.Width

$out = New-Object System.Drawing.Bitmap($pw, $ph, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($out)
$g.SmoothingMode = 'AntiAlias'
$g.InterpolationMode = 'HighQualityBicubic'
$g.PixelOffsetMode = 'HighQuality'
$g.DrawImage($preview, 0, 0, $pw, $ph)

$boxX = if ($Corner -eq 'BottomLeft') { 0.0 } else { $pw - $badgeW }
$boxY = $ph - $badgeH
$tx = $badgeW * $TranslatePercent[0]
$ty = $badgeH * $TranslatePercent[1]
$centerX = $boxX + $tx + $badgeW/2.0
$centerY = $boxY + $ty + $badgeH/2.0

$state = $g.Save()
$g.TranslateTransform($centerX, $centerY)
$g.RotateTransform($RotateDegrees)
$g.DrawImage($icon, -$badgeW/2.0, -$badgeH/2.0, $badgeW, $badgeH)
$g.Restore($state)

$g.Dispose()
$out.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
$out.Dispose(); $preview.Dispose(); $icon.Dispose()
Write-Output "wrote $outPath ($badgeW x $badgeH badge, corner $Corner, translate $($TranslatePercent -join ','), rotate $RotateDegrees deg)"
