namespace EverlandApi.Core.Models
{
    public enum ApiErrorCode
    {
        #region Generic errors
        /// <summary>
        /// Indicates a non-descriptive error.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the request is malformed. (For example, an invalid body.)
        /// </summary>
        InvalidRequest,

        /// <summary>
        /// Indicates that an unhandled exception was thrown.
        /// </summary>
        UnhandledException,

        /// <summary>
        /// Indicates that there was an error with the database.
        /// </summary>
        DatabaseError,

        /// <summary>
        /// Indicates that authentication failed.
        /// </summary>
        AuthenticationFailed,

        /// <summary>
        /// Indicates that a requested resource could not be found.
        /// </summary>
        NotFound,
        #endregion

        #region Account-specific errors
        /// <summary>
        /// Indicates a generic account creation error.
        /// </summary>
        AccountCreationFailed,

        /// <summary>
        /// Indicates that an account with the provided username already exists in the database.
        /// </summary>
        AccountUsernameInUse,

        /// <summary>
        /// Indicates that an account with the provided email already exists in the database.
        /// </summary>
        AccountEmailInUse
        #endregion
    }
}
