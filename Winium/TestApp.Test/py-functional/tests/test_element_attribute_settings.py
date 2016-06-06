# coding: utf-8
from copy import copy

import pytest
from selenium.webdriver import Remote

import config


@pytest.yield_fixture()
def driver(request):
    capabilities = request.param
    winium = Remote(command_executor="http://localhost:9999", desired_capabilities=capabilities)
    yield winium
    winium.quit()


def _enum_as_string_settings_capability(value):
    caps = copy(config.DESIRED_CAPABILITIES)
    caps['commandSettings'] = {'elementAttributeSettings': {'enumAsString': value}}
    return caps


def _access_modifier_settings_capability(value):
    caps = copy(config.DESIRED_CAPABILITIES)
    caps['commandSettings'] = {'elementAttributeSettings': {'accessModifier': value}}
    return caps


class TestElementAttributeCommandSettings(object):
    @pytest.mark.parametrize('driver', [_enum_as_string_settings_capability(False)], indirect=True)
    def test_get_element_attribute_enum_as_value(self, driver):
        element = driver.find_element_by_id('MyTextBox')
        element.get_attribute('DesiredSize')
        value = element.get_attribute('Visibility')
        assert '0' == value

    @pytest.mark.parametrize('driver', [_enum_as_string_settings_capability(True)], indirect=True)
    def test_get_element_attribute_enum_as_string(self, driver):
        element = driver.find_element_by_id('MyTextBox')
        value = element.get_attribute('Visibility')
        assert 'Visible' == value

    @pytest.mark.parametrize('driver', [
        _access_modifier_settings_capability('AutomationProperties'),
        _access_modifier_settings_capability('DependencyProperties'),
        _access_modifier_settings_capability('ClrProperties'),
    ], indirect=True)
    def test_get_element_attribute_access_modifier(self, driver):
        expected = {
            'AutomationProperties': ['UserCtrl', None, None],
            'DependencyProperties': ['UserCtrl', '0', None],
            'ClrProperties': ['UserCtrl', '0', 'Test'],
        }[driver.desired_capabilities['commandSettings']['elementAttributeSettings']['accessModifier']]

        element = driver.find_element_by_id('UserCtrl')

        ap, dp, clr = expected

        assert ap == element.get_attribute('AutomationProperties.AutomationId')
        assert dp == element.get_attribute('Height')
        assert clr == element.get_attribute('ClrProperty')
