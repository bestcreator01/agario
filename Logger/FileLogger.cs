using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

/// <summary>
/// Author:     Seoin Kim and Gloria Shin
/// Partner:    Seoin Kim and Gloria Shin
/// Date:       14-Apr-2023
/// Course:     CS 3500, University of Utah, School of Computing
/// Copyright:  CS 3500, Gloria Shin, and Seoin Kim - This work may not 
/// be copied for use in Academic Courswork.
/// 
/// We, Seoin Kim and Gloria Shin, certify that we wrote this code from scratch and did not copy it in part or whole from another source. 
/// All references used in the completion of the assignments are cited in my README file.
/// 
/// File Contents
/// 
///     This contains the logging part of the Agario solution.
///     
/// </summary>

namespace Logger
{
    /// <summary>
    /// A custom logger implementation that writes log entries to a file.
    /// </summary>
    public class FileLogger : ILogger<FileLogger>
    {
        /// <summary>
        /// The name of the logger.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// The path to the log file.
        /// </summary>
        private readonly string _fileName;

        /// <summary>
        /// Creates a new instance of the FileLogger class.
        /// </summary>
        /// <param name="name"> The name of the logger. </param>
        public FileLogger(string name)
        {
            _name = name;

            // Get the path to the ApplicationData folder and create the file name.
            _fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
               + Path.DirectorySeparatorChar
               + $"CS3500-{name}.log";
        }

        /// <summary>
        /// Creates a new scope for logging.
        /// </summary>
        /// <typeparam name="TState"> The type of the scope state. </typeparam>
        /// <param name="state"> The scope state. </param>
        /// <returns> An IDisposable that ends the logging scope when disposed. </returns>
        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            // Create a new FileLoggerProvider instance and set its state to the given state.
            var scope = new FileLoggerProvider();

            scope.State = state;

            return scope;
        }

        /// <summary>
        /// Checks if logging is enabled for the given LogLevel.
        /// </summary>
        /// <param name="logLevel">The LogLevel to check.</param>
        /// <returns>True if logging is enabled, false otherwise.</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            // In this example, we enable logging for all log levels.
            // You can customize this behavior as needed for your use case.
            return logLevel != LogLevel.Trace;
        }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState">The type of the log state.</typeparam>
        /// <param name="logLevel">The LogLevel of the log entry.</param>
        /// <param name="eventId">The EventId of the log entry.</param>
        /// <param name="state">The state of the log entry.</param>
        /// <param name="exception">The exception associated with the log entry, if any.</param>
        /// <param name="formatter">The formatter function that creates the log message.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) { return; }

            string logMessage = $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {_name}: {formatter(state, exception)}{Environment.NewLine}";

            try
            {
                // Append the log entry to the log file.
                File.AppendAllText(_fileName, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to write to log file '{_fileName}': {ex.Message}");
            }
        }
    }

    /// <summary>
    /// A custom logger provider that writes log messages to a file.
    /// </summary>
    public class FileLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// A dictionary that stores created logger instances.
        /// </summary>
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();

        /// <summary>
        /// The minimum log level to be written to the file.
        /// </summary>
        private LogLevel _logLevel = LogLevel.Information;

        /// <summary>
        /// Gets or sets the state of the logger provider.
        /// </summary>
        public object State
        {
            get => _logLevel;
            set => _logLevel = (LogLevel)value;
        }

        /// <summary>
        /// Creates a new logger instance with the specified category name.
        /// </summary>
        /// <param name="categoryName">The category name of the logger.</param>
        /// <returns>A new instance of the FileLogger class.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            // Try to get a logger instance from the cache first.
            if (_loggers.TryGetValue(categoryName, out var logger))
            {
                return logger;
            }

            // Create a new file with a unique name based on the category name.
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + $"CS3500-{categoryName}.log";
            File.WriteAllText(filename, string.Empty);

            // Create a new instance of the FileLogger class with the unique file name.
            logger = new FileLogger(filename);

            // Add the new logger instance to the cache.
            _loggers.TryAdd(categoryName, logger);

            return logger;
        }

        /// <summary>
        /// Disposes all created logger instances.
        /// </summary>
        public void Dispose()
        {
            // Dispose all created loggers (if any).
            foreach (var logger in _loggers.Values)
            {
                (logger as IDisposable)?.Dispose();
            }

            // Clear the cache.
            _loggers.Clear();
        }
    }
}