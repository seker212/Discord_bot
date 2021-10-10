from json import load
import logging
import pytz

class Settings():
    def __init__(self) -> None:
        with open('settings.json', 'r') as settings_file:
            settings = load(settings_file)
            self.channel_id = settings['log_channel_id']
            self.timezone = pytz.timezone(settings['timezone'])

            logging.info('Settings read')