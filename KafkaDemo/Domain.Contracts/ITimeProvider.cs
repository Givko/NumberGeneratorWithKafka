namespace Domain.Contracts
{
    public interface ITimeProvider
    {
        /// <summary>
        /// Returns how much miliseconds are the passed minutes.
        /// </summary>
        /// <param name="minutes">The minutes from which we want to get the miliseconds.</param>
        /// <returns></returns>
        int GetMilisecondsForMinutes(int minutes);
    }
}
