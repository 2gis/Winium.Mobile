# coding: utf-8
import os

CONFIGURATION = os.getenv('CONFIGURATION', 'Debug')
BASE_DIR = os.path.dirname(os.path.dirname(__file__))
AUT_PATH = r"..\TestApp.Silverlight\Bin\{0}\TestApp_{0}_AnyCPU.xap".format(CONFIGURATION)

DESIRED_CAPABILITIES = {
    "deviceName": "Emulator 8.1",
    "app": os.path.abspath(os.path.join(BASE_DIR, AUT_PATH)),
    # "debugConnectToRunningApp": True,
}
