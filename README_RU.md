# Winium для Store Apps
[![Inner Server NuGet downloads](https://img.shields.io/nuget/dt/Winium.StoreApps.InnerServer.svg?style=flat-square)](https://www.nuget.org/packages/Winium.StoreApps.InnerServer/)
[![Inner Server NuGet version](https://img.shields.io/nuget/v/Winium.StoreApps.InnerServer.svg?style=flat-square)](https://www.nuget.org/packages/Winium.StoreApps.InnerServer/)
[![GitHub release](https://img.shields.io/github/release/2gis/Winium.StoreApps.svg?style=flat-square)](https://github.com/2gis/Winium.StoreApps/releases/)
[![GitHub license](https://img.shields.io/badge/license-MPL 2.0-blue.svg?style=flat-square)](LICENSE)

<p align="center">
<img src="https://raw.githubusercontent.com/2gis/Winium.StoreApps/assets/winium.png" alt="Winium.StoreApps это реализация Selenium Remote WebDriver для автоматизации тестирования Windows Store приложений">
</p>

Winium.StoreApps это open-source инструмент для автоматизации Windows Store приложений, тестируемых на эмуляторах (пока поддерживаются только Windows Phone приложения).

## Поддерживаемые платформы
- Windows Phone 8.1

Для автоматизации Windows Phone 8 Silverlight есть [Windows Phone Driver](https://github.com/2gis/winphonedriver).
Для автоматизации Windows Desktop (WPF, WinForms) есть [Winium Desktop](https://github.com/2gis/Winium.Desktop).

## Почему Winium?
Вы имеете Selenium WebDriver для тестирования веб приложений, Appium для тестирования iOS и Android приложений. И теперь у вас есть Selenium-based инструмент для тестирования Windows приложений. В чем преимущества? Вот что говорит Appium:
> - You can write tests with your favorite dev tools using any WebDriver-compatible language such as Java, Objective-C, JavaScript with Node.js (in promise, callback or generator flavors), PHP, Python, Ruby, C#, Clojure, or Perl with the Selenium WebDriver API and language-specific client libraries.
> - You can use any testing framework.

## Требования
* Windows 8 или выше
* Visual Studio 2013 (Update 2 или выше)
* Windows phone 8.1 SDK

Вы можете взять Visual Studio и SDK с сайта Microsoft [здесь](https://dev.windows.com/en-us/develop/download-phone-sdk).

## Быстрый старт
1. Добавить ссылку на `Winium.StoreApps.InnerServer` в проекте тестируемого приложения ([через NuGet пакет](https://www.nuget.org/packages/Winium.StoreApps.InnerServer/) или соберите проект у себя)

2. В тестовом приложении найдите место в коде где корневой элемент визуального дерева приложения `Frame` проинициализирован (обычно в `MainPageOnLoaded` для чистых приложений или в `PrepareApplication`, если вы используете `Caliburn.Micro`)  и добавьте следующий код

	```cs
	AutomationServer.Instance.InitializeAndStart(Frame);
	```

	или (если вы хотите включить драйвер только при debug сборке)

	```cs
	#if DEBUG
		AutomationServer.Instance.InitializeAndStart(Frame);
	#endif // DEBUG
	```

3. Пишите тесты на удобном языке. В тесте используйте `app` [desired capability](https://github.com/2gis/Winium.StoreApps/wiki/Capabilities) для задания пакета (appx) приложения. Это пример на python:
	```python
	# put it in setUp
	self.driver = webdriver.Remote(command_executor='http://localhost:9999',
	                               desired_capabilities={'app': 'C:\\testApp.appx'})
	# put it in test method body
	element = self.driver.find_element_by_id('SetButton')
	element.click()
	assert 'CARAMBA' == self.driver.find_element_by_id('MyTextBox').text
	```

4. Запустите `Winium.StoreApps.Driver.exe` ([загрузить последнюю версию с github](https://github.com/2gis/Winium.StoreApps/releases) или соберите проект у себя)

5. Запустите тесты и балдейте от происходящей магии

## Написание тестов
По сути, Winium.StoreApps поддерживает ограниченное подмножество команд из [WebDriver JSON Wire Protocol](https://code.google.com/p/selenium/wiki/JsonWireProtocol), т.е. вы можете писать ваши тесты также, как если бы вы писали их под Selenium или Appium, см. например [документацию Selenium](http://docs.seleniumhq.org/docs/03_webdriver.jsp).
В качестве примеров можно использовать наши [функциональные тесты](Winium/TestApp.Test/py-functional) или [примеры с wiki](https://github.com/2gis/Winium.StoreApps/wiki/Test-Samples).

## Как это работает
Winium.StoreApps состоит из двух основных частей:

1. **Winium.StoreApps.Driver** реализует Selenium Remote WebDriver и слушает команды в формате JsonWireProtocol. Он отвечает за запуск эмулятора, деплой тестируемого приложения, эмуляцию ввода, перенаправление команд в `Winium.StoreApps.InnerServer`, и т.д.

2. **Winium.StoreApps.InnerServer** (должен быть встроен в тестируемое приложение) взаимодействует с `Winium.StoreApps.Driver.exe` и исполняет различные команды, например поиск элементов, задание и установку текстовых значений, свойств, и т.д.

<p align="center">
<img src="https://raw.githubusercontent.com/2gis/Winium.StoreApps/assets/winium-storeapps-struct.png" alt="Winium.StoreApps structure">
</p>

## Вклад в развитие

Мы открыты для сотрудничества!

1. Проверьте нет ли уже открытого issue или заведите новый issue для обсуждения новой фичи или бага.
2. Форкните репозиторий и начните делать свои изменения в ветке мастер или новой ветке
3. Мы советуем написать тест, который покажет, что баг был починен или что новая фича работает как ожидается.
4. Создайте pull-request и тыкайте в мэнтейнера до тех пор, пока он не примет и не сольет ваши изменения.  :smiley:

## Контакты

Есть вопросы? Нашли ошибку? Создавайте [новое issue](https://github.com/2gis/Winium.StoreApps/issues/new) или пишите n.abalov@2gis.ru

## Лицензия

Winium выпущен под MPL 2.0 лицензией. [Подробности](LICENSE).
