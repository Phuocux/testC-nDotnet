# C#nDotnet

## Đăng ký và đăng nhập

Ứng dụng dùng kết nối `ConnectionStrings:DefaultConnection` trong `appsettings.json`
để truy cập MySQL. Bảng `webapp1db.account` cần có các cột `id_account`,
`username` và `password`. `id_account` cần tự tăng; `username` phải duy nhất;
cột `password` cần đủ chỗ lưu hash.

Nếu bảng đã tồn tại nhưng chưa có các kiểu dữ liệu/ràng buộc phù hợp, chạy một
lần trong phpMyAdmin (bỏ qua câu lệnh thêm unique key nếu bảng đã có key đó):

```sql
USE webapp1db;

ALTER TABLE account
    MODIFY username VARCHAR(100) NOT NULL,
    MODIFY password VARCHAR(255) NOT NULL;

ALTER TABLE account
    ADD UNIQUE KEY uq_account_username (username);
```

Đăng ký lưu mật khẩu đã băm bằng ASP.NET Identity PasswordHasher; đăng nhập
kiểm tra hash và tạo cookie xác thực. Vì vậy, các mật khẩu cũ lưu dạng văn bản
không thể đăng nhập bằng luồng này; cần đặt lại hoặc chuyển đổi mật khẩu trước.

Chạy ứng dụng bằng `dotnet run` trong thư mục `WebApplication1`.
