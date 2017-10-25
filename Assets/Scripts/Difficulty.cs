/// <summary>
/// A static class to represent levels of difficulty in the game.
/// </summary>
public static class Difficulty {
    public static readonly float[] sunScale = new float[4] { 1f, 0.75f, 0.5f, 0.25f };
    public static readonly float[] ballScale = new float[4] { 1f, 0.75f, 0.5f, 0.4f };
    public static readonly int[] scoreDecay = new int[4] { 0, 30, 15, 7 };
    public static readonly float[] delay = new float[4] { 3f, 1.5f, 0.5f, 0.25f };
    
    // NEED TO TRANSLATE VELOCITY UNITS TO UNITY 3D
    public static readonly float[] velocityScale = new float[4] { 1, 2, 3, 4 };
    // public static readonly float[] velocityDev = new float[4] { };
}
