# coding: utf-8
import pytest
from appium.webdriver import Remote
from selenium.webdriver.support.wait import WebDriverWait
import config_silverlight as config


class SilverlightTestCase(object):
    """
    If True, then new session is created when test class is being setup,
    i.e. one session is used for all test methods in class.
    If False, then new session is created when test method is being setup,
    i.e. each test method will run in new session.
    """
    __shared_session__ = True

    desired_capabilities = config.DESIRED_CAPABILITIES

    @staticmethod
    def _create_session(o):
        o.driver = Remote(command_executor="http://localhost:9999", desired_capabilities=o.desired_capabilities)

    @staticmethod
    def _destroy_session(o):
        o.driver.quit()

    def setup_method(self, _):
        if not self.__shared_session__:
            SilverlightTestCase._create_session(self)

    def teardown_method(self, _):
        if not self.__shared_session__:
            SilverlightTestCase._destroy_session(self)

    @classmethod
    def setup_class(cls):
        if cls.__shared_session__:
            SilverlightTestCase._create_session(cls)

    @classmethod
    def teardown_class(cls):
        if cls.__shared_session__:
            SilverlightTestCase._destroy_session(cls)

    @pytest.fixture
    def waiter(self):
        return WebDriverWait(self.driver, timeout=5)
