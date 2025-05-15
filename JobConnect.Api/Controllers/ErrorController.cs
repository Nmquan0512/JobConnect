using Microsoft.AspNetCore.Mvc;

namespace JobConnect.Api.Controllers
{
    [ApiController]
    [Route("api/error")]
    public class ErrorController : ControllerBase
    {
        [HttpGet("test-server-error")]
        public IActionResult TestServerError()
        {
            throw new Exception("Đây là lỗi hệ thống giả lập");
        }

        [HttpGet("test-not-found")]
        public IActionResult TestNotFound()
        {
            throw new KeyNotFoundException("Không tìm thấy tài nguyên");
        }

        [HttpGet("test-business-error")]
        public IActionResult TestBusinessError()
        {
            throw new ArgumentException("Lỗi nghiệp vụ: Dữ liệu không hợp lệ");
        }

        [HttpGet("test-unauthorized")]
        public IActionResult TestUnauthorized()
        {
            throw new UnauthorizedAccessException("Chưa xác thực");
        }
    }
}
