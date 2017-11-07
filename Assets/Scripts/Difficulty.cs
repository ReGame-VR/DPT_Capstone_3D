/// <summary>
/// A static class to represent levels of difficulty in the game.
/// </summary>
public static class Difficulty {
    // the amount to scale the target by (was a sun in 2D environment)
    public static readonly float[] sunScale = new float[4] { 1f, 0.75f, 0.5f, 0.25f };

    // the amount to scale the ball by
    public static readonly float[] ballScale = new float[4] { 1f, 0.75f, 0.5f, 0.4f };

    // the score decays by 1 every scoreDecay frames
    public static readonly int[] scoreDecay = new int[4] { 0, 30, 15, 7 };

    // the delay between trials, in seconds
    public static readonly float[] delay = new float[4] { 3f, 1.5f, 0.5f, 0.25f };

    // the time the user has to throw the ball before it is destroyed (no limit if 0)
    public static readonly float[] throwLim = new float[4] { 0, 0, 1.8f, 1.2f };

    // the amount to scale the ball velocity
    public static readonly float[] velocityScale = new float[4] { 1, 2, 3, 4 };
}
