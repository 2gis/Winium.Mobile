# Winium for Store Apps
[![Inner Server NuGet downloads](https://img.shields.io/nuget/dt/Winium.StoreApps.InnerServer.svg?style=flat-square)](https://www.nuget.org/packages/Winium.StoreApps.InnerServer/)
[![Inner Server NuGet version](https://img.shields.io/nuget/v/Winium.StoreApps.InnerServer.svg?style=flat-square)](https://www.nuget.org/packages/Winium.StoreApps.InnerServer/)

Winium.StoreApps is an open source test automation tool for Windows Store apps, tested on emulators (currently it supports only testing of Windows Phone apps).

## Supported Platforms
- Windows Phone 8.1

For Windows Phone 8 Silverlight test automation tool see [Windows Phone Driver](https://github.com/2gis/winphonedriver).
For Windows Desktop (WPF, WinForms) test automation tool see [Winium Desktop](https://github.com/2gis/cruciatus).

## Why Winium?
As said by Appium:
> - You can write tests with your favorite dev tools using any WebDriver-compatible language such as Java, Objective-C, JavaScript with Node.js (in promise, callback or generator flavors), PHP, Python, Ruby, C#, Clojure, or Perl with the Selenium WebDriver API and language-specific client libraries.
> - You can use any testing framework.

## Requirements
* Windows 8 or higher
* Visual Studio 2013 with Update 2 or higher
* Windows phone 8.1 SDK

You can get Visual Studio and SDK from Microsoft [here](https://dev.windows.com/en-us/develop/download-phone-sdk).

## Quick Start
**App under test (AUT)** is application that you would like to test.

1. Add reference to `Winium.StoreApps.InnerServer` in AUT project ([install NuGet package](https://www.nuget.org/packages/Winium.StoreApps.InnerServer/) or build project yourself)

2. In your AUT's source code find place where `Frame` is set (usually in `MainPageOnLoaded` for vanilla app or `PrepareApplication` if you use `Caliburn.Micro`)  add

	```cs
	AutomationServer.Instance.InitializeAndStart(Frame);
	```

	or (will include driver only for debug build)

	```cs
	#if DEBUG
		AutomationServer.Instance.InitializeAndStart(Frame);
	#endif // DEBUG
	```

	where `Frame` is visual root of application.

3. Write your tests using you favorite language. In your tests use `app` [desired capability](https://github.com/2gis/Winium.StoreApps/wiki/Capabilities) to set path to tested app's appx file. Here is python example:
	```python
	# put it in setUp
	self.driver = webdriver.Remote(command_executor='http://localhost:9999',
	                               desired_capabilities={'app': 'C:\\testApp.appx'})
	# ut it in test method body
	element = self.driver.find_element_by_id('SetButton')
	element.click()
	assert 'CARAMBA' == self.driver.find_element_by_id('MyTextBox').text
	```

4. Start `Winium.StoreApps.Driver.exe` ([download release from github](https://github.com/2gis/Winium.StoreApps/releases) or build it yourself)

5. Run your tests and watch the magic happening

## Writing tests
Essentially, Winium.StoreApps supports limited subset of [WebDriver JSON Wire Protocol](https://code.google.com/p/selenium/wiki/JsonWireProtocol), which means that you can write tests just like you would write for Selenium or Appium, here are some [docs](http://docs.seleniumhq.org/docs/03_webdriver.jsp).
For test samples look at [our functional tests](https://github.com/2gis/Winium.StoreApps/tree/master/Winium/TestApp.Test/py-functional).


## How it works
Winium.StoreApps consists of two essential parts:

1. **Winium.StoreApps.Driver** implements Selenium Remote WebDriver and listens for JsonWireProtocol commands. It is responsible for launching emulator, deploying AUT, simulating input, forwarding commands to `Winium.StoreApps.InnerServer`, etc.

2. **Winium.StoreApps.InnerServer** (the one that should be embedded into AUT) communicates with `Winium.StoreApps.Driver.exe` and executes different commands, like finding elements, getting or setting text values, properties, etc., inside your application.
