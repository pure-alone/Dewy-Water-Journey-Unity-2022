import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]


def text(path):
    return (ROOT / path).read_text(encoding='utf-8')


class WebGLRuntimeRegressionTests(unittest.TestCase):
    def test_standalone_input_module_has_legacy_input_enabled(self):
        app = text('Assets/Scripts/DewyApp.cs')
        settings = text('ProjectSettings/ProjectSettings.asset')
        self.assertIn('typeof(StandaloneInputModule)', app)
        self.assertIn('activeInputHandler: 0', settings)

    def test_audio_has_listener_and_user_gesture_resume(self):
        app = text('Assets/Scripts/DewyApp.cs')
        audio = text('Assets/Scripts/DewyAudio.cs')
        self.assertIn('AudioListener', audio)
        self.assertIn('NotifyUserGesture', audio)
        self.assertIn('Input.GetMouseButtonDown(0)', app)
        self.assertIn('Audio.NotifyUserGesture()', app)

    def test_home_and_scene1_layout_matches_html_source(self):
        app = text('Assets/Scripts/DewyApp.cs')
        self.assertIn('ApplyHtmlLayoutParity(page)', app)
        self.assertIn('DewyUI.Place(sun, 281f, 88f, 92f, 92f)', app)
        self.assertIn('ray.anchoredPosition = new Vector2(324f, -75f)', app)
        self.assertIn('DewyUI.Place(sun, 163f, 300f, 92f, 92f)', app)
        self.assertIn('DewyUI.Place(dewy, 173f, 418f, 72f, 72f)', app)
        self.assertIn('new Vector2(173f, -250f)', app)

    def test_dewy_character_is_round_and_face_stays_inside_it(self):
        ui = text('Assets/Scripts/DewyUI.cs')
        dewy_method = ui.split('public static Image Dewy', 1)[1].split('public static RectTransform Cloud', 1)[0]
        self.assertIn('Image drop = Circle(name, parent', dewy_method)
        self.assertNotIn('localEulerAngles', dewy_method)
        self.assertIn('RectTransform face = MakeRect("Face", drop.transform)', dewy_method)
        self.assertIn('Circle("EyeL", face', dewy_method)
        self.assertIn('Circle("EyeR", face', dewy_method)
        self.assertIn('Panel("Smile", face', dewy_method)

    def test_music_and_navigation_controls_use_webgl_safe_visible_text(self):
        app = text('Assets/Scripts/DewyApp.cs')
        self.assertIn('"MUSIC ON"', app)
        self.assertIn('"MUSIC OFF"', app)
        self.assertIn('DewyUI.Button("Back", nav.transform, "<"', app)
        self.assertIn('string nextGlyph = page == Page.Credits ? "HOME" : ">";', app)
        self.assertNotIn('"‹"', app)
        self.assertNotIn('"›"', app)
        self.assertNotIn('"♪"', app)

    def test_forward_navigation_only_allows_already_unlocked_pages(self):
        app = text('Assets/Scripts/DewyApp.cs')
        self.assertIn('private Page highestUnlockedPage = Page.Scene1;', app)
        self.assertIn('private bool IsForwardUnlocked(Page page)', app)
        self.assertIn('private void UnlockNextPage()', app)
        self.assertIn('(int)page < (int)highestUnlockedPage', app)
        self.assertIn('if (!IsForwardUnlocked(CurrentPage)) return;', app)
        self.assertIn('UnlockNextPage();', app)


if __name__ == '__main__':
    unittest.main()
