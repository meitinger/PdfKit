PdfKit
======


Description
-----------
This utility allows you to

- quickly view PDF documents, (encapsulated) PostScript, and other image files,
- convert image files to PDF documents and vice versa,
- convert (encapsulated) PostScript files to PDF documents and vice versa
  (as well as to other image files),
- combine multiple PDF documents into a single document and
- extract pages from a PDF document.

Its aim is ease of use by supporting drag & drop, providing a simple user
interface, and to offer great performance as well as seamless integration
into *Windows Explorer*.


Requirements
------------
The utility requires [GPL Ghostscript](https://github.com/ArtifexSoftware/ghostpdl-downloads/releases/)
for displaying PDF documents and (Encapsulated) PostScript files, as well as
converting these files to other file formats.
Please create a folder named `lib`, download Ghostscript and place the files
`gsdll32.dll` and `gsdll64.dll` into this folder.

The [PdfSharp](https://www.nuget.org/packages/PDFsharp) library is required as
well, so `PdfSharp.dll` has to be placed in the `lib`. Any localized satellite
assembly (`PdfSharp.resources.dll`) has to be placed into a sub-folder under
`lib`.

(The files are not included in this repository to avoid any licensing issues.)


Translations
------------
So far the software is available in English and German. If you want to
provide your own language, please translate the `*.resx` files and `Verbs.rc`.
Add the culture to the list at the beginning of `Product.wxs` (use `;` as
separator) and the `makefile` (separated by spaces).


Installation
------------
Make sure you have the [Windows Installer Toolset](http://wixtoolset.org/)
installed, open and build `PdfKit.sln` and install the resulting `PdfKit.msi`.
