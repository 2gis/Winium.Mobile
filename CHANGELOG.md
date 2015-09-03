# Change Log

<!--## Unreleased-->

## v1.3.1
- Fix click command for custom controls


## v1.3.0

- Add dynamic port assigment for InnerServer #39 
- Add `autoLaunch` capability: whether to launch the app automatically. Default `true`
- Add `--version` option to a driver CLI. Fix exit codes
- Add `SetOrientation` command (Note that orientation is preserved between sessions)
- Fix `GetOrientation` command
- Add Emulator VMs caching (i.e. speed up session creation when same emulator is requested)
- Fix Emulator VMs disposing on Driver exit (note that VM will not be closed)
- Fix Driver failing to create a new session if user name is not lower case (bug introduced in 778ca88)


## v1.2.0

- Simplify inclusion of `InnerServer`: call `AutomationServer.Instance.InitializeAndStart();` on UI thread. No need to pass visual root any more.
- Add `LaunchApp`, `CloseApp` commands [#44](https://github.com/2gis/Winium.StoreApps/issues/44)
- Add `GetElementSize`, `GetElementRect` commands
- Add `SubmitElement` command and limited support for `SendKeysToActiveElement` (<kbd>Enter</kbd> key only)
- Add `SendKeys` support for PasswordBox type elements
- Add `automation: IsOffscreen` command to `ExecuteScript`
- Fix visual tree root discovery
- Fix popup child root element is not included in search [#40](https://github.com/2gis/Winium.StoreApps/issues/40)
- Fix alert related commands to work with `ContentDialog`


## v1.1.1

- Add execute script command to show or hide on-screen keyboard
- Add execute script command to return clickable point as determined by Automation API


## v1.1.0

- Add status command
- Change element visibility check algorithm in IsElementDisplayed
- Project configurations and code clean up


## v1.0.1

- Add toggle pattern support for ExecuteScript automation: command.
- Change PageSource to output element's rectangle instead of middle point.
- Add Winium.StoreApps.Inspector (tool to inspect tested app's UI).


## v1.0.0

- First official release.

