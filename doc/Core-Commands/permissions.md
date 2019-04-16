# Permissions System

### Table of Contents
> * [Summary](#summary)
> * [Access Level](#access-level)
> * Core Commands
>    * [Permissions set user](#core-command-permissions-set-user)
>    * [Permissions set role](#core-command-permissions-set-role)
>    * [Permissions get](#core-command-permissions-get)
>    * [Permissions del user](#core-command-permissions-del-user)
>    * [Permissions del user](#core-command-permissions-del-user)

## Summary
The new permission system is designed to be a bit more robust, and organized.

### Access Level:
Users who are not listed in the permission system will be granted normal access. The following access levels are available.
**NB**: Command modules, and command list will remark Access level as `AccessLevels.<AccessLevelName>`.

* `Blacklisted`

   * >Users with this permission set cannot interact with the bot. First attempt will result in a warning message letting them know they are blacklisted. After that, the bot will totally ignore their input.

* `Normal` - Default permission set. 

   * >They can
     >* Use non-restricted commands (module, or custom)

* `CommandManager` - Elevated permission set #1. 

   * >They can
     >* Use restricted custom commands (or module commands requiring `AccessLevels.CommandManager`access level)
     >* Add new custom commands
     >* Delete custom commands
     >* Edit custom commands

* `Administrator` - Elevated permissions set #2. 
   * **NB:** This will grant users most of the same permissions as bot owner. (barring module commands marked with `RequireBotOwner`) Use with caution

   * >They can
     >* Manage commands (Inherited from `CommandManger`)
     >* Alter bot status
     >* Shutdown and restart bot
     >* Manage user permissions
     
*Reminder:
>**All examples will be using `!` as a command prefix. You will specify your own when setting up ModularBOT**

## Core Command: `permissions set user`
Adds a new user (or updates existing user) to the permission system file.

**This command requires `AccessLevels.Administrator`**

Usage: `!permissions set user <@User#1234> <AccessLevelString>` **OR** `!psu <@User#1234> <AccessLevelString>`
* **NB:** You can either mention the user, or type out their `username#tag`.
* `AccessLevelString` does not include `AccessLevels.` Please only use `Blacklisted`, `Normal`, `CommandManager`, or `Administrator`

## Core Command: `permissions set role`
Adds a new role (or updates existing role) to the permission system file.

**This command requires `AccessLevels.Administrator`**

Usage: `!permissions set role <@RoleMention> <AccessLevelString>` **OR** `!psr <@RoleMention> <AccessLevelString>`
* You must be able to mention the role.
* `AccessLevelString` does not include `AccessLevels.` Please only use `Normal`, `CommandManager`, or `Administrator`
* You cannot blacklist a role. Doing so will result in a thrown exception.
>**InvalidOperationException: You can't blacklist an entire role.** As notated in source code:
>```
>//Unfortunately this would cause the warning system to work incorrectly.
>//Example: One user with BL role interacts with bot, gets the warning -> Warning flag resets.
>//         Another user with same BL role (who never got warned before) would NOT get warning.
>```

## Core Command: `permissions get`
Shows information regarding permissions on a specific user.

**This command requires `AccessLevels.Administrator`**

Usage: `!permissions get <@User#1234>` **OR** `!plist <@User#1234>`

## Core Command: `permissions del role`
Removes a role from the permission system.

**This command requires `AccessLevels.Administrator`**

Usage: `!permissions del role <@RoleMention>` **OR** `!pdr <@RoleMention>`

## Core Command: `permissions del user`
Removes a role from the permission system.

**This command requires `AccessLevels.Administrator`**

Usage: `!permissions del user <@User#1234>` **OR** `!pdu <@User#1234>`