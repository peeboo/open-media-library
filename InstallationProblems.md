# Resolving Installation Problems: #

**The installer says "The wizard has detected a previous installation of OML.  Press next to begin checking the database" when trying to install**

This is normal behaviour when upgrading to a newer version or reinstalling but in some cases when a previous installation went wrong or a completely clean installation is required look for a file called `settings.xml` under `C:\ProgramData\OpenMediaLibrary\` and rename it to `settings.bak`. Try to reinstall.


**The SQL installer fails without giving a clue as to why**

SQL puts a log folder under `C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Log` for every installation attempt and a `Summary.txt` log file for the last attempt. If the `Summary.txt` does not suggest the problem find the latest folder by sorting on Date Modified. Inside that folder will be many `Detail...txt` files which hopefully suggest in more detail the cause of the problem.