//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using System.Globalization;
using CuteAnt.Extensions.Logging;

namespace Microsoft.IdentityModel.Logging
{
  /// <summary>
  /// Helper class for logging.
  /// </summary>
  public class LogHelper
  {
    private static readonly ILoggerFactory _loggerFactory;
    public static readonly ILogger Logger;

    static LogHelper()
    {
      _loggerFactory = new LoggerFactory();
      _loggerFactory.AddNLog();
      Logger = _loggerFactory.CreateLogger("Microsoft.IdentityModel.EventSource");
    }

    /// <summary>
    /// Logs an exception using the event source logger and returns new <see cref="ArgumentNullException"/> exception.
    /// </summary>
    /// <param name="argument">argument that is null or empty.</param>
    /// <remarks>LogLevel is set to Error.</remarks>
    public static ArgumentNullException LogArgumentNullException(string argument)
    {
      return LogArgumentException<ArgumentNullException>(LogLevel.Error, argument, "IDX10000: The parameter '{0}' cannot be a 'null' or an empty object.", argument);
    }

    /// <summary>
    /// Logs an exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="message">message to log.</param>
    /// <remarks>LogLevel is set to Error.</remarks>
    public static T LogException<T>(string message) where T : Exception
    {
      return LogException<T>(LogLevel.Error, null, message, null);
    }

    /// <summary>
    /// Logs an argument exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="argumentName">Identifies the argument whose value generated the ArgumentException.</param>
    /// <param name="message">message to log.</param>
    /// <remarks>LogLevel is set to Error.</remarks>
    public static T LogArgumentException<T>(string argumentName, string message) where T : ArgumentException
    {
      return LogArgumentException<T>(LogLevel.Error, argumentName, null, message, null);
    }


    /// <summary>
    /// Logs an exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="format">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <remarks>LogLevel is set to Error.</remarks>
    public static T LogException<T>(string format, params object[] args) where T : Exception
    {
      return LogException<T>(LogLevel.Error, null, format, args);
    }

    /// <summary>
    /// Logs an argument exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="argumentName">Identifies the argument whose value generated the ArgumentException.</param>
    /// <param name="format">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <remarks>LogLevel is set to Error.</remarks>
    public static T LogArgumentException<T>(string argumentName, string format, params object[] args) where T : ArgumentException
    {
      return LogArgumentException<T>(LogLevel.Error, argumentName, null, format, args);
    }

    /// <summary>
    /// Logs an exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="innerException">the inner <see cref="Exception"/> to be added to the outer exception.</param>
    /// <param name="message">message to log.</param>
    /// <remarks>LogLevel is set to Error.</remarks>
    public static T LogException<T>(Exception innerException, string message) where T : Exception
    {
      return LogException<T>(LogLevel.Error, innerException, message, null);
    }

    /// <summary>
    /// Logs an argument exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="argumentName">Identifies the argument whose value generated the ArgumentException.</param>
    /// <param name="innerException">the inner <see cref="Exception"/> to be added to the outer exception.</param>
    /// <param name="message">message to log.</param>
    /// <remarks>LogLevel is set to Error.</remarks>
    public static T LogArgumentException<T>(string argumentName, Exception innerException, string message) where T : ArgumentException
    {
      return LogArgumentException<T>(LogLevel.Error, argumentName, innerException, message, null);
    }

    /// <summary>
    /// Logs an exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="innerException">the inner <see cref="Exception"/> to be added to the outer exception.</param>
    /// <param name="format">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <remarks>LogLevel is set to Error.</remarks>
    public static T LogException<T>(Exception innerException, string format, params object[] args) where T : Exception
    {
      return LogException<T>(LogLevel.Error, innerException, format, args);
    }

    /// <summary>
    /// Logs an argument exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="argumentName">Identifies the argument whose value generated the ArgumentException.</param>
    /// <param name="innerException">the inner <see cref="Exception"/> to be added to the outer exception.</param>
    /// <param name="format">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <remarks>LogLevel is set to Error.</remarks>
    public static T LogArgumentException<T>(string argumentName, Exception innerException, string format, params object[] args) where T : ArgumentException
    {
      return LogArgumentException<T>(LogLevel.Error, argumentName, innerException, format, args);
    }

    /// <summary>
    /// Logs an exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="eventLevel">Identifies the level of an event to be logged.</param>
    /// <param name="message">message to log.</param>
    public static T LogException<T>(LogLevel eventLevel, string message) where T : Exception
    {
      return LogException<T>(eventLevel, null, message, null);
    }

    /// <summary>
    /// Logs an argument exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="eventLevel">Identifies the level of an event to be logged.</param>
    /// <param name="argumentName">Identifies the argument whose value generated the ArgumentException.</param>
    /// <param name="message">message to log.</param>
    public static T LogArgumentException<T>(LogLevel eventLevel, string argumentName, string message) where T : ArgumentException
    {
      return LogArgumentException<T>(eventLevel, argumentName, null, message, null);
    }

    /// <summary>
    /// Logs an exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="eventLevel">Identifies the level of an event to be logged.</param>
    /// <param name="format">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static T LogException<T>(LogLevel eventLevel, string format, params object[] args) where T : Exception
    {
      return LogException<T>(eventLevel, null, format, args);
    }

