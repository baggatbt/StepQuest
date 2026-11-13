"""Read generated PNGs for atlas QA; never modifies the source image."""
import struct
import zlib

def read_png(path):
    image = path.read_bytes()
    width, height, depth, kind = struct.unpack(">IIBB", image[16:26])
    assert image[:8] == b"\x89PNG\r\n\x1a\n" and depth == 8 and kind in (2, 6), "Expected RGB/RGBA8 PNG"
    channels = 4 if kind == 6 else 3
    chunks = []
    cursor = 8
    while cursor < len(image):
        length = struct.unpack(">I", image[cursor:cursor+4])[0]
        if image[cursor+4:cursor+8] == b"IEND":
            break
        if image[cursor+4:cursor+8] == b"IDAT":
            chunks.append(image[cursor+8:cursor+8+length])
        cursor += length + 12
    raw = zlib.decompress(b"".join(chunks))
    stride = width * channels
    rows, previous, cursor = [], bytearray(stride), 0
    for _ in range(height):
        mode = raw[cursor]
        row = bytearray(raw[cursor+1:cursor+1+stride])
        cursor += stride + 1
        if mode:
            for x in range(stride):
                left = row[x-channels] if x >= channels else 0
                up = previous[x]
                corner = previous[x-channels] if x >= channels else 0
                if mode == 1:
                    prediction = left
                elif mode == 2:
                    prediction = up
                elif mode == 3:
                    prediction = (left + up) // 2
                elif mode == 4:
                    value = left + up - corner
                    a, b, c = abs(value-left), abs(value-up), abs(value-corner)
                    prediction = left if a <= b and a <= c else up if b <= c else corner
                else:
                    raise ValueError("Unsupported PNG filter")
                row[x] = (row[x] + prediction) & 255
        rows.append(row if channels == 4 else bytearray(v for x in range(width) for v in (*row[x*3:x*3+3],255)))
        previous = row
    return width, height, rows

def inspect(path, cutoff=192):
    width, height, rows = read_png(path)
    small_action_sheet = path.name.startswith("BuffmageApproved-")
    columns, grid_rows = (4,2) if small_action_sheet else ((7, 8) if (width, height) == (448, 512) else ((7, 7) if (width, height) == (448, 448) else (8, 6)))
    frame_count = 8 if small_action_sheet else (50 if (width, height) == (448, 512) else 48)
    assert small_action_sheet or (width % columns == 0 and height % grid_rows == 0), "Atlas dimensions must match the grid"
    cw, ch = width // columns, height // grid_rows
    cells = []
    for r in range(grid_rows):
        for c in range(columns):
            if len(cells) == frame_count:
                break
            ox, oy = round(c*width/columns), round(r*height/grid_rows)
            points, red = [], 0
            for y in range(ch):
                line = rows[oy+y]
                for x in range(cw):
                    k = (ox+x)*4
                    if line[k+3] < cutoff:
                        continue
                    points.append((x, y))
                    red += int(line[k] > 180 and line[k+1] < 40 and line[k+2] < 40)
            assert points, f"Empty frame {r},{c}"
            cells.append(dict(row=r, column=c, pixelX=ox, pixelY=oy,
                bounds=[min(x for x,y in points), min(y for x,y in points),
                        max(x for x,y in points), max(y for x,y in points)],
                pixels=len(points), red=red))
    return dict(width=width, height=height, columns=columns, rows=grid_rows, cellWidth=cw, cellHeight=ch, cells=cells)
