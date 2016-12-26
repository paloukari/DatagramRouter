
namespace System.ServiceModel.ChannelPool
{
    /// <summary>
    /// Simple list of constants.
    /// </summary>
    /// <remarks>This should be typically be replaced with a resource file...but later</remarks>
    internal class Constants
    {
        internal const string EXCEPTION_CANNOT_OBTAIN_LOCK_REFILL = "Unable to obtain an exclusive lock to perform a pool refill";
        internal const string EXCEPTION_CANNOT_OBTAIN_LOCK_GETCHANNEL = "Unable to obtain an exclusive lock to retrieve a channel from the pool";
        internal const string EXCEPTION_CANNOT_CREATE_CHANNEL = "Unable to create a communication channel";
        internal const string EXCEPTION_EXCEEDED_MAXIMUM_POOL_SIZE = "Exceeded maximum pool Size. Current maximum is 126.";

        internal const string EXCEPTION_CANNOT_OBTAIN_CHANNEL_FROM_POOL =
            "Unable to obtain a channel from the Channel Pool. Try adjusting the channel pool interval and threshold values.";
        
        internal const int LOCK_ATTEMPT_PERIOD = 5000;   // The number of milliseconds we wait while trying to obtain an exclusive lock for refilling of the pool
    }
}
