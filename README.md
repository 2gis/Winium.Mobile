
# Capture Winium tests

The Capture specific SystemTest documentation can be found [here.](https://gist.github.com/ole-vegard/05ae46c2d7accd170f9f)

<p align="right">
English description | <a href="README_RU.md">Описание на русском</a>
</p>

# Winium for Store Apps
[![Build Status](https://img.shields.io/jenkins/s/http/opensource-ci.2gis.ru/Winium.StoreApps.svg?style=flat-square)](http://opensource-ci.2gis.ru/job/Winium.StoreApps/)
[![Inner Server NuGet downloads](https://img.shields.io/nuget/dt/Winium.StoreApps.InnerServer.svg?style=flat-square)](https://www.nuget.org/packages/Winium.StoreApps.InnerServer/)
[![Inner Server NuGet version](https://img.shields.io/nuget/v/Winium.StoreApps.InnerServer.svg?style=flat-square)](https://www.nuget.org/packages/Winium.StoreApps.InnerServer/)
[![GitHub release](https://img.shields.io/github/release/2gis/Winium.StoreApps.svg?style=flat-square)](https://github.com/2gis/Winium.StoreApps/releases/)
[![GitHub license](https://img.shields.io/badge/license-MPL 2.0-blue.svg?style=flat-square)](LICENSE)

<p align="center">
<img src="https://raw.githubusercontent.com/2gis/Winium.StoreApps/assets/winium.png" alt="Winium.StoreApps is Selenium Remote WebDriver implementation for automated testing of Windows Store apps">
</p>

Winium.StoreApps is an open source test automation tool for Windows Store apps, tested on emulators (currently it supports only testing of Windows Phone apps).

## Supported Platforms
- Windows Phone 8.1

For Windows Phone 8 Silverlight test automation tool see [Windows Phone Driver](https://github.com/2gis/winphonedriver).
For Windows Desktop (WPF, WinForms) test automation tool see [Winium Desktop](https://github.com/2gis/cruciatus).

## Why Winium?
You have Selenium WebDriver for testing of web apps, Appium for testing of iOS and Android apps. And now you have Selenium-based tools for testing of Windows apps too. What are some of the benefits? As said by Appium:
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

2. In your AUT's source code add following lines to be called on UI thread after visual root is initialized (usually in `MainPageOnLoaded` for vanilla app or `PrepareApplication` if you use `Caliburn.Micro`)

	```cs
	AutomationServer.Instance.InitializeAndStart();
	```

	or (will include driver only for debug build)

	```cs
	#if DEBUG
		AutomationServer.Instance.InitializeAndStart();
	#endif // DEBUG
	```


3. Write your tests using you favorite language. In your tests use `app` [desired capability](https://github.com/2gis/Winium.StoreApps/wiki/Capabilities) to set path to tested app's appx file. Here is python example:
	```python
	# put it in setUp
	self.driver = webdriver.Remote(command_executor='http://localhost:9999',
	                               desired_capabilities={'app': 'C:\\testApp.appx'})
	# put it in test method body
	element = self.driver.find_element_by_id('SetButton')
	element.click()
	assert 'CARAMBA' == self.driver.find_element_by_id('MyTextBox').text
	```
	> Make sure to set `deviceName` capability to `Emulator` if you are using the driver on a system where Visula Studio 2015 or Winodws 10 SDK is installed.

4. Start `Winium.StoreApps.Driver.exe` ([download release from github](https://github.com/2gis/Winium.StoreApps/releases) or build it yourself)

5. Run your tests and watch the magic happening

## Writing tests
Essentially, Winium.StoreApps supports limited subset of [WebDriver JSON Wire Protocol](https://code.google.com/p/selenium/wiki/JsonWireProtocol), which means that you can write tests just like you would write for Selenium or Appium, here are some [docs](http://docs.seleniumhq.org/docs/03_webdriver.jsp).
For test samples look at [our functional tests](Winium/TestApp.Test/py-functional) or [test samples page](https://github.com/2gis/Winium.StoreApps/wiki/Test-Samples).

## Winium.StoreApps vs Winium.StoreApps.CodedUi
[Winium.StoreApps vs Winium.StoreApps.CodedUi](https://github.com/2gis/Winium/wiki/Winium.StoreApps-vs-Winium.StoreApps.CodedUi)

## How it works
Winium.StoreApps consists of two essential parts:

1. **Winium.StoreApps.Driver** implements Selenium Remote WebDriver and listens for JsonWireProtocol commands. It is responsible for launching emulator, deploying AUT, simulating input, forwarding commands to `Winium.StoreApps.InnerServer`, etc.

2. **Winium.StoreApps.InnerServer** (the one that should be embedded into AUT) communicates with `Winium.StoreApps.Driver.exe` and executes different commands, like finding elements, getting or setting text values, properties, etc., inside your application.

<p align="center">
<img src="https://raw.githubusercontent.com/2gis/Winium.StoreApps/assets/winium-storeapps-struct.png" alt="Winium.StoreApps structure">
</p>

## Contributing

Contributions are welcome!

1. Check for open issues or open a fresh issue to start a discussion around a feature idea or a bug.
2. Fork the repository to start making your changes to the master branch (or branch off of it).
3. We recommend to write a test which shows that the bug was fixed or that the feature works as expected.
4. Send a pull request and bug the maintainer until it gets merged and published. :smiley:

## Contact

Have some questions? Found a bug? Create [new issue](https://github.com/2gis/Winium.StoreApps/issues/new) or contact us at n.abalov@2gis.ru

## License

Winium is released under the MPL 2.0 license. See [LICENSE](LICENSE) for details.
