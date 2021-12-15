using System;

namespace DiScenFw
{
    /// <summary>
    /// Message log severity level.
    /// </summary>
    public enum LogLevel
    {
        Debug = 0, Verbose, Log, Warning, Error, Fatal
    };


    /// <summary>
    /// Delegate for displaying log messages.
    /// </summary>
    /// <param name="severity">Severity level (see LogLevel).</param>
    /// <param name="message">Text of the message to be displayed.</param>
    /// <param name="category">Category name, used to identify the source of the message.</param>
    /// <param name="onConsole">Display the message on console (when available).</param>
    /// <param name="onScreen">Display the message on screen (when available).</param>
    /// <param name="msgTag">Reference identifier used to update existing messages instead of creating a new one.</param>
    public delegate void DisplayMessageAction(
            LogLevel severity, string message, string category,
            bool onConsole, bool onScreen, string msgTag);


    /// <summary>
    /// Delegate for synchronization of an element in the scenario from a scene object.
    /// </summary>
    /// <param name="elemId">Identifier of the scenario element.</param>
    /// <param name="sceneTransform">Local 3D transformation coming from the scene.</param>
    public delegate void SyncElementTransformAction(string elemId,
        ref LocalTransformData sceneTransform);


    /// <summary>
    /// Delegate for synchronization of a scene object from an element in the scenario.
    /// </summary>
    /// <param name="elemId">Identifier of the scenario element.</param>
    /// <param name="scenarioTransform">Local 3D transformation coming from the scenario data.</param>
	public delegate void SyncSceneObjectTransformAction(string elemId,
        ref LocalTransformData scenarioTransform);


    /// <summary>
    /// Delegate for linear interpolation bewtween two transformation of an element in the scenario.
    /// </summary>
    /// <remarks>
    /// Implementations of this delegate must set the transformation parent
    /// to the first transformation parent until trim parameter becomes 1,
    /// then the second transformation parent must be set.
    /// While trim is less than 1 the element transformation must be considered
    /// relative to the parent defined by transform1.
    /// </remarks>
    /// <param name="elemId">Identifier of the scenario element.</param>
    /// <param name="transform1">First reference transformation.</param>
    /// <param name="transform2">Second reference transformation.</param>
    /// <param name="trim">Trimming value between the two transformations (0 to 1).</param>
	public delegate void LerpElementTransformAction(string elemId,
        ref LocalTransformData transform1, ref LocalTransformData transform2, float trim);

}
