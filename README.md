# BGS_SITREP
 Discord Bot to relay BGS information.

# Configuration
* Run once to create the configuration file under a folder created at the run location named "Resources".
* Edit the newly-created config.json file and populate it with your bot-token and the command prefix you'd like for the bot.
* Restart the bot.

# Available Commands
Remember that all commands must begin with the prefix from the configuration file, e.g., if the command prefix is '$', then the command would be '$sitrep Sol'
* **sitrep *[system]*** : returns all the factions in [system] as well as their state and influence level. If fewer than 7 factions are in a system, a message will be displayed indicating such.
* **traffic *[system]*** : returns the daily/weekly/total traffic in [system].
* **exrep *[system]*** : returns the likely expansion candidate for any particularr system. Must set a faction in the config file. Note: This command can take several seconds.
