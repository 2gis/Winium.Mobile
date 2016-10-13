# coding: utf-8
import os

BASE_DIR = os.path.dirname(os.path.dirname(__file__))
CONFIG_IDENTIFIER = 'Release' if os.getenv('REMOTE_RUN') else 'Debug'
AUT_PATH = r"..\TestApp.Silverlight\Bin\{0}\TestApp_{0}_AnyCPU.xap".format(CONFIG_IDENTIFIER)

DESIRED_CAPABILITIES = {
    "deviceName": "Emulator 8.1",
    "app": os.path.abspath(os.path.join(BASE_DIR, AUT_PATH)),
}
