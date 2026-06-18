import cv2
import numpy as np

ALLOWED_EXTENSIONS = {".jpg", ".jpeg", ".png", ".webp"}


def read_image(file_storage):
    """
    Read uploaded image safely.

    Supports:
    - JPG
    - JPEG
    - PNG
    - WEBP

    Preserves alpha channel when available.
    """

    if file_storage is None:
        raise ValueError("No file provided")

    filename = file_storage.filename

    if not filename:
        raise ValueError("Empty filename")

    extension = "." + filename.split(".")[-1].lower()

    if extension not in ALLOWED_EXTENSIONS:
        raise ValueError("Unsupported image format. " "Use JPG, JPEG, PNG or WEBP.")

    file_bytes = file_storage.read()

    if len(file_bytes) == 0:
        raise ValueError("Empty file")

    np_buffer = np.frombuffer(file_bytes, dtype=np.uint8)

    image = cv2.imdecode(np_buffer, cv2.IMREAD_UNCHANGED)

    if image is None:
        raise ValueError("Failed to decode image")

    # Handle grayscale images
    if len(image.shape) == 2:

        image = cv2.cvtColor(image, cv2.COLOR_GRAY2BGR)

    return image


def ensure_bgr(image):
    """
    Convert image to BGR if needed.
    """

    if image is None:
        raise ValueError("Image is None")

    if len(image.shape) == 2:

        return cv2.cvtColor(image, cv2.COLOR_GRAY2BGR)

    return image


def ensure_alpha(image):
    """
    Convert BGR image to BGRA if needed.
    """

    if image is None:
        raise ValueError("Image is None")

    if len(image.shape) != 3:
        raise ValueError("Invalid image dimensions")

    if image.shape[2] == 4:
        return image

    if image.shape[2] == 3:

        alpha = np.full((image.shape[0], image.shape[1]), 255, dtype=np.uint8)

        b, g, r = cv2.split(image)

        return cv2.merge([b, g, r, alpha])

    raise ValueError("Unsupported image channels")
