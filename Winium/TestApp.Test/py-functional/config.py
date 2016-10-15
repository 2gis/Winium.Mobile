# coding: utf-8
import os

CONFIGURATION = os.getenv('CONFIGURATION', 'Debug')
BASE_DIR = os.path.dirname(os.path.dirname(__file__))
SUFFIX = '_Debug' if CONFIGURATION == 'Debug' else ''
APPX_PATH = '..\\TestApp.StoreApps\\TestApp.WindowsPhone\\AppPackages\\TestApp.WindowsPhone_1.0.0.0_AnyCPU{0}_Test\\' \
            'TestApp.WindowsPhone_1.0.0.0_AnyCPU{0}.appx'.format(SUFFIX)

DESIRED_CAPABILITIES = {
    "app": os.path.abspath(os.path.join(BASE_DIR, APPX_PATH)),
    "deviceName": "Emulator 8.1"
    # "debugConnectToRunningApp": True
}
