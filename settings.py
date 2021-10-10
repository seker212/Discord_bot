from json import load
import pytz

import logger as log
logger = log.getLogger(__name__)

class Settings():
    def __init__(self) -> None:
        with open('settings/settings.json', 'r') as settings_file:
            settings = load(settings_file)
            self.channel_id = settings['log_channel_id']
            self.timezone = pytz.timezone(settings['timezone'])

            logger.info('Settings read')