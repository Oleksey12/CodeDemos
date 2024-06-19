import logging
import os


def create_logger(logger, log_file_path=os.path.join("logs", "main.log"), log_level=logging.INFO) -> logging.Logger:
    if (logger.hasHandlers()):
        logger.handlers.clear()
    logger.setLevel(log_level)
    handler = logging.FileHandler(log_file_path, "a+")
    formatter = logging.Formatter("%(name)s %(asctime)s %(levelname)s %(message)s")
    handler.setFormatter(formatter)
    logger.addHandler(handler)
    return logger