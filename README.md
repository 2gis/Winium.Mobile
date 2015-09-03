
# Capture Winium tests

The Capture Winium / Selenium tests consists of several parts

## system_test.py
---
This is where the induvidual tests functions are implemented. It contains all functions for setting up, executing the various steps of each test and tearing down the test.

It mostly serves as a wrapper around the appium/winium specific parts of the test to provide a platform-independant interface. It enables writing one test for all three platforms. The implementations of these functions is located in `wp_driver.py`

It supports :

##### setUp()
Does everything needed to start a test, including starting the emulator, connecting to it, installing the app on the emulator and starting it.

##### tearDown()

Does everything needed to stop the test

##### click( id )
Clicks on the elment with the id `id`. If no element was found or the element wasn't clickable, the test will fail

##### verify_text( text, id )
Verifies that the text of the element with the id `id` is the same as `text`. Fails if the texts are different

##### verify_text_using_libres_impl( element )
Takes an element ( note `element` is the actual element, not an id ). And verifies that it has the correct value in `strings.csv`.
Test fail if string is not identical

See `string_dictonary` for more info

##### verify_text_using_libres ( id )
Helper function for `verify_text_using_libres_impl`. This function takes an `id`

##### verify_all_using_libres()
Helper function for `verify_text_using_libres_impl`. This function gets all visible elements on the current page and checks them usinga `verify_text_using_libres_impl`

##### verify_unescaped( id )
This function is used to verify escaping. It checks whether the element with the id `id` has any XML escape characters in it text

##### is_displayed( id )
Returns true of the element with the id `id` is visible

##### verify_visible( id )
Does the above check, but fails the test if the item is not visible

##### verify_not_visible( id )
Does the `is_displayed` check, but fails the test if the item is visible

##### enter_text( text, id )
Writes the text `text` into the element with the id `id`

### app_fields.py
---
Contains the element ids as constant strings so that the can be refered to by the id wihtout writing the id itself

### app_screens.py
---
Contains functionality for confirming that the various screens are correct using system_test

Note : not an actual test in itself, it's just used by the test to confirm that the page is correct

### test_login.py
---
Currently the only test. It tests the login page in various ways by using system_test and app_screens.py

### context.py
---
Contains a few helper functions for creating correct parts and similar

### lookup_ids.csv
---
A mapping between the element ids and the string ids. Used by string dictionary to check strings

### string_dictionary.py
---
A dictonary that can be used to get the correct string for a given element id. It uses the `lookup_ids.csv` and `strings.csv` to look up strings based on the element id.

It can be configured to look up strings in any supported language, it defaults to English

##### look_up( id )
Looks up a the element with the id `id` and returns the corresponding string

##### find_string_key( id )
Finds the strings.csv key for the element with the id `id`

##### get_from_key( key )
Returs the string for a the key `key`

Note : `key` is the strings.csv id, not the element id. It is meant to be used with the find_string_key

### wp_driver.py
---
The winium specific implementation of system_test.py. It deals with everything that concerns the phone / emulator we are testing on

##### start()
Starts the driver, which is the windows equivelent of Appium. It also tells the driver about the dependency ( VisualC++ )

##### connect()
Connects to the driver and tells it to start the emulator and install + start the app.

##### disconnect()
Disconnects from the driver which tells the emulator to stop

##### stop()
Disconnects from the server and kills the driver.exe

##### find_item( id )
Returns an item with the id, `id`

##### find_all_items()
Returns all visible elements of the types TextBlock, Button, HyperlinkButton and PasswordBox.

Can be extended to support more control types

##### has_item( id )
Verifies that an item with the id `id` is present and reachable

##### hide_keyboard()
What the signature said

##### click( id )
Hides keyboard and tries to click the item with the id `id`

It needs to hide the keyboard first because if we don't, we could end up pressing the keyboard instead of the actual button

##### find_last_appx_file()
Used to find the appx file with the highest version number so that we can have multiple appx files without having to delete the old ones or specify an explicit path

## Step by step
---

### 1. Building appx package
In visual studio, right click on the project  `Capture.UniWin.WindowsPhone` and selecet `Store -> Create App Package` and press `Next`

From the bottom left menu, selct `x86` and `Debug(x86)`

Click `Create`

The package will now be created and placed in `[CAPTURE ROOT]\Capture.UniWin\Capture.UniWin.WindowsPhone\bin`

### 2. Starting the tests
Open a terminal and navigate to `[CAPTURE ROOT]\SystemTest`.

Run `py -3 -m unitttest discover` ( you might have to swap `py -3` with `python` or similar. The `-3` parameter is not mandatory. )

### 3. Sit back and watch the magic
The test will now start the winium server. Which in turns will start the emulator, install the app and so on. The test will also look for the latest appx package ( the one with the highest version number ) and install that along with the dependencies. 

Finally when all is set up, the tests defined in `test_login.py` ( and any other unit tests ) will run. The test will not stop until all test steps are done, even if there have been failures. You have to wait in order to get feedback ( though an option should be added here so that we don't always have to wait )


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
