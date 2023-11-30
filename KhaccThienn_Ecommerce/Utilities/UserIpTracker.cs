namespace KhaccThienn_Ecommerce.Utilities
{
    public class UserIpTracker
    {
        private static Dictionary<string, int> userIpCounts = new Dictionary<string, int>();

        public static void TrackUserIp(string ipAddress)
        {
            if (userIpCounts.ContainsKey(ipAddress))
            {
                // Người dùng đã tồn tại trong danh sách, tăng số lần truy cập lên 1
                userIpCounts[ipAddress]++;
            }
            else
            {
                // Thêm người dùng mới vào danh sách với số lần truy cập là 1
                userIpCounts.Add(ipAddress, 1);
            }
        }

        public static int GetUserCount()
        {
            // Trả về tổng số người dùng đang truy cập
            return userIpCounts.Count;
        }

        public static Dictionary<string, int> GetUserIpCounts()
        {
            // Trả về danh sách các địa chỉ IP và số lần truy cập tương ứng
            return userIpCounts;
        }
    }
}
