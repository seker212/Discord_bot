import logging
import logging.config

logging.config.fileConfig('settings/logging.conf')
logger = logging.getLogger('root')

def getLogger(name) -> logging.Logger:
    return logging.getLogger(name)