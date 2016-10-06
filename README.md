Pdf Kit
=======


Description
-----------
This utility combines multiple PDF documents into a single document and
extracts one or more pages from a single PDF document.
Its aim is to be easy to use by supporting drag & drop and providing a simple
user interface, and to offer great performance as well as seamless integration
into *Windows Explorer*.


Requirements
------------
The following two libraries must be downloaded into the `lib` folder:
- `gsdll32.dll`: [GPL Ghostscript 32bit](https://github.com/ArtifexSoftware/ghostpdl-downloads/releases/)
- `Ghostscript.NET.dll`: [.NET Wrapper](https://github.com/jhabjan/Ghostscript.NET/releases/)

These files are not included in this repository to avoid any licensing issues.


Translations
------------
So far the software is available in English and German. If you want to
provide your own language, please translate the `CombineForm`, `ExtractForm`
and the installer's `wxl` file.


Installation
------------
Build the `PdfKitInstaller.wixproj` using [WiX](http://wixtoolset.org/) and run
the resulting `PdfKit.msi` that matches your system's locale.
