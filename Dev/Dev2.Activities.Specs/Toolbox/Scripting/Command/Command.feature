﻿Feature: Command
	In order to execute command line scripts
	As a Warewolf user
	I want a tool that allows me to execute commands 

Scenario: Execute command 
	Given I have a variable "[[drive]]" with this value "C:\"
	And I have this command script to execute "cmd.exe /c dir [[drive]]"	
	When the command tool is executed
	Then the result of the command tool will be ""

#Scenario: Execute multiple commands like a batch file
#Scenario: Execute a command that requires user interaction like pause
#Scenario: Execute a blank cmd
#Scenario: Execute invalid cmd


