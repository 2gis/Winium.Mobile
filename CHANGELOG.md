# Change Log

## v1.7.1
- Fix IsDisplayed for Silverlight. Driver considired elements invisible when container size was (0, 0), which is notm for some containers like canvas. (Thanks to @bayandin)
- Fix links to SDK in READMEs. (Thanks to @sargissargsyan)
- Fix possible `Element does not support ... control pattern interface.`
- Add `ExecuteScript` `mobile: invokeMethod` support to StoreApps. Lets you execute any public static method in AUT from your tests. See `py-functional` tests for usage example.
- Add `ExecuteScript` `automation` sub-commands support. See [wiki](https://github.com/2gis/Winium.Mobile/wiki/Command-Execute-Script#use-automationpeerspatterninterface-on-element). Fixes https://github.com/2gis/winphonedriver/issues/83

## v1.7.0
- Fix driver crashing on empty incoming request
- Add Winium.Silverlight.InnerServer for Windows Phone 8.1 Silverlight apps (from https://github.com/2gis/winphonedriver) (kudos to [Badoo Development](https://github.com/badoo)) #164
- Add Clear command support #160
- Add PushFile and PullFile commands (see tests or Appium bindings for usage) #140


## v1.6.2
- Fix `PageSource` failing to serialize nested classes


## v1.6.1
- Fix `GetElementAttribute` not returning some Dependency Properties.


## v1.6.0
- Add path expansion for files capability to make it easier to deploy folders to app's local storage #128
- Add setting to limit access to Automation, Dependency or CLR properties for `GetElementAttribute` command #120
- Add setting to serialize `Enums` by name, not value for `GetElementAttribute` command #120
- Change the way Automation Properties names are handled by `GetElementAttribute` command #123


## v1.5.0
- Fix crash on bad JSON request body
- Fix not reading http request body to end in certain cases
- Increase default ping timeout
- Add `--ping-timeout` option and `pingTimeout` desired capability to set ping timeout
- Add `noFallback` desired capability to prevent driver from trying to connect to `9998` port
- Add support for deployment of UWP apps (Windows 10 apps)
- Fix error when `debugConnectToRunningApp` is set to `true`
- Add support for accessing `AutomationProperties` with `GetElementAttribute` command


## v1.4.0
- Fix `Remote Procedure Call Failed` when trying to launch app under test by retrying it
- Fix `sessionId` being `null` in response for some commands (thanks to [@tkurnosova](https://github.com/tkurnosova))
- Add support for deployment of dependenecies (thanks to [@ole-vegard](https://github.com/ole-vegard))
- Add `IsElementEnabled` command
- Fix socket interrupt handling
- Add Selenium Grid support and autoregistering Winium as Selenium Grid node ([Running tests in parallel](https://github.com/2gis/Winium.StoreApps/wiki/Running-tests-in-parallel))
- Fix sessionId not being unique
- Add `--bound-device-name` command line option
- Use indented formatting for JSON responses for pretty logging
- Fix logger timestamp format (thanks to [@magnarn](https://github.com/magnarn))


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
