using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TeslaException : Exception
{
    /// <summary>
    /// Create an empty TeslaException
    /// </summary>
    public TeslaException() { }

    /// <summary>
    /// Create a TeslaException from an error message
    /// </summary>
    /// <param name="message">Error message</param>
    public TeslaException(string message) : base(message) { }
}