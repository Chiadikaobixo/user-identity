namespace Transaction_helper
{
    public class TransactionHelper
    {
        public void creditAccount()
        {
            try
            {

            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }

        public void debitAccount()
        {
            try
            {

            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }
    }
}