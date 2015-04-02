# coding: utf-8
import pytest
from selenium.common.exceptions import NoSuchElementException

from selenium.webdriver.common.by import By
from tests import WuaTestCase


class TestGetCommands(WuaTestCase):
    """
    Test GET commands that do not change anything in app, meaning they can all be run in one session.
    """
    def test_get_current_window_handle(self):
        """
        GET /session/:sessionId/window_handle Retrieve the current window handle.
        """
        assert 'current' == self.driver.current_window_handle

    def test_screenshot(self):
        """
        GET /session/:sessionId/screenshot Take a screenshot of the current page.
        """
        assert self.driver.get_screenshot_as_png(), 'Screenshot should not be empty'

    def test_get_window_size(self):
        """
        GET /session/:sessionId/window/:windowHandle/size Get the size of the specified window.
        """
        size = self.driver.get_window_size()
        assert {'height': 800, 'width': 480} == size

    def test_get_page_source(self):
        """
        GET /session/:sessionId/source Get the current page source (as xml).
        """
        from xml.etree import ElementTree

        source = self.driver.page_source
        root = ElementTree.fromstring(source)
        assert 'Windows.UI.Xaml.Controls.Frame' == root.tag
        assert any(root.iterfind('*')), 'Page source should contain at least one children of root frame'

    @pytest.mark.parametrize(("by", "value"), [
        (By.ID, 'MyTextBox'),
        (By.NAME, 'NonUniqueName'),
        (By.CLASS_NAME, 'Windows.UI.Xaml.Controls.TextBox'),
        (By.TAG_NAME, 'Windows.UI.Xaml.Controls.TextBox'),
        (By.ID, 'FirstAppBarButton')
    ], ids=['by id', 'by name', 'by class name', 'by tag name', 'in popup'])
    def test_find_element(self, by, value):
        """
        POST /session/:sessionId/element Search for an element on the page, starting from the document root.
        """
        try:
            self.driver.find_element(by, value)
        except NoSuchElementException as e:
            pytest.fail(e)

    @pytest.mark.parametrize(("by", "value", "expected_count"), [
        (By.NAME, 'NonUniqueName', 2),
        (By.CLASS_NAME, 'Windows.UI.Xaml.Controls.AppBarButton', 3),
        (By.TAG_NAME, 'Windows.UI.Xaml.Controls.TextBlock', 30),
    ], ids=['by name', 'by class name', 'by tag name'])
    def test_find_elements(self, by, value, expected_count):
        """
        POST /session/:sessionId/elements Search for multiple elements on the page, starting from the document root.
        """
        assert expected_count == len(self.driver.find_elements(by, value))

    def test_find_child_element(self):
        """
        POST /session/:sessionId/element/:id/element
        Search for an element on the page, starting from the identified element.
        """
        parent_element = self.driver.find_element_by_class_name('TestApp.MainPage')
        try:
            parent_element.find_element_by_id('MyTextBox')
        except NoSuchElementException as e:
            pytest.fail(e)

    def test_find_child_elements(self):
        """
        POST /session/:sessionId/element/:id/elements
        Search for multiple elements on the page, starting from the identified element.
        """
        parent_element = self.driver.find_element_by_id('MyListBox')
        elements = parent_element.find_elements_by_class_name('Windows.UI.Xaml.Controls.TextBlock')

        assert 21 == len(elements)

    def test_get_element_text(self):
        """
        GET /session/:sessionId/element/:id/text Returns the visible text for the element.
        """
        text = self.driver.find_element_by_id('SetButton').text
        assert "Set 'CARAMBA' text to TextBox" == text

    @pytest.mark.parametrize(("attr_name", "expected_value"), [
        ('Width', '300'),
        ('DesiredSize.Width', '300'),
    ], ids=['simple property', 'nested property'])
    def test_get_element_attribute(self, attr_name, expected_value):
        """
        GET /session/:sessionId/element/:id/attribute/:name Get the value of an element's attribute.
        """
        element = self.driver.find_element_by_id('MyTextBox')
        value = element.get_attribute(attr_name)
        assert expected_value == value

    def test_get_element_attribute_serialized(self):
        """
        GET /session/:sessionId/element/:id/attribute/:name Get the value of an element's attribute.
        Complex properties (i.e. non scalar and non string should be serialized to JSON and returned as string)
        """
        from json import loads
        element = self.driver.find_element_by_id('MyTextBox')
        value = loads(element.get_attribute('DesiredSize'))
        assert isinstance(value, dict)
        assert {'Width', 'Height', 'IsEmpty'} == set(value.keys())

    @pytest.mark.parametrize(("automation_id", "expected_value"), [
        ('MyTextBox', True),
        ('FirstAppBarButton', False)
    ])
    def test_is_element_displayed(self, automation_id, expected_value):
        """
        GET /session/:sessionId/element/:id/displayed Determine if an element is currently displayed.
        """
        is_displayed = self.driver.find_element_by_id(automation_id).is_displayed()
        assert expected_value == is_displayed

    def test_get_element_location(self):
        """
        GET /session/:sessionId/element/:id/location Determine an element's location on the page.
        """
        location = self.driver.find_element_by_id('MyTextBox').location
        assert {'x': 240, 'y': 261} == location

    def test_get_orientation(self):
        """
        GET /session/:sessionId/orientation Get the current browser orientation.
        Note: we lost orientation support in universal driver, atm it always returns portrait
        """
        # TODO: rewrite and parametrize test to test different orientations
        assert 'PORTRAIT' == self.driver.orientation


