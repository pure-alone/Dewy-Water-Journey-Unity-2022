public static class DewyContent
{
    public readonly struct PageCopy
    {
        public readonly string Brand, Kicker, Title, Body, Instruction, Learning;
        public PageCopy(string brand, string kicker, string title, string body, string instruction = "", string learning = "")
        { Brand = brand; Kicker = kicker; Title = title; Body = body; Instruction = instruction; Learning = learning; }
    }

    public static readonly PageCopy Home = new PageCopy(
        "Dewy's Water Journey", "Interactive Storybook", "Welcome to Dewy's journey",
        "Meet Dewy, a tiny water drop about to travel through ocean, sky, forest, groundwater, a mountain stream and river before finding the way back to the sea.");

    public static readonly PageCopy Scene1 = new PageCopy(
        "Scene 1 of 6", "The Sunny Ocean", "Warm sunshine lifts Dewy",
        "Dewy is resting in the ocean. As the sun climbs higher, the water becomes warmer and tiny wisps of vapour begin to rise.",
        "Drag: move the sun upward until Dewy begins to rise.", "Evaporation");

    public static readonly PageCopy Scene2 = new PageCopy(
        "Scene 2 of 6", "Cloud Workshop", "Little droplets gather",
        "High in the cooler sky, Dewy meets other droplets. When enough tiny drops come together, they can build a cloud.",
        "Drag: gather all four droplets inside the cloud.", "Condensation");

    public static readonly PageCopy Scene3 = new PageCopy(
        "Scene 3 of 6", "Rainy Forest", "The cloud is ready to rain",
        "The cloud has grown heavy. Below, the forest is dry and waiting for water. Dewy can help bring the plants back to life.",
        "Tap: press the dark cloud four times to release rain.", "Precipitation");

    public static readonly PageCopy Scene4 = new PageCopy(
        "Scene 4 of 6", "Underground Adventure", "Dewy sinks below the forest",
        "Some rainwater soaks into the soil instead of staying on the surface. Help Dewy move around the rocks and reach the groundwater layer before the journey returns toward daylight.",
        "Drag: guide Dewy through the soil and touch each glowing checkpoint.", "Infiltration / Groundwater");

    public static readonly PageCopy Scene5 = new PageCopy(
        "Scene 5 of 6", "The Mountain Stream", "Small flows join together",
        "Dewy emerges from a hillside spring. As water moves downhill, tiny surface flows meet and collect, turning a narrow trickle into a lively mountain stream.",
        "Swipe / Drag: guide Dewy downhill and touch each bright checkpoint as the stream grows.", "Runoff & Collection");

    public static readonly PageCopy Scene6 = new PageCopy(
        "Scene 6 of 6", "River to the Ocean", "Follow the river home",
        "The mountain stream has joined a larger river. Guide Dewy through the bends, past a waterfall and across a lake until the water reaches the ocean again.",
        "Drag: follow the bright river checkpoints all the way to the ocean.", "River Collection / Return");

    public static readonly PageCopy Credits = new PageCopy(
        "Credits", "Journey complete", "Every drop matters",
        "The ocean from the opening page returns here to show that the cycle continues rather than truly ending.");

    public const string HomeHero = "THE WONDERFUL JOURNEY\nOF A LITTLE WATER DROP";
    public const string HomeTagline = "Every drop has a journey, and every drop matters.";
    public const string EndingTitle = "The journey ends where another journey begins.";
    public const string EndingBody = "Dewy is back in the ocean, ready for the water cycle to begin again.";
    public const string ConservationMessage = "Save Water, Protect Every Drop.";
    public const string CreditsTitle = "The Wonderful Journey of a Little Water Drop";
    public const string CreditsAssessment = "Interactive 2D storybook developed for PROG2006 - Designing the User Experience, Assessment 2.";
    public const string CreditsCreator = "Created by: Zhihe Zhang";
    public const string CreditsQuote = "“The journey ends where another journey begins.”\nSave Water, Protect Every Drop.";
    public const string CreditsDetail = "This final interactive storybook refines the Assessment 1 design for mobile users through touch-based navigation, interactive story events, visual feedback, animation and sound.";
}
