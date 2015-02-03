Windows Phone Driver
====================

Selenium Driver for automated testing of Windows Phone 8.1 Silverlight applications.

This repository hosts the code for the Windows Phone driver. You can use it for testing of native Windows Phone 8.1 applications. Currently it implements only limited subset of [WebDriver JSON Wire Protocol](https://code.google.com/p/selenium/wiki/JsonWireProtocol) and supports testing only via an emulator (Windows phone 8.1).

Driver contains of two parts: Outer and Inner. To run tests you will need to add `WindowsPhoneDriver.InnerDriver` to the app you want to test and start `WindowsPhoneDriver.OuterDriver` (Remote WebDriver to send Json Wire Protocol commands to).

Requirements to run tests using Windows Phone driver
---------------------------------------------------

* Windows 8 or higher
* Windows phone 8.1 SDK
* You will also need Visual Studio 2013 with Update 2 or higher to build driver.

Usage
-----
1. Build solution
2. In tested app project, add reference to `WindowsPhoneDriver.InnerDriver`
3. In your app’s source code locate place where `RootFrame` is set (usually in `PrepareApplication` if you use `Caliburn.Micro` or App.xaml.cs for vanilla app) and add
    
    ```cs
    AutomationServer.Instance.InitializeAndStart(RootFrame);
    ```

    or (will include driver only for debug build)
    
    ```cs
    #if DEBUG
        AutomationServer.Instance.InitializeAndStart(RootFrame);
    #endif // DEBUG
    ```
    
    where `RootFrame` is visual root of application.

4. Write your tests using you favorite language. In your test use `app` desired capability to set path to tested app's xap file (python example).
    ```python
    ...
    self.driver = webdriver.Remote(
                command_executor = 'http://localhost:9999',
                desired_capabilities={
                    "app": r"C:\testApp.xap"
                })
    ...
    # find all Textblock elements
    blocks= self.driver.find_elements_by_tag_name("System.Windows.Controls.TextBlock")
    ```
5. Start WindowsPhoneDriver.OuterDriver.exe
6. Run your tests