
import math

from mcp.server.fastmcp import FastMCP

mcp = FastMCP(
    name="calculation",
    instructions="""
    Server cung cấp các tính năng phục vụ việc tính toán:
    - Công thức tính toán các đại lượng liên quan đến đất đai
    """,
)

# TOOLS
@mcp.tool()
def non_agricultural_land_tax(area: float, per_price: int, threshold: float) -> int:
    """
    Tính thuế sử dụng đất ở hay đất phi nông nghiệp.
    Bao gồm cả trường hợp đất ở sử dụng vào mục đích kinh doanh.
    Để tính toán cần biết:
    1. Diện tích đất
    2. Giá của 1 mét vuông đất
    3. Hạn mức đất
    """

    tax_rates = [0.03 / 100, 0.07 / 100, 0.15 / 100]

    # Level 1
    in_threshold = min(area, threshold) * per_price * tax_rates[0]
    # Level 2
    if area > threshold:
        in_3x_threshold = (area - threshold)  * per_price * tax_rates[1]
    else:
        in_3x_threshold = 0
    # Level 3
    if area > 3 * threshold:
        in_3x_threshold -= (area - 3 * threshold)  * per_price * tax_rates[1]
        out_3x_threshold = (area - 3 * threshold)  * per_price * tax_rates[2]
    else:
        out_3x_threshold = 0

    return math.ceil(in_threshold + in_3x_threshold + out_3x_threshold)

@mcp.tool()
def agricultural_land_tax(area: float, land_rank: int) -> int | str:
    """
    Tính thuế sử dụng đất ở hay đất nông nghiệp.
    Đất sử dụng cho mục đích trồng trọt và nuôi trồng thủy sản.
    Để tính toán cần biết:
    1. Diện tích đất
    2. Hạng đất dựa trên số kilôgam thóc trên một đơn vị diện tích của từng hạng đất (từ 1 - 6)
    """

    tax_rates = [550, 460, 370, 280, 180, 50]
    try:
        return math.ceil(area * tax_rates[land_rank - 1])
    except:
        return "Hạng đất không hợp lệ. Hạng đất phải thuộc đoạn từ 1 đến 6."

# PROMPTS

# RESOURCES

# Init MCP server
if __name__ == "__main__":
    mcp.run(transport="stdio")
