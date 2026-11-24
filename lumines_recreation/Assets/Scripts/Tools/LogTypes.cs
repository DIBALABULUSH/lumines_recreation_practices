namespace Tools
{
    /// <summary>
    /// The LogType enum represents the different types of log messages that can be logged
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// A message that provides information about the state of the application
        /// </summary>
        Information,

        /// <summary>
        /// A message that indicates a potential problem that the user should be aware of
        /// </summary>
        Warning,
        
        /// <summary>
        /// A message that represents a serious error that has occurred
        /// </summary>
        Error,
        
        /// <summary>
        /// A message that represents an exception that has been thrown
        /// </summary>
        Exception
    } 

    /// <summary>
    /// The LogColor enum represents the different colors that a log message can be displayed in
    /// </summary>
    public enum LogColor
    {
        /// <summary>
        /// The default color for log messages
        /// </summary>
        Default,

        /// <summary>
        /// A color for log messages of type Warning
        /// </summary>
        Warning,

        /// <summary>
        /// A color for log messages of type Error
        /// </summary>
        Error,
        
        /// <summary>
        /// A color for log messages of type Exception
        /// </summary>
        Exception,
        
        /// <summary>
        /// A color for log messages that represent a To Do item
        /// </summary>
        ToDo
    }
}