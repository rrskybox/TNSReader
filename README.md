# TNSReader

Queries Transient Name Server for recent supernovae that supports creation of an user SDB in TheSkyX

Windows 10 Console App written in C# on Visual Studio 2019.  Tested on TSX Pro Win32 and Win64 versions.

Runs a supernova search via URL on the IAU Transient Name Server, then spoofs the data into the same form that it comes from the CBAT (Harvard Recent Supernovae Webpage) copying process, then saves it on the clipboard.  Upon completion, TheSkyX Edit -> Paste Photo loads the clipboard data into a user SDB.  The query is constrained to read just the most recent thirty days of reports

To install, download the TNSReader.zip file in the Publish directory.  Extract and run setup.exe.  The app will launch from the Start Menu under the folder "TSX ToolKit".

