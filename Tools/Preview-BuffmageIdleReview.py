"""Read-only asset comparison. No source raster or Unity binding is changed."""
from pathlib import Path
import struct,zlib,sys
from BuffmageAtlas import read_png
ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/'output/buffmage/idle-review'
W,H=400,190
canvas=bytearray(bytes([39,34,32,255])*(W*H))
def draw(path,cx,height):
 w,h,rows=read_png(path)
 points=[(x,y) for y in range(h) for x in range(w) if rows[y][x*4+3]>=192]
 x0,y0=min(x for x,y in points),min(y for x,y in points)
 x1,y1=max(x for x,y in points),max(y for x,y in points)
 scale=height/(y1-y0+1)
 dw=round((x1-x0+1)*scale)
 for y in range(height):
  sy=y0+min(y1-y0,int(y/scale))
  for x in range(dw):
   sx=x0+min(x1-x0,int(x/scale))
   p=rows[sy][sx*4:sx*4+4]
   if p[3]>=192:
    at=((155-height+y)*W+cx-dw//2+x)*4
    canvas[at:at+4]=p[:3]+b'\xff'
draw(Path('C:/Users/bagga/Downloads/HeroKnight.png'),100,100)
draw(OUT/'BuffmageIdle-bulk-v4.png',300,105)
def chunk(k,d):return struct.pack('>I',len(d))+k+d+struct.pack('>I',zlib.crc32(k+d)&0xffffffff)
raw=b''.join(b'\0'+canvas[y*W*4:(y+1)*W*4] for y in range(H))
dest=OUT/'Knight-and-Buffmage.png'
dest.write_bytes(b'\x89PNG\r\n\x1a\n'+chunk(b'IHDR',struct.pack('>IIBBBBB',W,H,8,6,0,0,0))+chunk(b'IDAT',zlib.compress(raw,9))+chunk(b'IEND',b''))
print(dest)
