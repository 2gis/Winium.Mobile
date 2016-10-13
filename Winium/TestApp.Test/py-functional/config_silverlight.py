# coding: utf-8
import os

BUILD_CONFIG = os.environ.get('BUILD_CONFIG', 'Debug')

BASE_DIR = os.path.dirname(os.path.dirname(__file__))
AUT_PATH = r"..\TestApp.Silverlight\Bin\x86\{0}\TestApp_{0}_x86.xap".format(BUILD_CONFIG)

DESIRED_CAPABILITIES = {
    "deviceName": "Emulator 8.1",
    "app": os.path.abspath(os.path.join(BASE_DIR, AUT_PATH)),
}
