namespace TowardAgarioStepOne;
using System.Diagnostics;
using System.Timers;

/// <summary>
/// Author:     Seoin Kim and Gloria Shin
/// Partner:    Seoin Kim and Gloria Shin
/// Date:       17-Apr-2023
/// Course:     CS 3500, University of Utah, School of Computing
/// Copyright:  CS 3500, Gloria Shin, and Seoin Kim - This work may not 
/// be copied for use in Academic Courswork.
/// 
/// We, Seoin Kim and Gloria Shin, certify that we wrote this code from scratch and did not copy it in part or whole from another source. 
/// All references used in the completion of the assignments are cited in my README file.
/// 
/// File Contents
/// 
///     This contains the "global state" of our program, which has the x, y, a radius of a circle,
///     the direction that the circle is moving in, and the width and height of the "window".
///     
/// </summary>

public partial class MainPage : ContentPage
{
    /// <summary>
    ///     True if the GUI is initialized.
    /// </summary>
    public bool initialized = false;

    /// <summary>
    ///     The WorldDrawable field.
    /// </summary>
    public WorldDrawable worldDrawable;

    /// <summary>
    ///     The timestamp of the last frame rendered in the game.
    /// </summary>
    public DateTime lastFrameTime;

    /// <summary>
    ///     The MainPage of ClientGUI.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
        worldDrawable = new();
    }

    /// <summary>
    ///    This method will be called every time the window is resized
    ///    including the first time the window "shows up" on the screen.
    /// </summary>
    /// <param name="width"> the width of the window </param>
    /// <param name="height"> the height of the window </param>
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        Debug.WriteLine($"OnSizeAllocated {width} {height}");

        if (!initialized)
        {
            initialized = true;
            InitializeGameLogic();
        }
    }

    /// <summary>
    ///     Initializes the game logic.
    /// </summary>
    private void InitializeGameLogic()
    {
        // Assign your WorldDrawable to the PlaySurface.Drawable property.
        PlaySurface1.Drawable = worldDrawable;

        // Resize the widget.
        Window.Width = 500;
        Window.Height = 500;

        lastFrameTime = DateTime.Now;
        // The Timer should have a Tick event that calls a method GameStep.
        Timer timer = new(2_000);
        timer.Elapsed += GameStep;
        timer.Start();
    }

    /// <summary>
    /// This method is called repeatedly by a timer to update the game state and refresh the display.
    /// </summary>
    /// <param name="state"> An object containing state information for the timer (not used in this method). </param>
    /// <param name="args"> An object containing event data for the elapsed event (not used in this method). </param>
    void GameStep(object state, ElapsedEventArgs args)
    {
        // Tell the world model to AdvanceGameOneStep.
        worldDrawable.worldModel.AdvanceGameOneStep();

        // Tell the play surface to redraw itself.
        PlaySurface1.Invalidate();

        double fps = (DateTime.Now - lastFrameTime).TotalMilliseconds / 1000;
        lastFrameTime = DateTime.Now;

        // Update the GUI labels to show the current location of the circle and its direction.
        Dispatcher.Dispatch(() =>
        {
            FPS.Text = $"FPS: {fps:F2}";
            CircleCenter.Text = $"Center: {worldDrawable.worldModel.CircleX}, {worldDrawable.worldModel.CircleY}";
            Direction.Text = $"Direction: {worldDrawable.worldModel.CircleDirection}";
        });
    }
}

