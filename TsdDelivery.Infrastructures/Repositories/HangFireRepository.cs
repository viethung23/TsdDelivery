using Microsoft.Data.SqlClient;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Repositories;

namespace TsdDelivery.Infrastructures.Repositories;

public class HangFireRepository : IHangFireRepository
{
    private readonly AppConfiguration _configuration;

    public HangFireRepository(AppConfiguration appConfiguration)
    {
        _configuration = appConfiguration;
    }
    
    public async Task<string> DeleteJob(Guid reservationId, string? methodName)
    {
        try
        {
            var result = "";
            var connection = new SqlConnection(_configuration.DatabaseConnection);
            connection.Open();

            var query = "DELETE FROM HangFire.Job WHERE HangFire.Job.Arguments LIKE '%' + @arguments + '%' AND HangFire.Job.InvocationData LIKE '%' + @methodName + '%'";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@arguments", reservationId.ToString());
            command.Parameters.AddWithValue("@methodName", methodName);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                result = "Xóa thành công.";
            }
            else
            {
                result = "Không tìm thấy dữ liệu để xóa.";
            }

            // Use the connection object for other queries

            // Close the connection when you are done
            connection.Close();
            return result;
        }
        catch (System.Exception ex)
        {
            throw new Exception($"Error at DeleteJob by booking id and method name: Message " + ex.Message);
        }
    }

    public async Task<string> DeleteJob(Guid reservationId)
    {
        try
        {
            var result = "";
            var query = "DELETE FROM HangFire.Job WHERE HangFire.Job.Arguments LIKE '%' + @arguments + '%'";

            using (var connection = new SqlConnection(_configuration.DatabaseConnection))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@arguments", reservationId.ToString());

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        result = "Xóa thành công.";
                    }
                    else
                    {
                        result = "Không tìm thấy dữ liệu để xóa.";
                    }
                }
            }
            return result;
        }
        catch (System.Exception ex)
        {
            throw new Exception($"Error at DeleteJob by Reservation id: Message " + ex.Message);
        }
    }
}