# coding: utf-8
import os

BASE_DIR = os.path.dirname(os.path.dirname(__file__))
CONFIG_IDENTIFIER = '' if os.getenv('REMOTE_RUN') else '_Debug'
APPX_PATH = '..\\TestApp\\TestApp.WindowsPhone\\AppPackages\\TestApp.WindowsPhone_1.0.0.0_AnyCPU{0}_Test\\' \
            'TestApp.WindowsPhone_1.0.0.0_AnyCPU{0}.appx'.format(CONFIG_IDENTIFIER)

DESIRED_CAPABILITIES = {
    "app": os.path.abspath(os.path.join(BASE_DIR, APPX_PATH))
    # "debugConnectToRunningApp": True
}