class TestAlert(WuaTestCase):
    # __shared_session__ = False

    def test_get_alert_text(self):
        """GET /session/:sessionId/alert_text Gets the text of the currently displayed alert"""
        pytest.skip('TODO')

    def test_accept_alert(self):
        """POST /session/:sessionId/accept_alert Accepts the currently displayed alert dialog."""
        pytest.skip('TODO')

    def test_dismiss_alert(self):
        """POST /session/:sessionId/dismiss_alert Dismisses the currently displayed alert dialog."""
        pytest.skip('TODO')


class TestExecuteScript(WuaTestCase):
    """
    Tests for Execute Script command. Supported scripts are listed at
    https://github.com/2gis/windows-universal-app-driver/wiki/Command-Execute-Script
    Tested scripts do affect app interface, but test methods are made in such way that they can be run in one session.
    """
    @pytest.mark.parametrize("command_alias", ["automation: invoke", "automation: InvokePattern.Invoke"])
    def test_automation_invoke(self, command_alias):
        self.driver.find_element_by_id('MyTextBox').send_keys('')
        element = self.driver.find_element_by_id('SetButton')
        self.driver.execute_script(command_alias, element)
        assert 'CARAMBA' == self.driver.find_element_by_id('MyTextBox').text

    @pytest.mark.parametrize("command_alias", ["automation: scroll", "automation: ScrollPattern.Scroll"])
    def test_automation_scroll(self, command_alias):
        list_box = self.driver.find_element_by_id('MyListBox')
        list_item = list_box.find_element_by_name('November')
        start_location = list_item.location
        scroll_info = {"v": "smallIncrement", "count": 10}
        self.driver.execute_script(command_alias, list_box, scroll_info)
        end_location = list_item.location

        assert (end_location['y'] - start_location['y']) < 0

    def test_automation_toggle(self):
        element = self.driver.find_element_by_id("FirstToggleAppBarButton")
        start_state = self.driver.execute_script("automation: TogglePattern.ToggleState", element)
        self.driver.execute_script("automation: TogglePattern.Toggle", element)
        end_state = self.driver.execute_script("automation: TogglePattern.ToggleState", element)

        assert start_state != end_state

    @pytest.mark.parametrize(("attribute", "value"), [('Width', 10, ), ('Background.Opacity', 0, )],
                             ids=["should set basic properties", "should set nested properties"])
    def test_attribute_set(self, attribute, value):
        element = self.driver.find_element_by_id('SetButton')
        self.driver.execute_script("attribute: set", element, attribute, value)
        assert str(value) == element.get_attribute(attribute)


class TestBasicInput(WuaTestCase):
    __shared_session__ = False

    def test_send_keys_to_element(self):
        """
        POST /session/:sessionId/element/:id/value Send a sequence of key strokes to an element.
        TODO: test magic keys
        """
        actual_input = 'Some test string'
        element = self.driver.find_element_by_id('MyTextBox')
        element.send_keys(actual_input)
        assert actual_input == element.text

    def test_back(self):
        self.driver.find_element_by_id('GoAppBarButton').click()
        text_box = self.driver.find_element_by_id('MyTextBox')
        self.driver.back()
        assert 'AppBar Closed' == text_box.text

    def test_click_element(self):
        element = self.driver.find_element_by_id('SetButton')
        element.click()
        assert 'CARAMBA' == self.driver.find_element_by_id('MyTextBox').text


class TestInputChains(WuaTestCase):
    def test_action_chain(self):
        # TODO MouseMoveTo, MouseClick, MouseDown, MouseUp
        pytest.skip('TODO')

    def test_touch_actions(self):
        # TODO TouchSingleTap, TouchScroll, TouchFlick
        pytest.skip('TODO')
