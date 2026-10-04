import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

def text(path):
    return (ROOT / path).read_text(encoding='utf-8')

class VisualPolishRuntimeTests(unittest.TestCase):
    def test_polish_installs_automatically(self):
        code = text('Assets/Scripts/DewyVisualPolish.cs')
        self.assertIn('RuntimeInitializeOnLoadMethod', code)
        self.assertIn('ApplyPagePolish()', code)

    def test_scene1_uses_single_programmatic_up_arrow(self):
        code = text('Assets/Scripts/DewyVisualPolish.cs')
        self.assertIn('PolishScene1Sun()', code)
        self.assertIn('Dewy_UpArrow', code)
        self.assertIn('GetUpArrowSprite()', code)
        self.assertIn('DewyUI.Place(sun, 163f, 235f, 92f, 92f)', code)
        self.assertNotIn('HeadLeft', code)
        self.assertNotIn('HeadRight', code)

    def test_scene5_matches_mountain_stream_storyboard(self):
        code = text('Assets/Scripts/DewyVisualPolish.cs')
        for token in ['MountainBackdrop', 'HillsideSpring', 'StreamSegment',
                      'TributaryLeft', 'TributaryRight', 'StreamRipple',
                      'UpdateScene5Progress']:
            self.assertIn(token, code)

    def test_scene6_matches_river_to_ocean_storyboard(self):
        code = text('Assets/Scripts/DewyVisualPolish.cs')
        for token in ['RiverBend0', 'RiverBend1', 'WaterfallCurtain',
                      'WaterfallFoam', 'CalmLake', 'OceanHorizon',
                      'UpdateScene6Progress']:
            self.assertIn(token, code)

    def test_guided_routes_and_next_checkpoint_highlight_exist(self):
        code = text('Assets/Scripts/DewyVisualPolish.cs')
        self.assertIn('AddGuidedPath("GroundwaterPath"', code)
        self.assertIn('StreamGuidePath', code)
        self.assertIn('RiverGuidePath', code)
        self.assertIn('HighlightNext(', code)
        self.assertIn('DewyUI.Hex("#FFC94F")', code)

if __name__ == '__main__':
    unittest.main()
