
from mcp.server.fastmcp import FastMCP

mcp = FastMCP(
    name="conversation",
    instructions="""
    Server cung cấp các tính năng phục vụ quá trình hội thoại:
    - Truy vấn ngôn ngữ tự nhiên phục vụ hội thoại
    """,
)

# TOOLS

# PROMPTS
@mcp.prompt()
def conversation_summary_prompt() -> str:
    """
    Tạo một truy vấn ngôn ngữ tự nhiên để tóm tắt hội thoại
    """
    return """
    Dựa vào lịch sử tin nhắn, hãy tóm tắt cuộc hội thoại với người dùng.
    Cụ thể, hãy trình bày các điều sau nếu có một cách ngắn gọn.
    Tuy vậy, không được chỉ đưa ra khái niệm, hãy thêm nội dung đã giải thích:
    - Yêu cầu của người dùng.
    - Thông tin đã cung cấp (có thể trình bày dài hơn một chút).
    - Giải pháp đã nêu ra (có thể trình bày dài hơn một chút).
    - Quá trình tính toán.
    Tuyệt đối không đưa thêm những thông tin không có trong lịch sử hội thoại.
    """

# RESOURCES

# Init MCP server
if __name__ == "__main__":
    mcp.run(transport="stdio")
