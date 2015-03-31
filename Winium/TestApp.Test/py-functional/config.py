# coding: utf-8
import os

BASE_DIR = os.path.dirname(os.path.dirname(__file__))
APPX_PATH = '..\\TestApp\\TestApp.WindowsPhone\\AppPackages\\TestApp.WindowsPhone_1.0.0.0_AnyCPU_Debug_Test\\' \
            'TestApp.WindowsPhone_1.0.0.0_AnyCPU_Debug.appx'

DESIRED_CAPABILITIES = {
    "app": os.path.abspath(os.path.join(BASE_DIR, APPX_PATH))
    # "debugConnectToRunningApp": True
}
