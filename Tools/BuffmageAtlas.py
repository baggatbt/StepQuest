"""Read generated PNGs for atlas QA; never modifies the source image."""
import struct
import zlib

def read_png(path):
    image = path.read_bytes()
    width, height, depth, kind = struct.unpack(">IIBB", image[16:26])
    assert image[:8] == b"\x89PNG\r\n\x1a\n" and (depth, kind) == (8, 6), "Expected RGBA8 PNG"
    chunks = []
    cursor = 8
    while cursor < len(image):
        length = struct.unpack(">I", image[cursor:cursor+4])[0]
        if image[cursor+4:cursor+8] == b"IDAT":
            chunks.append(image[cursor+8:cursor+8+length])
        cursor += length + 12
    raw = zlib.decompress(b"".join(chunks))
    stride = width * 4
    rows, previous, cursor = [], bytearray(stride), 0
    for _ in range(height):
        mode = raw[cursor]
        row = bytearray(raw[cursor+1:cursor+1+stride])
        cursor += stride + 1
        if mode:
            for x in range(stride):
                left = row[x-4] if x >= 4 else 0
                up = previous[x]
                corner = previous[x-4] if x >= 4 else 0
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
        rows.append(row)
        previous = row
    return width, height, rows

def inspect(path, cutoff=192):
    width, height, rows = read_png(path)
    assert width % 8 == 0 and height % 6 == 0, "Atlas must be an exact 8x6 grid"
    cw, ch = width // 8, height // 6
    cells = []
    for r in range(6):
        for c in range(8):
            points, red = [], 0
            for y in range(ch):
                line = rows[r*ch+y]
                for x in range(cw):
                    k = (c*cw+x)*4
                    if line[k+3] < cutoff:
                        continue
                    points.append((x, y))
                    red += int(line[k] > 180 and line[k+1] < 40 and line[k+2] < 40)
            assert points, f"Empty frame {r},{c}"
            cells.append(dict(row=r, column=c,
                bounds=[min(x for x,y in points), min(y for x,y in points),
                        max(x for x,y in points), max(y for x,y in points)],
                pixels=len(points), red=red))
    return dict(width=width, height=height, cellWidth=cw, cellHeight=ch, cells=cells)
