using UnityEngine;

/// <summary>
/// Base interface for bow animation.
/// </summary>
public interface IBowAnimation {

    /// <summary>
    /// Animate bow based on provided draw value.
    /// </summary>
    /// <param name="draw">Current amount of bow draw (0 -> max draw)</param>
    void SetDraw(float draw);
}