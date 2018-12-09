namespace Domain.Contracts
{
    public interface ITimeProvider
    {
        /// <summary>
        /// Returns how many miliseconds are the passed minutes.
        /// </summary>
        /// <param name="minutes">The minutes from which we want to get the miliseconds.</param>
        /// <returns>How many miliseconds are the passed minutes.</returns>
        int GetMilisecondsForMinutes(int minutes);

        /// <summary>
        /// Returns how many miliseconds are the passed seconds.
        /// </summary>
        /// <param name="seconds">The seconds from which we want to get the miliseconds.</param>
        /// <returns>How many miliseconds are the passed seconds.</returns>
        int GetMilisecondsForSeconds(int seconds);
    }
}
