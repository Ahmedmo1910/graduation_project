import cv2
import numpy as np


class GlassesOverlay:

    WIDTH_FACTOR = 1.08

    @staticmethod
    def overlay(face_image, glasses_png, face_data):

        angle = face_data["angle"]

        nose_bridge = face_data["nose_bridge"]

        face_width = face_data["face_width"]

        glasses_png = GlassesOverlay.ensure_alpha(glasses_png)

        # ? Resize using face width

        glasses_width = int(face_width * GlassesOverlay.WIDTH_FACTOR)

        aspect_ratio = glasses_png.shape[1] / glasses_png.shape[0]

        glasses_height = int(glasses_width / aspect_ratio)

        resized = cv2.resize(
            glasses_png, (glasses_width, glasses_height), interpolation=cv2.INTER_AREA
        )

        # ? Rotate

        rotated = GlassesOverlay.rotate_image(resized, angle)

        rh, rw = rotated.shape[:2]

        # ? Position using nose bridge

        x = int(nose_bridge[0] - (rw / 2))

        y = int(nose_bridge[1] - (rh * 0.42))

        return GlassesOverlay.alpha_blend(face_image, rotated, x, y)

    @staticmethod
    def ensure_alpha(image):

        if image.shape[2] == 4:
            return image

        gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

        _, alpha = cv2.threshold(gray, 250, 255, cv2.THRESH_BINARY_INV)

        b, g, r = cv2.split(image)

        image = cv2.merge([b, g, r, alpha])

        return image

    @staticmethod
    def rotate_image(image, angle):

        h, w = image.shape[:2]

        center = (w // 2, h // 2)

        rotation_matrix = cv2.getRotationMatrix2D(center, angle, 1.0)

        cos = np.abs(rotation_matrix[0, 0])

        sin = np.abs(rotation_matrix[0, 1])

        new_w = int((h * sin) + (w * cos))

        new_h = int((h * cos) + (w * sin))

        rotation_matrix[0, 2] += (new_w / 2) - center[0]

        rotation_matrix[1, 2] += (new_h / 2) - center[1]

        rotated = cv2.warpAffine(
            image,
            rotation_matrix,
            (new_w, new_h),
            flags=cv2.INTER_LINEAR,
            borderMode=cv2.BORDER_CONSTANT,
            borderValue=(0, 0, 0, 0),
        )

        return rotated

    @staticmethod
    def alpha_blend(background, overlay, x, y):

        result = background.copy()

        h, w = overlay.shape[:2]

        if x >= result.shape[1]:
            return result

        if y >= result.shape[0]:
            return result

        x1 = max(0, x)
        y1 = max(0, y)

        x2 = min(result.shape[1], x + w)

        y2 = min(result.shape[0], y + h)

        overlay_x1 = x1 - x
        overlay_y1 = y1 - y

        overlay_x2 = overlay_x1 + (x2 - x1)

        overlay_y2 = overlay_y1 + (y2 - y1)

        if x1 >= x2 or y1 >= y2:
            return result

        overlay_crop = overlay[overlay_y1:overlay_y2, overlay_x1:overlay_x2]

        alpha = overlay_crop[:, :, 3].astype(np.float32) / 255.0

        alpha = np.expand_dims(alpha, axis=2)

        foreground = overlay_crop[:, :, :3].astype(np.float32)

        background_crop = result[y1:y2, x1:x2].astype(np.float32)

        blended = foreground * alpha + background_crop * (1 - alpha)

        result[y1:y2, x1:x2] = blended.astype(np.uint8)

        return result
