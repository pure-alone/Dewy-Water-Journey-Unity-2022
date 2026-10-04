import hashlib
import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SOURCE_AUDIO = Path('/mnt/data/dewy_src/audio')
AUDIO_NAMES = {
    'bubble.mp3','cloud_workshop.mp3','credits_theme.mp3','evaporation.mp3',
    'forest_complete.mp3','home_theme.mp3','journey_complete.mp3','mountain_stream.mp3',
    'rain.mp3','rainy_forest.mp3','river_ocean.mp3','sparkle.mp3','stream_splash.mp3',
    'sunny_ocean.mp3','ui_click.mp3','underground.mp3','underground_drip.mp3','water_drop.mp3'
}


def text(path):
    return (ROOT / path).read_text(encoding='utf-8')


def sha256(path):
    h = hashlib.sha256()
    with path.open('rb') as f:
        for chunk in iter(lambda: f.read(65536), b''):
            h.update(chunk)
    return h.hexdigest()


class DewyUnityProjectTests(unittest.TestCase):
    def test_required_unity_files_exist(self):
        required = [
            'Packages/manifest.json', 'ProjectSettings/ProjectVersion.txt',
            'ProjectSettings/EditorBuildSettings.asset', 'ProjectSettings/ProjectSettings.asset',
            'Assets/Scenes/Main.unity', 'Assets/Scripts/DewyBootstrap.cs',
            'Assets/Scripts/DewyApp.cs', 'Assets/Scripts/DewyPages.cs',
            'Assets/Scripts/DewyUI.cs', 'Assets/Scripts/DewyAudio.cs',
            'Assets/Scripts/DewyDragHandler.cs', 'Assets/Scripts/DewyContent.cs',
            'Assets/Editor/DewyBuild.cs', 'Assets/WebGLTemplates/Dewy/index.html',
        ]
        for path in required:
            self.assertTrue((ROOT / path).is_file(), path)

    def test_project_is_pinned_to_unity_2022_3_62f2(self):
        version = text('ProjectSettings/ProjectVersion.txt')
        self.assertIn('m_EditorVersion: 2022.3.62f2', version)
        self.assertIn('7670c08855a9', version)

    def test_reference_resolution_is_450_by_900(self):
        combined = '\n'.join([
            text('Assets/Scripts/DewyApp.cs'),
            text('Assets/Editor/DewyBuild.cs'),
            text('Assets/WebGLTemplates/Dewy/index.html'),
        ])
        self.assertRegex(combined, r'450')
        self.assertRegex(combined, r'900')
        self.assertIn('new Vector2(450f, 900f)', combined)
        self.assertIn('defaultWebScreenWidth = 450', combined)
        self.assertIn('defaultWebScreenHeight = 900', combined)

    def test_main_scene_is_registered_and_bootstrapped(self):
        settings = text('ProjectSettings/EditorBuildSettings.asset')
        scene = text('Assets/Scenes/Main.unity')
        meta = text('Assets/Scripts/DewyBootstrap.cs.meta')
        self.assertIn('Assets/Scenes/Main.unity', settings)
        guid = re.search(r'guid: ([0-9a-f]{32})', meta).group(1)
        self.assertIn(guid, scene)
        self.assertIn('m_Name: DewyBootstrap', scene)

    def test_all_source_story_copy_is_present(self):
        content = text('Assets/Scripts/DewyContent.cs')
        phrases = [
            "Welcome to Dewy's journey",
            'Warm sunshine lifts Dewy',
            'Little droplets gather',
            'The cloud is ready to rain',
            'Dewy sinks below the forest',
            'Small flows join together',
            'Follow the river home',
            'Every drop matters',
            'Created by: Zhihe Zhang',
            'Save Water, Protect Every Drop.'
        ]
        for phrase in phrases:
            self.assertIn(phrase, content)

    def test_all_six_interaction_gates_are_implemented(self):
        pages = text('Assets/Scripts/DewyPages.cs')
        expected = [
            'SUN_COMPLETION_Y',
            'dropletsGathered == 4',
            'rainTaps >= 4',
            'GROUNDWATER_CHECKPOINTS',
            'STREAM_CHECKPOINTS',
            'RIVER_CHECKPOINTS',
            'app.SetSceneComplete(true)',
        ]
        for token in expected:
            self.assertIn(token, pages)

    def test_next_button_locking_and_sound_persistence_exist(self):
        app = text('Assets/Scripts/DewyApp.cs')
        audio = text('Assets/Scripts/DewyAudio.cs')
        self.assertIn('SetSceneComplete(false)', app)
        self.assertIn('nextButton.interactable = sceneComplete', app)
        self.assertIn('dewySound', audio)
        self.assertIn('PlayerPrefs', audio)

    def test_webgl_template_has_portrait_canvas(self):
        html = text('Assets/WebGLTemplates/Dewy/index.html')
        css = text('Assets/WebGLTemplates/Dewy/TemplateData/style.css')
        self.assertIn('unity-canvas', html)
        self.assertIn('width: 450px', css)
        self.assertIn('height: 900px', css)

    def test_unity_2022_packages_and_builtin_font_are_compatible(self):
        import json
        manifest = json.loads(text('Packages/manifest.json'))
        self.assertEqual('1.0.0', manifest['dependencies'].get('com.unity.ugui'))
        self.assertEqual('1.0.0', manifest['dependencies'].get('com.unity.modules.audio'))
        ui = text('Assets/Scripts/DewyUI.cs')
        self.assertIn('Arial.ttf', ui)
        self.assertNotIn('LegacyRuntime.ttf', ui)

    def test_audio_module_is_enabled_for_audio_source_compilation(self):
        import json
        manifest = json.loads(text('Packages/manifest.json'))
        self.assertEqual('1.0.0', manifest['dependencies'].get('com.unity.modules.audio'))

    def test_audio_inventory_and_bytes_match_source(self):
        dest = ROOT / 'Assets/Resources/Audio'
        self.assertTrue(dest.is_dir())
        self.assertEqual(AUDIO_NAMES, {p.name for p in dest.glob('*.mp3')})
        self.assertTrue(SOURCE_AUDIO.is_dir())
        for name in sorted(AUDIO_NAMES):
            self.assertEqual(sha256(SOURCE_AUDIO / name), sha256(dest / name), name)

    def test_webgl_template_escapes_unity_string_variables(self):
        html = text('Assets/WebGLTemplates/Dewy/index.html')
        self.assertIn('JSON.stringify(COMPANY_NAME)', html)
        self.assertIn('JSON.stringify(PRODUCT_NAME)', html)
        self.assertIn('JSON.stringify(PRODUCT_VERSION)', html)
        self.assertNotIn("productName: '{{{ PRODUCT_NAME }}}'", html)

    def test_guided_scene_learning_labels_do_not_overlap_status_bar(self):
        app = text('Assets/Scripts/DewyApp.cs')
        ui = text('Assets/Scripts/DewyUI.cs')
        self.assertIn('GUIDED_LEARNING_Y = 500f', app)
        self.assertIn('CurrentPage >= Page.Scene4 && CurrentPage <= Page.Scene6', app)
        self.assertIn('rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -GUIDED_LEARNING_Y)', app)
        self.assertIn('LearningLabel(Transform stage, string value, float y = 548f)', ui)
        self.assertIn('109, y, 200, 42', ui)

    def test_readme_matches_audio_complete_main_branch(self):
        readme = text('README.md')
        self.assertIn('main branch contains all **18 original MP3 files**', readme)
        self.assertNotIn('main` branch intentionally does **not** contain', readme)
        self.assertNotIn('commit the 18 MP3 files', readme)


if __name__ == '__main__':
    unittest.main()