    /// <summary>
    /// Logs an argument exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="eventLevel">Identifies the level of an event to be logged.</param>
    /// <param name="argumentName">Identifies the argument whose value generated the ArgumentException.</param>
    /// <param name="format">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static T LogArgumentException<T>(LogLevel eventLevel, string argumentName, string format, params object[] args) where T : ArgumentException
    {
      return LogArgumentException<T>(eventLevel, argumentName, null, format, args);
    }

    /// <summary>
    /// Logs an exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="eventLevel">Identifies the level of an event to be logged.</param>
    /// <param name="innerException">the inner <see cref="Exception"/> to be added to the outer exception.</param>
    /// <param name="message">message to log.</param>
    public static T LogException<T>(LogLevel eventLevel, Exception innerException, string message) where T : Exception
    {
      return LogException<T>(eventLevel, innerException, message, null);
    }

    /// <summary>
    /// Logs an argument exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="eventLevel">Identifies the level of an event to be logged.</param>
    /// <param name="argumentName">Identifies the argument whose value generated the ArgumentException.</param>
    /// <param name="innerException">the inner <see cref="Exception"/> to be added to the outer exception.</param>
    /// <param name="message">message to log.</param>
    public static T LogArgumentException<T>(LogLevel eventLevel, string argumentName, Exception innerException, string message) where T : ArgumentException
    {
      return LogArgumentException<T>(eventLevel, argumentName, innerException, message, null);
    }

    /// <summary>
    /// Logs an exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="eventLevel">Identifies the level of an event to be logged.</param>
    /// <param name="innerException">the inner <see cref="Exception"/> to be added to the outer exception.</param>
    /// <param name="format">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static T LogException<T>(LogLevel eventLevel, Exception innerException, string format, params object[] args) where T : Exception
    {
      return LogExceptionImpl<T>(eventLevel, null, innerException, format, args);
    }

    /// <summary>
    /// Logs an argument exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="eventLevel">Identifies the level of an event to be logged.</param>
    /// <param name="argumentName">Identifies the argument whose value generated the ArgumentException.</param>
    /// <param name="innerException">the inner <see cref="Exception"/> to be added to the outer exception.</param>
    /// <param name="format">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static T LogArgumentException<T>(LogLevel eventLevel, string argumentName, Exception innerException, string format, params object[] args) where T : ArgumentException
    {
      return LogExceptionImpl<T>(eventLevel, argumentName, innerException, format, args);
    }

    /// <summary>
    /// Logs an exception using the event source logger.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    public static Exception LogExceptionMessage(Exception exception)
    {
      return LogExceptionMessage(LogLevel.Error, exception);
    }


    /// <summary>
    /// Logs an exception using the event source logger.
    /// </summary>
    /// <param name="eventLevel">Identifies the level of an event to be logged.</param>
    /// <param name="exception">The exception to log.</param>
    public static Exception LogExceptionMessage(LogLevel eventLevel, Exception exception)
    {
      if (Logger.IsEnabled(eventLevel))
      {
        switch (eventLevel)
        {
          case LogLevel.Trace:
            Logger.LogTrace(exception.ToString());
            break;
          case LogLevel.Debug:
            Logger.LogDebug(exception.ToString());
            break;
          case LogLevel.Information:
            Logger.LogInformation(exception.ToString());
            break;
          case LogLevel.Warning:
            Logger.LogWarning(exception.ToString());
            break;
          case LogLevel.Error:
            Logger.LogError(exception.ToString());
            break;
          case LogLevel.Critical:
            Logger.LogCritical(exception.ToString());
            break;
          case LogLevel.None:
          default:
            break;
        }
      }
      return exception;
    }

    /// <summary>
    /// Logs an exception using the event source logger and returns new typed exception.
    /// </summary>
    /// <param name="eventLevel">Identifies the level of an event to be logged.</param>
    /// <param name="argumentName">Identifies the argument whose value generated the ArgumentException.</param>
    /// <param name="innerException">the inner <see cref="Exception"/> to be added to the outer exception.</param>
    /// <param name="format">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    private static T LogExceptionImpl<T>(LogLevel eventLevel, string argumentName, Exception innerException, string format, params object[] args) where T : Exception
    {
      string message = null;

      if (args != null)
        message = string.Format(CultureInfo.InvariantCulture, format, args);
      else
        message = format;

      if (Logger.IsEnabled(eventLevel))
      {
        switch (eventLevel)
        {
          case LogLevel.Trace:
            Logger.LogTrace(innerException, message);
            break;
          case LogLevel.Debug:
            Logger.LogDebug(innerException, message);
            break;
          case LogLevel.Information:
            Logger.LogInformation(innerException, message);
            break;
          case LogLevel.Warning:
            Logger.LogWarning(innerException, message);
            break;
          case LogLevel.Error:
            Logger.LogError(innerException, message);
            break;
          case LogLevel.Critical:
            Logger.LogCritical(innerException, message);
            break;
          case LogLevel.None:
          default:
            break;
        }
      }

      if (innerException != null)
        if (String.IsNullOrEmpty(argumentName))
          return (T)Activator.CreateInstance(typeof(T), message, innerException);
        else
          return (T)Activator.CreateInstance(typeof(T), argumentName, message, innerException);
      else
          if (String.IsNullOrEmpty(argumentName))
        return (T)Activator.CreateInstance(typeof(T), message);
      else
        return (T)Activator.CreateInstance(typeof(T), argumentName, message);
    }

  }
}
