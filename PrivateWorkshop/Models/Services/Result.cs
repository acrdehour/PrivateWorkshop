namespace PrivateWorkshop.Models.Services
{
    public class Result
    {
        public bool Succeeded { get; private set; }
        public string? ErrorMessage { get; private set; }

        public static Result Ok()
        {
            return new Result { Succeeded = true };
        }

        public static Result Fail(string errorMessage)
        {
            return new Result
            {
                Succeeded = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
