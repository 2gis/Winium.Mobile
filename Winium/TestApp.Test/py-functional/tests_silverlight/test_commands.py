# coding: utf-8
import base64

import pytest
from selenium.common.exceptions import NoSuchElementException
from selenium.webdriver.common.by import By

from tests_silverlight import SilverlightTestCase


By.XNAME = 'xname'


class TestGetCommands(SilverlightTestCase):
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
        root = ElementTree.fromstring(source.encode('utf-8'))
        visual_root = next(root.iterfind('*'))
        assert 'System.Windows.Controls.Border' == visual_root.tag

    @pytest.mark.parametrize(("by", "value"), [
        (By.ID, 'MyTextBox'),
        (By.NAME, 'NonUniqueName'),
        (By.CLASS_NAME, 'System.Windows.Controls.TextBox'),
        (By.TAG_NAME, 'System.Windows.Controls.TextBox'),
    ], ids=['by id', 'by name', 'by class name', 'by tag name'])
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
        (By.TAG_NAME, 'System.Windows.Controls.TextBlock', 30),
    ], ids=['by name', 'by class name'])
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
        elements = parent_element.find_elements_by_class_name('System.Windows.Controls.TextBlock')

        assert 25 == len(elements)

    def test_get_element_text(self):
        """
        GET /session/:sessionId/element/:id/text Returns the visible text for the element.
        """
        text = self.driver.find_element_by_id('SetButton').text
        assert "Set 'CARAMBA' text to TextBox" == text

    @pytest.mark.parametrize(("attr_name", "expected_value"), [('Width', '400', )])
    def test_get_element_attribute(self, attr_name, expected_value):
        """
        GET /session/:sessionId/element/:id/attribute/:name Get the value of an element's attribute.
        """
        element = self.driver.find_element_by_id('MyTextBox')
        value = element.get_attribute(attr_name)
        assert expected_value == value

    def test_get_element_attribute_dot_syntax(self):
        element = self.driver.find_element_by_id('SetButton')
        value = element.get_attribute('Background.Color')
        assert '#00FFFFFF' == value

    @pytest.mark.parametrize(("automation_id", "expected_value"), [
        ('MyTextBox', True),
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
        assert {'x': 240, 'y': 269} == location

    def test_get_element_size(self):
        size = self.driver.find_element_by_id('MyTextBox').size
        assert {'height': 100, 'width': 400} == size

    def test_get_element_rect(self):
        rect = self.driver.find_element_by_id('MyTextBox').rect
        assert {'x': 40, 'y': 219, 'height': 100, 'width': 400} == rect

    def test_get_orientation(self):
        """
        GET /session/:sessionId/orientation Get the current browser orientation.
        Note: we lost orientation support in universal driver, atm it always returns portrait
        """
        # TODO: rewrite and parametrize test to test different orientations
        assert 'PORTRAIT' == self.driver.orientation

    @pytest.mark.parametrize(("name", "expected_value"), [
        ('May', True),
        ('June', True),
        ('November', False),
    ])
    def test_is_displayed(self, name, expected_value):
        element = self.driver.find_element_by_name(name)
        assert expected_value == element.is_displayed()

    def test_file_ops(self):
        encoding = 'utf-8'
        with open(__file__, encoding=encoding) as f:
            encoded = base64.b64encode(f.read().encode(encoding)).decode(encoding)
        self.driver.push_file(r"test\sample.dat", encoded)
        data = self.driver.pull_file(r"test\sample.dat")
        assert encoded == data

    def test_execute_script_invoke_method_echo_with_arg(self):
        rv = self.driver.execute_script('mobile: invokeMethod', 'TestApp.AutomationApi', 'Echo', 'blah blah')
        assert 'blah blah' == rv

    def test_execute_script_invoke_method_complex_return_value_no_args(self):
        expected = {u'Date': u'1985-10-21T01:20:00', u'Text': u'Flux', u'Value': 3}
        rv = self.driver.execute_script('mobile: invokeMethod', 'TestApp.AutomationApi', 'ReturnStubState')
        assert expected == rv


class TestBasicInput(SilverlightTestCase):
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

    def test_click_element(self):
        element = self.driver.find_element_by_id('SetButton')
        element.click()
        assert 'CARAMBA' == self.driver.find_element_by_id('MyTextBox').text

    def test_app_bar_item_invoke(self):
        element = self.driver.find_element_by_id('MyTextBox')
        self.driver.execute_script("mobile: invokeAppBarItem", 'iconButton', 1)
        assert 'Button 2' == element.text
        self.driver.execute_script("mobile: invokeAppBarItem", 'menuItem', 0)
        assert 'MenuItem 1' == element.text
